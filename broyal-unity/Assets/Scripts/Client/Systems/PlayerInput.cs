
using Bootstrappers;
using RemoteConfig;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;

#if true

#if SERVER_INPUT_SETUP
            var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
            var ghostId = GhostSerializerCollection.FindGhostType<CubeSnapshotData>();
            var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
            var player = EntityManager.Instantiate(prefab);
            EntityManager.SetComponentData(player, new MovableCubeComponent { PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value});

            PostUpdateCommands.AddBuffer<CubeInput>(player);
            PostUpdateCommands.SetComponent(req.SourceConnection, new CommandTargetComponent {targetEntity = player});
#endif

public struct PlayerInput : ICommandData<PlayerInput>
{
    public uint Tick => tick;
    public uint tick;
    
    public short horizontal;
    public short vertical;
    
    public short attackDirectionX;
    public short attackDirectionY;
    
    public short attackType;
    public int jump;

    public void Deserialize(uint tick, ref DataStreamReader reader)
    {
        this.tick = tick;
        horizontal = reader.ReadShort();
        vertical = reader.ReadShort();
        attackType = reader.ReadShort();
        attackDirectionX = reader.ReadShort();
        attackDirectionY = reader.ReadShort();
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteShort(horizontal);
        writer.WriteShort(vertical);
        writer.WriteShort(attackType);
        writer.WriteShort(attackDirectionX);
        writer.WriteShort(attackDirectionY);
    }

    public void Deserialize(uint tick, ref DataStreamReader reader, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Deserialize(tick, ref reader);
    }

    public void Serialize(ref DataStreamWriter writer, PlayerInput baseline, NetworkCompressionModel compressionModel)
    {
        Serialize(ref writer);
    }
}

public class PlayerInputSendCommandSystem : CommandSendSystem<PlayerInput>
{
}
public class PlayerInputReceiveCommandSystem : CommandReceiveSystem<PlayerInput>
{
}

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[UpdateBefore(typeof(PlayerInputSendCommandSystem))]
// Try to sample input as late as possible
[UpdateAfter(typeof(GhostSimulationSystemGroup))]
public class SamplePlayerInput : ComponentSystem
{
    //private readonly InputMaster _inputMaster = new InputMaster();
    private static InputMaster _inputMaster => BaseBootStrapper.Container.Resolve<InputMaster>();
    //private Session _appConfig => BaseBootStrapper.Container.Resolve<Session>();
    protected override void OnCreate()
    {
        //_inputMaster.Enable();
        
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableDOTSGhostReceiveSystemComponent>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;
        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithNone<PlayerInput>().ForEach((Entity ent, ref MovableCharacterComponent character) =>
            {
                if (character.PlayerId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<PlayerInput>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent {targetEntity = ent});
                }
            });
            return;
        }
        var inputBuffer = EntityManager.GetBuffer<PlayerInput>(localInput);
        var input = default(PlayerInput);
        
        input.tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;

        var movement = _inputMaster.Player.Movement.ReadValue<Vector2>();
        var action = (short)_inputMaster.Player.MainAction.ReadValue<float>();

        //input.horizontal = math.abs(movement.x) > 0.1 ? (movement.x > 0 ? 1 : -1) : 0;
        //input.vertical = math.abs(movement.y) > 0.1 ? (movement.y > 0 ? 1 : -1) : 0;

        input.horizontal = (short)math.round(movement.x * 10);
        input.vertical = (short)math.round(movement.y * 10);
        
        // var haveDubCmd = inputBuffer.GetDataAtTick(input.tick, out var dupCmd) && dupCmd.Tick == input.tick;
        // if (!haveDubCmd)
        // {
             input.attackType = action;
             input.attackDirectionX = (short)math.round(UIController.AttackDirection.x * 10);
             input.attackDirectionY = (short)math.round(UIController.AttackDirection.y * 10);
             
             if(input.attackType > 0)
             {
                 Debug.Log($"AttackDirection => {UIController.AttackDirection.x,6:F3} : {UIController.AttackDirection.y,6:F3}");
             }
        // }
        //    return default;

        // Debug.Log($"MainAction => {action} ");
        // Debug.Log($"AttackDirection => {UIController.AttackDirection.x,6:F3} : {UIController.AttackDirection.y,6:F3}");
        // Debug.Log($"Input => {input.horizontal} : {input.vertical}");
        // if (Input.GetKey(KeyCode.A))
        //     input.horizontal -= 1;
        // if (Input.GetKey(KeyCode.D))
        //     input.horizontal += 1;
        // if (Input.GetKey(KeyCode.S))
        //     input.vertical -= 1;
        // if (Input.GetKey(KeyCode.W))
        //     input.vertical += 1;
        //
        // if (Input.GetKey(KeyCode.Space))
        //     input.attackType = 1;
        
      
        inputBuffer.AddCommandData(input);
    }
}
#endif
