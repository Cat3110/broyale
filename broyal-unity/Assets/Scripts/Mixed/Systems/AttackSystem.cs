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
    private EntityArchetype _archetypeDot;
    protected override void OnCreate()
    {
        base.OnCreate();
        _archetypeDot = EntityManager.CreateArchetype(typeof(Translation), typeof(Dot));
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

                var skillInfo = _appConfig.GetSkillByAttackType(attack.AttackType);
                var distance = GetDistanceByAttackType(attack.AttackType);
                var direction = new float3(attack.AttackDirection.x,0,attack.AttackDirection.y);
                var byDirection = skillInfo.AimType == AimType.Direction || skillInfo.AimType == AimType.Sector || skillInfo.AimType == AimType.Trajectory;
                var center = byDirection ? translation.Value : translation.Value + (direction * skillInfo.Radius);
            
                var enemy = players
                    .Where(e => e != player)
                    .FirstOrDefault(x => 
                        math.distancesq(center, 
                            EntityManager.GetComponentData<Translation>(x).Value) < distance * distance);

                Debug.Log($"{(IsServer?"IsServer":"Client")}:Attack To => {player} => {enemy}");

                attack.Target = enemy;
            
                if (enemy != Entity.Null && skillInfo.Type == SkillType.Main)
                {
                    var damage = EntityManager.GetComponentData<Damage>(enemy);
                    Vector3 other = EntityManager.GetComponentData<Translation>(enemy).Value - translation.Value;

                    var dot = Vector3.Dot(direction, other );
                    
                    Debug.Log($"{(IsServer?"IsServer":"Client")}:Attack To => {player} => {enemy} => d => {dot}");

                    if (attack.AttackType == 1 || attack.AttackType == 2 ||
                        attack.AttackType == 3 && dot > 0.0f || attack.AttackType == 4 && dot > 0.0f )
                    {
                        ApplyDamageByAttackType(pdata, attack.AttackType, attack.AttackType * (int) (Time.ElapsedTime * 1000), 
                            player, ref damage);
                
                        EntityManager.SetComponentData(enemy, damage);
                    }
                }
                else if(skillInfo.AimType == AimType.Area || skillInfo.AimType == AimType.Dot || skillInfo.AimType == AimType.None )
                {
                    var e = EntityManager.CreateEntity(_archetypeDot);
                    EntityManager.SetComponentData(e, new Translation{ Value = center} );
                    EntityManager.SetComponentData(e, new Dot
                    {
                        Owner = player, 
                        Duration = (skillInfo.ImpactTime * pdata.magic),
                        Value = (pdata.magic * skillInfo.MagDMG) + (pdata.power * skillInfo.PhysDMG),
                        SpeedFactor = skillInfo.SpeedEffect * pdata.power,
                        HaveStun = skillInfo.Id == SkillId.ID_Magicjump,
                        Radius = skillInfo.Radius
                    } );
                }
            }
            else if (attack.AttackType != 0)
            {
                attack.Duration -= deltaTime;
                attack.DamageTime -= deltaTime;
                
                attack.AttackType = attack.Duration > 0 ? attack.AttackType : 0;
                attack.ProccesedId = attack.Duration > 0 ? attack.ProccesedId : 0;
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