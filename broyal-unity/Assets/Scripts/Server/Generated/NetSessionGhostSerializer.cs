using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

public struct NetSessionGhostSerializer : IGhostSerializer<NetSessionSnapshotData>
{
    private ComponentType componentTypeNetSession;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<NetSession> ghostNetSessionType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<NetSessionSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeNetSession = ComponentType.ReadWrite<NetSession>();
        ghostNetSessionType = system.GetArchetypeChunkComponentType<NetSession>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref NetSessionSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataNetSession = chunk.GetNativeArray(ghostNetSessionType);
        snapshot.SetNetSessionTimeToStart(chunkDataNetSession[ent].TimeToStart, serializerState);
    }
}
