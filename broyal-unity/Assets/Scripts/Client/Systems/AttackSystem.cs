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
            var pdata = EntityManager.GetComponentData<PlayerData>(player);
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

                    Vector3 forward = new Vector3(attack.AttackDirection.x,0,attack.AttackDirection.y);
                    Vector3 other = EntityManager.GetComponentData<Translation>(enemy).Value - translation.Value;

                    var dot = Vector3.Dot(forward, other );
                    
                    Debug.Log($"{(IsServer?"IsServer":"Client")}:Attack To => {player} => {enemy} => d => {dot}");

                    if (attack.AttackType == 1 || attack.AttackType == 2 ||
                        attack.AttackType == 3 && dot > 0.0f )
                    {
                        ApplyDamageByAttackType(pdata, attack.AttackType, attack.AttackType * (int) (Time.ElapsedTime * 1000), 
                            player, ref damage);
                
                        EntityManager.SetComponentData(enemy, damage);
                    }
                }
            }
            else if (attack.AttackType != 0)
            {
                attack.Duration -= deltaTime;
                attack.DamageTime -= deltaTime;
                
                attack.AttackType = attack.Duration > 0 ? attack.AttackType : 0;
                attack.ProccesedId = attack.Duration > 0 ? attack.ProccesedId : 0;
                //attack.BackAttackType
            }
            
            EntityManager.SetComponentData(player, attack); 
        }
        
        players.Dispose();
    }

    private float GetDistanceByAttackType(int attackType)
    {
        var skill = _appConfig.GetSkillByAttackType(attackType);
        return skill.Range;
    }

    private void ApplyDamageByAttackType(PlayerData pdata, int attackType, int seed, Entity attacker, ref Damage damage)
    {
        var skill = _appConfig.GetSkillByAttackType(attackType);
        //var character = _appConfig.Characters[0];
        
        damage.Value = (pdata.magic * skill.MagDMG) + (pdata.power * skill.PhysDMG);
        damage.DamageType = attackType;
        damage.Duration = 0.3f;
        damage.NeedApply = true;
        damage.Seed = seed;
        damage.Attacker = attacker;
    }
}