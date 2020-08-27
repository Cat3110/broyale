using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.NetCode;

public struct DOTSGhostDeserializerCollection : IGhostDeserializerCollection
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public string[] CreateSerializerNameList()
    {
        var arr = new string[]
        {
            "SpawnerGhostSerializer",
            "CharacterGhostSerializer",
            "CubeGhostSerializer",
            "ItemSpawnerGhostSerializer",
        };
        return arr;
    }

    public int Length => 4;
#endif
    public void Initialize(World world)
    {
        var curSpawnerGhostSpawnSystem = world.GetOrCreateSystem<SpawnerGhostSpawnSystem>();
        m_SpawnerSnapshotDataNewGhostIds = curSpawnerGhostSpawnSystem.NewGhostIds;
        m_SpawnerSnapshotDataNewGhosts = curSpawnerGhostSpawnSystem.NewGhosts;
        curSpawnerGhostSpawnSystem.GhostType = 0;
        var curCharacterGhostSpawnSystem = world.GetOrCreateSystem<CharacterGhostSpawnSystem>();
        m_CharacterSnapshotDataNewGhostIds = curCharacterGhostSpawnSystem.NewGhostIds;
        m_CharacterSnapshotDataNewGhosts = curCharacterGhostSpawnSystem.NewGhosts;
        curCharacterGhostSpawnSystem.GhostType = 1;
        var curCubeGhostSpawnSystem = world.GetOrCreateSystem<CubeGhostSpawnSystem>();
        m_CubeSnapshotDataNewGhostIds = curCubeGhostSpawnSystem.NewGhostIds;
        m_CubeSnapshotDataNewGhosts = curCubeGhostSpawnSystem.NewGhosts;
        curCubeGhostSpawnSystem.GhostType = 2;
        var curItemSpawnerGhostSpawnSystem = world.GetOrCreateSystem<ItemSpawnerGhostSpawnSystem>();
        m_ItemSpawnerSnapshotDataNewGhostIds = curItemSpawnerGhostSpawnSystem.NewGhostIds;
        m_ItemSpawnerSnapshotDataNewGhosts = curItemSpawnerGhostSpawnSystem.NewGhosts;
        curItemSpawnerGhostSpawnSystem.GhostType = 3;
    }

    public void BeginDeserialize(JobComponentSystem system)
    {
        m_SpawnerSnapshotDataFromEntity = system.GetBufferFromEntity<SpawnerSnapshotData>();
        m_CharacterSnapshotDataFromEntity = system.GetBufferFromEntity<CharacterSnapshotData>();
        m_CubeSnapshotDataFromEntity = system.GetBufferFromEntity<CubeSnapshotData>();
        m_ItemSpawnerSnapshotDataFromEntity = system.GetBufferFromEntity<ItemSpawnerSnapshotData>();
    }
    public bool Deserialize(int serializer, Entity entity, uint snapshot, uint baseline, uint baseline2, uint baseline3,
        ref DataStreamReader reader, NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_SpawnerSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 1:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_CharacterSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 2:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_CubeSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            case 3:
                return GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeDeserialize(m_ItemSpawnerSnapshotDataFromEntity, entity, snapshot, baseline, baseline2,
                baseline3, ref reader, compressionModel);
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }
    public void Spawn(int serializer, int ghostId, uint snapshot, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        switch (serializer)
        {
            case 0:
                m_SpawnerSnapshotDataNewGhostIds.Add(ghostId);
                m_SpawnerSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<SpawnerSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 1:
                m_CharacterSnapshotDataNewGhostIds.Add(ghostId);
                m_CharacterSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<CharacterSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 2:
                m_CubeSnapshotDataNewGhostIds.Add(ghostId);
                m_CubeSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<CubeSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            case 3:
                m_ItemSpawnerSnapshotDataNewGhostIds.Add(ghostId);
                m_ItemSpawnerSnapshotDataNewGhosts.Add(GhostReceiveSystem<DOTSGhostDeserializerCollection>.InvokeSpawn<ItemSpawnerSnapshotData>(snapshot, ref reader, compressionModel));
                break;
            default:
                throw new ArgumentException("Invalid serializer type");
        }
    }

    private BufferFromEntity<SpawnerSnapshotData> m_SpawnerSnapshotDataFromEntity;
    private NativeList<int> m_SpawnerSnapshotDataNewGhostIds;
    private NativeList<SpawnerSnapshotData> m_SpawnerSnapshotDataNewGhosts;
    private BufferFromEntity<CharacterSnapshotData> m_CharacterSnapshotDataFromEntity;
    private NativeList<int> m_CharacterSnapshotDataNewGhostIds;
    private NativeList<CharacterSnapshotData> m_CharacterSnapshotDataNewGhosts;
    private BufferFromEntity<CubeSnapshotData> m_CubeSnapshotDataFromEntity;
    private NativeList<int> m_CubeSnapshotDataNewGhostIds;
    private NativeList<CubeSnapshotData> m_CubeSnapshotDataNewGhosts;
    private BufferFromEntity<ItemSpawnerSnapshotData> m_ItemSpawnerSnapshotDataFromEntity;
    private NativeList<int> m_ItemSpawnerSnapshotDataNewGhostIds;
    private NativeList<ItemSpawnerSnapshotData> m_ItemSpawnerSnapshotDataNewGhosts;
}
public struct EnableDOTSGhostReceiveSystemComponent : IComponentData
{}
public class DOTSGhostReceiveSystem : GhostReceiveSystem<DOTSGhostDeserializerCollection>
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnableDOTSGhostReceiveSystemComponent>();
    }
}
