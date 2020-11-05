// When server receives go in game request, go in game and delete request
using System;
using System.Linq;
using Bootstrappers;
using RemoteConfig;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class PlayerSpawnServerSystem : ComponentSystem
{
    private EntityQuery _query;
    private AppConfig _appConfig => ServerBootstrapper.Container.Resolve<AppConfig>();
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableDOTSGhostSendSystemComponent>();
        _query = GetEntityQuery(
            ComponentType.ReadWrite<SpawnPoint>(),
            ComponentType.ReadOnly<Translation>());
    }
    
    protected override void OnUpdate()
    {
        var spawners = _query.ToEntityArray(Allocator.TempJob);
        
        Entities.WithNone<SendRpcCommandRequestComponent>()
            .ForEach((Entity reqEnt, ref PlayerSpawnRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                var character = _appConfig.Characters[0];
                
                UnityEngine.Debug.Log(
                    $"Server setting connection {EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value} to in game");
                
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);

                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                
                var ghostId = DOTSGhostSerializerCollection.FindGhostType<CharacterSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = EntityManager.Instantiate(prefab);

                var spawnPoint = spawners.OrderBy(s => EntityManager.GetComponentData<SpawnPoint>(s).spawnCount)
                    .First();
                var spawnComponent =  EntityManager.GetComponentData<SpawnPoint>(spawnPoint);
                var spawnPosition = EntityManager.GetComponentData<Translation>(spawnPoint);
                
                EntityManager.SetComponentData(spawnPoint, 
                    new SpawnPoint{ radius = spawnComponent.radius, spawnCount = spawnComponent.spawnCount + 1});
                
                EntityManager.SetComponentData(player, new Translation{ Value = spawnPosition.Value});
                EntityManager.SetComponentData(player, new PlayerData{ 
                    maxHealth = character.Health, 
                    health = character.Health,
                    power = character.Power,
                    magic = character.Magic,
                    primarySkillId = req.skillId});
                EntityManager.SetComponentData(player, 
                    new PrefabCreator{ NameId = req.characterId, SkinId = req.skinId } );
                EntityManager.SetComponentData(player, 
                    new MovableCharacterComponent { PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value});

                PostUpdateCommands.AddBuffer<PlayerInput>(player);
                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });
                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        
        spawners.Dispose();
    }
}