using Bootstrappers;
using RemoteConfig;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class DotRemoveSystem : ComponentSystem
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
            
        Entities.WithAll<SpeedMod, PlayerData>()
            .ForEach((Entity entity, ref SpeedMod speedMod, ref PlayerData playerData) =>
            {
                if (speedMod.Duration <= 0)
                {
                    playerData.speedMod -= speedMod.Value;
                    PostUpdateCommands.RemoveComponent<SpeedMod>(entity);
                }
                else speedMod.Duration -= dt;
            });
        
        Entities.WithAll<Stun, PlayerData>()
            .ForEach((Entity entity, ref Stun stun, ref PlayerData playerData) =>
            {
                if (stun.Duration <= 0)
                {
                    playerData.stun = false;
                    PostUpdateCommands.RemoveComponent<Stun>(entity);
                }
                else stun.Duration -= dt;
            });
    }
}