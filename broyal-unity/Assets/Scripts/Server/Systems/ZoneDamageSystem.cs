using Bootstrappers;
using RemoteConfig;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

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