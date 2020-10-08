using Unity.Burst;
using Unity.NetCode;
using Unity.Networking.Transport;

[BurstCompile]
public struct PlayerSpawnRequest : IRpcCommand
{
    public int skillId;
    public int characterId;
    
    [BurstCompile]
    private static void InvokeExecute(ref RpcExecutor.Parameters parameters)
    {
        RpcExecutor.ExecuteCreateRequestComponent<PlayerSpawnRequest>(ref parameters);
    }

    public PortableFunctionPointer<RpcExecutor.ExecuteDelegate> CompileExecute()
    {
        return new PortableFunctionPointer<RpcExecutor.ExecuteDelegate>(InvokeExecute);
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteInt(skillId);
        writer.WriteInt(characterId);
    }

    public void Deserialize(ref DataStreamReader reader)
    {
        skillId = reader.ReadInt();
        characterId = reader.ReadInt();
    }
}

// The system that makes the RPC request component transfer
public class PlayerSpawnRequestRpcCommandRequestSystem : RpcCommandRequestSystem<PlayerSpawnRequest>
{
}
