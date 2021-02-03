using Bootstrappers;
using RemoteConfig;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class PlayerSpawnClientSystem : ComponentSystem
{
    private Session _session => BaseBootStrapper.Container.Resolve<Session>();
    private MainConfig _mainConfig => BaseBootStrapper.Container.Resolve<MainConfig>();
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableDOTSGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        Entities.WithNone<NetworkStreamInGame>()
            .ForEach((Entity ent, ref NetworkIdComponent id) =>
            {
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
                var req = PostUpdateCommands.CreateEntity();
                if (string.IsNullOrEmpty(_session.UserId.ToString()))
                {
                    PostUpdateCommands.AddComponent(req, new PlayerSpawnRequest
                    {
                        skillId = (short)_appConfig.GetSkillIndex(_session.MainSkill),
                        skill2Id = (short)_appConfig.GetSkillIndex(_session.AttackSkill),
                        skill3Id = (short)_appConfig.GetSkillIndex(_session.DefenceSkill),
                        skill4Id = (short)_appConfig.GetSkillIndex(_session.UtilitySkill),
                        characterId = _mainConfig.GetNameId(_session.Character.Id).Id,
                        skinId = _session.Character.SkinType
                    });
                }
                else
                {
                    PostUpdateCommands.AddComponent(req, new PlayerSpawnRequest
                    {
                        userId = _session.UserId
                    });
                }
                
                PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
            });
    }
}

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class ItemSpawnerSystem : ComponentSystem
{
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    private MainConfig _config;
    protected override void OnCreate()
    {
        _config = ServerBootstrapper.Container.Resolve<MainConfig>();
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        Entities.ForEach((Entity ent, ref ItemSpawner spawner, ref Translation trans) =>
        {
            if (spawner.duration <= 0)
            {
                if (EntityManager.Exists(spawner.spawnedItem)) return;
                
                spawner.duration = 100;
                
                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                
                var ghostId = DOTSGhostSerializerCollection.FindGhostType<ItemSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var loot = EntityManager.Instantiate(prefab);

                var itemInfo = _appConfig.Items[spawner.itemId];
                var nameId = _config.GetNameId(itemInfo.Id);
                
                //var loot = EntityManager.CreateEntity();
                //EntityManager.AddComponent<ItemComponent>(loot);
                
                EntityManager.SetComponentData(loot, new Translation{ Value = new float3(trans.Value.x, 0.5f, trans.Value.z)});
                EntityManager.SetComponentData(loot, new ItemComponent{ Id = spawner.itemId});
                EntityManager.SetComponentData(loot, new PrefabCreator{ NameId = nameId.Id });

                spawner.spawnedItem = loot;
            }
            else
            {
                spawner.duration -= dt;
            }
        });
    }
}

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class LootItemSystem : ComponentSystem
{
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    private MainConfig _config;
    private EntityQuery _playersQuery;
    private EntityQuery _itemsQuery;

    private const float LootDist = 0.6f;

    protected override void OnCreate()
    {
        _config = ServerBootstrapper.Container.Resolve<MainConfig>();

        _playersQuery = GetEntityQuery(
            ComponentType.ReadOnly<Attack>(),
            ComponentType.ReadWrite<PlayerData>(),
            ComponentType.ReadOnly<Damage>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.Exclude<DEAD>());
        
        _itemsQuery = GetEntityQuery(
            ComponentType.ReadOnly<ItemComponent>(),
            ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        
        var players = _playersQuery.ToEntityArray(Allocator.TempJob);
        var items = _itemsQuery.ToEntityArray(Allocator.TempJob);

        for (int i = 0; i < players.Length; i++)
        {
            var player = players[i];
            var playerPos = EntityManager.GetComponentData<Translation>(player).Value;

            for (int j = 0; j < items.Length; j++)
            {
                var item = items[j];
                var itemPos = EntityManager.GetComponentData<Translation>(items[j]).Value;
                if (math.abs(itemPos.x - playerPos.x) < LootDist && math.abs(itemPos.z - playerPos.z) < LootDist)
                {
                    //Debug.Log($"Loot it => {playerPos} <> {itemPos}");
                    var itemComponent = EntityManager.GetComponentData<ItemComponent>(item);
                    var itemInfo = _appConfig.Items[itemComponent.Id];
                    
                    var pdata = EntityManager.GetComponentData<PlayerData>(player);
                    pdata.AddItem(itemInfo);
                    EntityManager.SetComponentData(player, pdata);
                    
                    PostUpdateCommands.DestroyEntity(item);
                }
            }
        }

        items.Dispose();
        players.Dispose();
    }
}