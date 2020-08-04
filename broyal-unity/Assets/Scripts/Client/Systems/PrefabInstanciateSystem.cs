using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[DisableAutoCreation]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class PrefabInstanciateSystem : SystemBase
{
    EntityCommandBufferSystem _ecb;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        var config = Bootstrappers.ClientBootstrapper.Container.Resolve<MainConfig>();
        _ecb = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    // OnUpdate runs on the main thread.
    protected override void OnUpdate()
    {
        var commandBuffer = _ecb.CreateCommandBuffer().ToConcurrent();
        var deltaTime = Time.DeltaTime;

        /*Entities.ForEach((Entity entity, int nativeThreadIndex, ref Animator prefab) =>
        {
            Debug.Log("Fuck");
            //commandBuffer.RemoveComponent<PrefabCreator>(nativeThreadIndex,entity);
            //new GameObject("Fuck");
        }).ScheduleParallel();*/
        
        Entities.WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, in Animator prefab) =>
        {
            //Debug.Log("Fuck");
            //commandBuffer.RemoveComponent<PrefabCreator>(nativeThreadIndex,entity);
            //new GameObject("Fuck");
        }).Run();
        
        _ecb.AddJobHandleForProducer(Dependency);
    }
}
