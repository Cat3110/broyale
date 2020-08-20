using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct DOTSGhostSerializerCollection : IGhostSerializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "SpawnerGhostSerializer",
            "CharacterGhostSerializer",
            "CubeGhostSerializer",
        };
        return arr;
    }

    public int Length => 3;
#endif
    public static int FindGhostType<T>()
        where T : struct, ISnapshotData<T>
    {
        if (typeof(T) == typeof(SpawnerSnapshotData))
            return 0;
        if (typeof(T) == typeof(CharacterSnapshotData))
            return 1;
        if (typeof(T) == typeof(CubeSnapshotData))
            return 2;
        return -1;
    }

    public void BeginSerialize(ComponentSystemBase system)
    {
        m_SpawnerGhostSerializer.BeginSerialize(system);
        m_CharacterGhostSerializer.BeginSerialize(system);
        m_CubeGhostSerializer.BeginSerialize(system);
    }

    public int CalculateImportance(int serializer, ArchetypeChunk chunk)
    {
        switch (serializer)
        {
            case 0:
                return m_SpawnerGhostSerializer.CalculateImportance(chunk);
            case 1:
                return m_CharacterGhostSerializer.CalculateImportance(chunk);
            case 2:
                return m_CubeGhostSerializer.CalculateImportance(chunk);
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int GetSnapshotSize(int serializer)
    {
        switch (serializer)
        {
            case 0:
                return m_SpawnerGhostSerializer.SnapshotSize;
            case 1:
                return m_CharacterGhostSerializer.SnapshotSize;
            case 2:
                return m_CubeGhostSerializer.SnapshotSize;
        }

        throw new ArgumentException("Invalid serializer type");
    }

    public int Serialize(ref DataStreamWriter dataStream, SerializeData data)
    {
        switch (data.ghostType)
        {
            case 0:
            {
                return GhostSendSystem<DOTSGhostSerializerCollection>.InvokeSerialize<SpawnerGhostSerializer, SpawnerSnapshotData>(m_SpawnerGhostSerializer, ref dataStream, data);
            }
            case 1:
            {
                return GhostSendSystem<DOTSGhostSerializerCollection>.InvokeSerialize<CharacterGhostSerializer, CharacterSnapshotData>(m_CharacterGhostSerializer, ref dataStream, data);
            }
            case 2:
            {
                return GhostSendSystem<DOTSGhostSerializerCollection>.InvokeSerialize<CubeGhostSerializer, CubeSnapshotData>(m_CubeGhostSerializer, ref dataStream, data);
            }
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    private SpawnerGhostSerializer m_SpawnerGhostSerializer;
    private CharacterGhostSerializer m_CharacterGhostSerializer;
    private CubeGhostSerializer m_CubeGhostSerializer;
}

public struct EnableDOTSGhostSendSystemComponent : IComponentData
{}
public class DOTSGhostSendSystem : GhostSendSystem<DOTSGhostSerializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableDOTSGhostSendSystemComponent>();
    }

    public override bool IsEnabled()
    {
        return HasSingleton<EnableDOTSGhostSendSystemComponent>();
    }
}