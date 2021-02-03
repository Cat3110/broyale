using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using Unity.Networking.Transport;

[BurstCompile]
public struct PlayerSpawnRequest : IRpcCommand
{
    public short skillId;
    public short skill2Id;
    public short skill3Id;
    public short skill4Id;
    
    public uint characterId;
    public uint skinId;
    
    public NativeString64 userId;

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
        writer.WriteShort(skillId);
        writer.WriteShort(skill2Id);
        writer.WriteShort(skill3Id);
        writer.WriteShort(skill4Id);
        writer.WriteInt((int)characterId);
        writer.WriteInt((int)skinId);
        writer.WriteString(userId);
    }

    public void Deserialize(ref DataStreamReader reader)
    {
        skillId = reader.ReadShort();
        skill2Id = reader.ReadShort();
        skill3Id = reader.ReadShort();
        skill4Id = reader.ReadShort();
        characterId = (uint)reader.ReadInt();
        skinId = (uint)reader.ReadInt();
        userId = reader.ReadString();
    }
}

// The system that makes the RPC request component transfer
public class PlayerSpawnRequestRpcCommandRequestSystem : RpcCommandRequestSystem<PlayerSpawnRequest>
{
}
