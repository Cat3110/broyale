﻿using Unity.Burst;
using Unity.Collections;
using Unity.NetCode;
using Unity.Networking.Transport;

[BurstCompile]
public struct PlayerSpawnRequest : IRpcCommand
{
    public int skillId;
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
        writer.WriteInt(skillId);
        writer.WriteInt((int)characterId);
        writer.WriteInt((int)skinId);
        writer.WriteString(userId);
    }

    public void Deserialize(ref DataStreamReader reader)
    {
        skillId = reader.ReadInt();
        characterId = (uint)reader.ReadInt();
        skinId = (uint)reader.ReadInt();
        userId = reader.ReadString();
    }
}

// The system that makes the RPC request component transfer
public class PlayerSpawnRequestRpcCommandRequestSystem : RpcCommandRequestSystem<PlayerSpawnRequest>
{
}
