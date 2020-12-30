
     using Unity.Entities;
     using Unity.NetCode;
     using UnityEngine;

     /// <summary>
    /// System to clear all ghosts on the client
    /// </summary>
    [UpdateBefore(typeof(ConnectionSystem))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    public class ClearClientGhostEntities : SystemBase
    {
        protected EndSimulationEntityCommandBufferSystem commandBufferSystem;
 
        public struct ClientClearGhosts : IComponentData {};
 
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ClientClearGhosts>();
            commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
 
        protected override void OnUpdate()
        {
            var buffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent();
            // Also delete the existing ghost objects
            Entities.ForEach((Entity e, int entityInQueryIndex, ref GhostComponent ghost) =>
            {
                Debug.Log($"DestroyEntity{e.Index}");
                buffer.DestroyEntity(entityInQueryIndex, e);
            }).ScheduleParallel();
            
            Entities.ForEach((Entity e, int entityInQueryIndex, ref GhostPrefabCollectionComponent ghost) =>
            {
                Debug.Log($"DestroyEntity{e.Index}");
                buffer.DestroyEntity(entityInQueryIndex, e);
            }).ScheduleParallel();
            
            commandBufferSystem.CreateCommandBuffer().DestroyEntity(GetSingletonEntity<ClientClearGhosts>());
        }
    }
 
    /// <summary>
    /// System to handle disconnecting client from the server
    /// </summary>
    [UpdateBefore(typeof(GhostReceiveSystem<DOTSGhostDeserializerCollection>))]
    [UpdateInGroup(typeof(ClientSimulationSystemGroup))]
    
    // [UpdateInGroup(typeof(NetworkReceiveSystemGroup))]
    // [UpdateBefore(typeof(NetworkStreamReceiveSystem))]
    // [AlwaysUpdateSystem]
    public class ConnectionSystem : ComponentSystem
    {
        /// <summary>
        /// Has a disconnect been requested
        /// </summary>
        public static bool disconnectRequested;
    
        // Some extra code that doesn't matter...
    
        protected override void OnUpdate()
        {
            if (disconnectRequested)
            {
                Debug.Log("Attempting to disconnect");
                Entities.ForEach((Entity ent, ref NetworkStreamConnection conn) =>
                {
                    EntityManager.AddComponent(ent, typeof(NetworkStreamRequestDisconnect));
                    EntityManager.CreateEntity(ComponentType.ReadOnly(typeof(ClearClientGhostEntities.ClientClearGhosts)));
                });
                disconnectRequested = false;
            }
        }
    }