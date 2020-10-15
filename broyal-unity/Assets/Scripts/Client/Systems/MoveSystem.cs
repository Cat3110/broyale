using System.Collections.Generic;
using Bootstrappers;
using RemoteConfig;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveSystem : ComponentSystem
{
    private bool _isServer;
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    private List<ColliderData> _colliders => BaseBootStrapper.Container.Resolve<List<ColliderData>>();
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

            if (pdata.health <= 0) return;

            inputBuffer.GetDataAtTick(tick, out PlayerInput input);

            float h = input.horizontal / 10.0f ;// > 0 ? 1.0f : (input.horizontal < 0) ? -1.0f : 0.0f;
            float v = input.vertical / 10.0f;//  > 0 ? 1.0f : (input.vertical < 0) ? -1.0f : 0.0f;

            var lastPos = trans.Value;
            
            if (math.abs(input.horizontal) > 0)
                trans.Value.x += input.horizontal / 10.0f * deltaTime * _appConfig.Characters[0].Speed;

            if (math.abs(input.vertical) > 0) 
                trans.Value.z += input.vertical / 10.0f * deltaTime * _appConfig.Characters[0].Speed;
            
            var collided = false;
            
            //if (_isServer)
            {
                for (int i = 0; i < _colliders.Count; i++)
                {
                    var collider = _colliders[i];
                    if (collider.Type == ColliderType.Box)
                    {
                        collided = Intersect(collider.Min, collider.Max, trans.Value, new float3(0.5f));
                        if (collided)
                        {
                            trans.Value = lastPos;
                            break;
                        }
                    }
                }
            }

            //physicsVelocity.Linear = new float3(h * deltaTime * _appConfig.Characters[0].Speed * 50.0f,
            //    0, v * deltaTime * _appConfig.Characters[0].Speed * 50.0f);
            
            /*if (input.horizontal > 0)
                trans.Value.x += input.horizontal/10.0f * deltaTime;
            if (input.horizontal < 0)
                trans.Value.x -= input.horizontal/10.0f * deltaTime;
            if (input.vertical > 0)
                trans.Value.z += input.vertical/10.0f * deltaTime;
            if (input.vertical < 0)
                trans.Value.z -= input.vertical/10.0f * deltaTime;*/
            // if (math.abs(input.horizontal) > 0 || math.abs(input.vertical) > 0)
            // {
            //     attack.PredTrans = new float3(input.horizontal, 0, input.vertical);
            // }
            
            if (math.abs(input.attackDirectionX) > 0 || math.abs(input.attackDirectionY) > 0)
            {
                attack.AttackDirection = new float2(input.attackDirectionX / 10.0f, input.attackDirectionY / 10.0f);
            }
            

            if (input.attackType == 1 && attack.ProccesedId == 0 && attack.AttackType == 0)
            {
                Debug.Log(
                    $"{(_isServer ? "Server" : "Client")}({tick})({input.tick}):Attack Start  => {e} => {attack.NeedApplyDamage} => {Time.ElapsedTime}");
                InitAttackByType(pdata.primarySkillId, trans.Value, ref attack, input.attackType * (int) (Time.ElapsedTime));
                
                //EntityManager.SetComponentData(e, attack);
            }
        });
        
    }
    public static bool Intersect(float3 posA, float3 sizeA, float3 posB, float3 sizeB)
    {
        float minAX = posA.x - sizeA.x * 0.5f;
        float maxAX = posA.x + sizeA.x * 0.5f;
        
        float minAY = posA.z - sizeA.z * 0.5f;
        float maxAY = posA.z + sizeA.z * 0.5f;
        
        float minBX = posB.x - sizeB.x * 0.5f;
        float maxBX = posB.x + sizeB.x * 0.5f;
        
        float minBY = posB.z - sizeB.z * 0.5f;
        float maxBY = posB.z + sizeB.z * 0.5f;

        return (minAX <= maxBX && maxAX >= minBX) && (minAY <= maxBY && maxAY >= minBY);
    }
    
    public static bool Intersect(float2 min1, float2 max1, float3 posB, float3 sizeB)
    {
        var x1 = posB.x - sizeB.x * 0.5f;
        var x2 = posB.x + sizeB.x * 0.5f;
        
        var y1 = posB.z - sizeB.z * 0.5f;
        var y2 = posB.z + sizeB.z * 0.5f;

        float2 min2 = new float2(math.min(x1, x2), math.min(y1, y2));
        float2 max2 = new float2(math.max(x1, x2), math.max(y1, y2));

        return (min1.x <= max2.x && max1.x >= min2.x) && (min1.y <= max2.y && max1.y >= min2.y);
    }
    
    static bool AreSquaresOverlapping(float3 posA, float sizeA, float3 posB, float sizeB)
    {
        float d = (sizeA / 2) + (sizeB / 2);
        return (math.abs(posA.x - posB.x) < d && math.abs(posA.z - posB.z) < d);
    }
    
    private void InitAttackByType(int skillId, float3 transValue, ref Attack attack, int seed)
    {
        var skill = _appConfig.Skills[skillId];
        
        Debug.Log($"{(_isServer ? "Server" : "Client")}:Attack Start  => {skill.Id}");

       
        attack.Duration = skill.Cooldown;
        //attack.BackAttackType = 
        attack.AttackType = skillId + 1;
        attack.ProccesedId = attack.AttackType;
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