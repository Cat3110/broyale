using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

public struct CubeGhostSerializer : IGhostSerializer<CubeSnapshotData>
{
    private ComponentType componentTypeMovableCharacterComponent;
    private ComponentType componentTypePlayerData;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<MovableCharacterComponent> ghostMovableCharacterComponentType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerData> ghostPlayerDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<CubeSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeMovableCharacterComponent = ComponentType.ReadWrite<MovableCharacterComponent>();
        componentTypePlayerData = ComponentType.ReadWrite<PlayerData>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostMovableCharacterComponentType = system.GetArchetypeChunkComponentType<MovableCharacterComponent>(true);
        ghostPlayerDataType = system.GetArchetypeChunkComponentType<PlayerData>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref CubeSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataMovableCharacterComponent = chunk.GetNativeArray(ghostMovableCharacterComponentType);
        var chunkDataPlayerData = chunk.GetNativeArray(ghostPlayerDataType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetMovableCharacterComponentPlayerId(chunkDataMovableCharacterComponent[ent].PlayerId, serializerState);
        snapshot.SetPlayerDatahealth(chunkDataPlayerData[ent].health, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
