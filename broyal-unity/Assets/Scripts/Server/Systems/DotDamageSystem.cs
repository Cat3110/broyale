using Bootstrappers;
using RemoteConfig;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class DotDamageSystem : ComponentSystem
{
    private AppConfig _appConfig => BaseBootStrapper.Container.Resolve<AppConfig>();
    private MainConfig _config;
    private EntityQuery _playesQuery;
    private EntityQuery _dotsQuery;

    private float _decreaseTick = 1.0f;
    private float _lastTick = 0;

    protected override void OnCreate()
    {
        _config = ServerBootstrapper.Container.Resolve<MainConfig>();

        _playesQuery = GetEntityQuery(
            ComponentType.ReadOnly<Attack>(),
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.ReadWrite<Damage>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.Exclude<DEAD>());
        
        _dotsQuery = GetEntityQuery(
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadWrite<Dot>());
    }

    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        if (_lastTick <= 0)
        {
            _lastTick = _decreaseTick;
            
            var dots = _dotsQuery.ToEntityArray(Allocator.TempJob);
            var players = _playesQuery.ToEntityArray(Allocator.TempJob);

            foreach (var dot in dots)
            {
                var position = EntityManager.GetComponentData<Translation>(dot).Value;
                var component = EntityManager.GetComponentData<Dot>(dot);
                foreach (var player in players)
                {
                    if(player == component.Owner) continue;
                    
                    var playerPos = EntityManager.GetComponentData<Translation>(player).Value;
                    var playerData = EntityManager.GetComponentData<PlayerData>(player);
                    if (math.distancesq(position, playerPos) < (component.Radius * component.Radius))
                    {
                        var damage = EntityManager.GetComponentData<Damage>(player);
                    
                        damage.Value += component.Value;
                        damage.DamageType = 5;
                        damage.Duration = 0.3f;
                        damage.NeedApply = true;
                        damage.Seed = (int)dt * 1000;
                        EntityManager.SetComponentData(player, damage);

                        if (math.abs(component.SpeedFactor) > 0 && !EntityManager.HasComponent<SpeedMod>(player) )
                        {
                            playerData.speedMod += component.SpeedFactor;
                            EntityManager.SetComponentData(player, playerData);
                            EntityManager.AddComponentData(player, new SpeedMod{Duration = component.Duration*1000f, Value = component.SpeedFactor});
                        }
                        else if((math.abs(component.SpeedFactor) > 0 && !EntityManager.HasComponent<Stun>(player)))
                        {
                            playerData.stun = true;
                            EntityManager.SetComponentData(player, playerData);
                            EntityManager.AddComponentData(player, new Stun{Duration = component.Duration*1000f});
                        }
                    }
                }

                component.Duration -= _decreaseTick;
                EntityManager.SetComponentData(dot, component);
                
                if(component.Duration <= 0.0f )
                    EntityManager.DestroyEntity(dot);
            }
            players.Dispose();
            dots.Dispose();
        }
        else
        {
            _lastTick -= dt;
        }
    }
}