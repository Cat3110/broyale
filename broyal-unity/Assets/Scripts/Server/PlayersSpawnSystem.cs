// When server receives go in game request, go in game and delete request
using System;
using Bootstrappers;
using RemoteConfig;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class PlayerSpawnServerSystem : ComponentSystem
{
    private AppConfig _appConfig => ServerBootstrapper.Container.Resolve<AppConfig>();
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableDOTSGhostSendSystemComponent>();
    }
    
    protected override void OnUpdate()
    {
        Entities.WithNone<SendRpcCommandRequestComponent>()
            .ForEach((Entity reqEnt, ref PlayerSpawnRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                UnityEngine.Debug.Log(
                    $"Server setting connection {EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value} to in game");
                
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);

                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                
                var ghostId = DOTSGhostSerializerCollection.FindGhostType<CharacterSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = EntityManager.Instantiate(prefab);
                
                EntityManager.SetComponentData(player, new PlayerData{health = _appConfig.Characters[0].Health});
                EntityManager.SetComponentData(player, new MovableCharacterComponent { PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});

                PostUpdateCommands.AddBuffer<PlayerInput>(player);
                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });
                PostUpdateCommands.DestroyEntity(reqEnt);
            });
    }
}