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
                        skillId = _session.SkillId,
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

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class ZoneDamageSystem : ComponentSystem
{
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    private MainConfig _config;
    private EntityQuery _playesQuery;

    private float _areaSize = 110f;
    private float _decreaseTick = 2.0f;
    private float _decreaseAmount = 1.0f;
    
    private float lastTick = 0;

    protected override void OnCreate()
    {
        _config = ServerBootstrapper.Container.Resolve<MainConfig>();

        _areaSize = _appConfig.Main.MapAreaSize;
        _decreaseTick = _appConfig.Main.MapAreaDecreaseTick;
        _decreaseAmount = _appConfig.Main.MapAreaDecreaseAmount;

        _playesQuery = GetEntityQuery(
            ComponentType.ReadOnly<Attack>(),
            ComponentType.ReadWrite<PlayerData>(),
            ComponentType.ReadWrite<Damage>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.Exclude<DEAD>());
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        if (lastTick <= 0)
        {
            lastTick = _decreaseTick;
            _areaSize -= _decreaseAmount;
            
            var players = _playesQuery.ToEntityArray(Allocator.TempJob);
            foreach (var player in players)
            {
                var playerPos = EntityManager.GetComponentData<Translation>(player).Value;
                
                if (math.abs(playerPos.x) > _areaSize || math.abs(playerPos.y) > _areaSize)
                {
                    var damage = EntityManager.GetComponentData<Damage>(player);
                    
                    damage.Value += 100;
                    damage.DamageType = 5;
                    damage.Duration = 0.3f;
                    damage.NeedApply = true;
                    damage.Seed = (int)dt * 1000;
                    
                    EntityManager.SetComponentData(player, damage);
                }
                
                var playerData = EntityManager.GetComponentData<PlayerData>(player);
                playerData.damageRadius = (int)_areaSize;
                
                EntityManager.SetComponentData(player, playerData);
            }
            players.Dispose();
        }
        else
        {
            lastTick -= dt;
        }
    }
}