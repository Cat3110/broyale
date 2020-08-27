// When client has a connection with network id, go in game and tell server to also go in game

using Bootstrappers;
using RemoteConfig;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class PlayerSpawnClientSystem : ComponentSystem
{
    private Session _session => BaseBootStrapper.Container.Resolve<Session>();
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
                PostUpdateCommands.AddComponent(req, new PlayerSpawnRequest { skillId = _session.SkillId});
                PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
            });
    }
}
