using Bootstrappers;
using RemoteConfig;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveSystem : ComponentSystem
{
    private bool _isServer;
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        _isServer = World.GetExistingSystem<ServerSimulationSystemGroup>() != null;

        // otherPlayers = GetEntityQuery(
        //     ComponentType.ReadWrite<PlayerData>(),
        //     ComponentType.ReadWrite<Damage>(),
        //     ComponentType.ReadOnly<Translation>(),
        //     ComponentType.Exclude<PlayerInput>());
    }

    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity e, DynamicBuffer<PlayerInput> inputBuffer, ref Attack attack, ref Damage damage,
            ref Translation trans, ref PlayerData pdata, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            inputBuffer.GetDataAtTick(tick, out PlayerInput input);

            if (math.abs(input.horizontal) > 0)
                trans.Value.x += input.horizontal / 10.0f * deltaTime * _appConfig.Characters[0].Speed;

            if (math.abs(input.vertical) > 0)
                trans.Value.z += input.vertical / 10.0f * deltaTime * _appConfig.Characters[0].Speed;

            /*if (input.horizontal > 0)
                trans.Value.x += input.horizontal/10.0f * deltaTime;
            if (input.horizontal < 0)
                trans.Value.x -= input.horizontal/10.0f * deltaTime;
            if (input.vertical > 0)
                trans.Value.z += input.vertical/10.0f * deltaTime;
            if (input.vertical < 0)
                trans.Value.z -= input.vertical/10.0f * deltaTime;*/
            if (math.abs(input.horizontal) > 0 || math.abs(input.vertical) > 0)
            {
                attack.PredTrans = new float3(input.horizontal, 0,input.vertical );
            }

            if (input.attackType == 1 && attack.AttackType == 0) //Range
            {
                Debug.Log(
                    $"{(_isServer ? "Server" : "Client")}({tick})({input.tick}):Attack Start  => {e} => {attack.NeedApplyDamage} => {Time.ElapsedTime}");
                InitAttackByType(pdata.primarySkillId, trans.Value, ref attack, input.attackType * (int) (Time.ElapsedTime * 1000));
            }
        });
    }

    private void InitAttackByType(int skillId, float3 transValue, ref Attack attack, int seed)
    {
        var skill = _appConfig.Skills[skillId];
        
        Debug.Log($"{(_isServer ? "Server" : "Client")}:Attack Start  => {skill.Id}");
        
        attack.Duration = skill.Cooldown;
        attack.AttackType = skillId + 1;
        //attack.PredTrans = transValue;
        
        if (attack.AttackType == 1)
        {
            attack.DamageTime = 0.5f;
            attack.Seed = seed;
            attack.NeedApplyDamage = true;
        }
        else if (attack.AttackType == 2)
        {
            attack.DamageTime = 0.5f;
            attack.Seed = seed;
            attack.NeedApplyDamage = true;
        }
        else if (attack.AttackType == 3)
        {
            attack.DamageTime = 0.5f;
            attack.Seed = seed;
            attack.NeedApplyDamage = true;
        }
        else if (attack.AttackType == 4)
        {
            attack.DamageTime = 0.5f;
            attack.Seed = seed;
            attack.NeedApplyDamage = true;
        }
    }
}