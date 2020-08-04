using System.Linq;
using Bootstrappers;
using RemoteConfig;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
public class AttackSystem : ComponentSystem
{
    private EntityQuery _query;
    private bool IsServer { get; set; }
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        IsServer = World.GetExistingSystem<ServerSimulationSystemGroup>() != null;
        
        _query = GetEntityQuery(
            ComponentType.ReadOnly<Attack>(),
            ComponentType.ReadWrite<Damage>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.Exclude<DEAD>());
    }
    
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var players = _query.ToEntityArray(Allocator.TempJob);

        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i];
            var attack = EntityManager.GetComponentData<Attack>(player);
            var translation = EntityManager.GetComponentData<Translation>(player);
            //var input = EntityManager.GetBuffer<PlayerInput>(player);

            if (attack.NeedApplyDamage && attack.DamageTime < 0)
            {
                attack.NeedApplyDamage = false;

                var distance = GetDistanceByAttackType(attack.AttackType);
            
                var enemy = players
                    .Where(e => e != player)
                    .FirstOrDefault(x => 
                        math.distancesq(translation.Value, 
                            EntityManager.GetComponentData<Translation>(x).Value) < distance * 2);

                Debug.Log($"{(IsServer?"IsServer":"Client")}:Attack To => {player} => {enemy}");

                attack.Target = enemy;
            
                if (enemy != Entity.Null)
                {
                    var damage = EntityManager.GetComponentData<Damage>(enemy);
                
                    ApplyDamageByAttackType(attack.AttackType, attack.AttackType * (int) (Time.ElapsedTime * 1000), 
                        player, ref damage);
                
                    EntityManager.SetComponentData(enemy, damage);
                }
            }
            else if (attack.AttackType != 0)
            {
                attack.Duration -= deltaTime;
                attack.DamageTime -= deltaTime;
                
                attack.AttackType = attack.Duration > 0 ? attack.AttackType : 0;
            }
            
            EntityManager.SetComponentData(player, attack); 
        }
        
        players.Dispose();
    }

    private float GetDistanceByAttackType(int attackType)
    {
        switch (attackType)
        {
            case 1:
                return _appConfig.Skills[1].Range;
            case 2:
                return 1.0f;
            default:
                return float.MaxValue;
        }
    }

    private static void ApplyDamageByAttackType(int attackType, int seed, Entity attacker, ref Damage damage)
    {
        var damageValue = attackType * 1.0f;
        
        damage.DamageType = attackType;
        damage.Duration = 0.3f;
        damage.NeedApply = true;
        damage.Seed = seed;
        damage.Attacker = attacker;
        
    }
}