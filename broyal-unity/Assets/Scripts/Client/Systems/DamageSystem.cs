using Bootstrappers;
using RemoteConfig;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
[UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
public class DamageSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem barrier;

    protected override void OnCreate()
    {
        barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    private struct DamageJob : IJobForEachWithEntity<PlayerData, Damage>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        public float DeltaTime;
        private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
        
        [ReadOnly] public ComponentDataFromEntity<DEAD> Dead;
        
        public void Execute(Entity entity, int index, ref PlayerData playerData, ref Damage damage)
        {
            if ( damage.NeedApply )
            {
                damage.NeedApply = false;
                
                playerData.health = Mathf.RoundToInt( playerData.health  - damage.Value );
                
                if (playerData.health <= 0.0f && !Dead.Exists(entity) )
                {
                    Ecb.AddComponent(index, entity, new DEAD());
                }
            }
            else if (damage.DamageType != 0)
            {
                damage.Duration -= DeltaTime;
                damage.DamageType = damage.Duration > 0 ? damage.DamageType : 0;
            }
        }
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DamageJob
        {
            Ecb = barrier.CreateCommandBuffer().ToConcurrent(),
            Dead = GetComponentDataFromEntity<DEAD>(),
            DeltaTime = Time.DeltaTime
        };
        inputDeps = job.Schedule(this, inputDeps);
        barrier.AddJobHandleForProducer(inputDeps);
        return inputDeps;
    }
}