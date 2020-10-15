using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;

public struct CharacterGhostSerializer : IGhostSerializer<CharacterSnapshotData>
{
    private ComponentType componentTypeAttack;
    private ComponentType componentTypeDamage;
    private ComponentType componentTypeMovableCharacterComponent;
    private ComponentType componentTypePlayerData;
    private ComponentType componentTypePrefabCreator;
    private ComponentType componentTypeLocalToWorld;
    private ComponentType componentTypeRotation;
    private ComponentType componentTypeTranslation;
    // FIXME: These disable safety since all serializers have an instance of the same type - causing aliasing. Should be fixed in a cleaner way
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Attack> ghostAttackType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Damage> ghostDamageType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<MovableCharacterComponent> ghostMovableCharacterComponentType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PlayerData> ghostPlayerDataType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<PrefabCreator> ghostPrefabCreatorType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Rotation> ghostRotationType;
    [NativeDisableContainerSafetyRestriction][ReadOnly] private ArchetypeChunkComponentType<Translation> ghostTranslationType;


    public int CalculateImportance(ArchetypeChunk chunk)
    {
        return 1;
    }

    public int SnapshotSize => UnsafeUtility.SizeOf<CharacterSnapshotData>();
    public void BeginSerialize(ComponentSystemBase system)
    {
        componentTypeAttack = ComponentType.ReadWrite<Attack>();
        componentTypeDamage = ComponentType.ReadWrite<Damage>();
        componentTypeMovableCharacterComponent = ComponentType.ReadWrite<MovableCharacterComponent>();
        componentTypePlayerData = ComponentType.ReadWrite<PlayerData>();
        componentTypePrefabCreator = ComponentType.ReadWrite<PrefabCreator>();
        componentTypeLocalToWorld = ComponentType.ReadWrite<LocalToWorld>();
        componentTypeRotation = ComponentType.ReadWrite<Rotation>();
        componentTypeTranslation = ComponentType.ReadWrite<Translation>();
        ghostAttackType = system.GetArchetypeChunkComponentType<Attack>(true);
        ghostDamageType = system.GetArchetypeChunkComponentType<Damage>(true);
        ghostMovableCharacterComponentType = system.GetArchetypeChunkComponentType<MovableCharacterComponent>(true);
        ghostPlayerDataType = system.GetArchetypeChunkComponentType<PlayerData>(true);
        ghostPrefabCreatorType = system.GetArchetypeChunkComponentType<PrefabCreator>(true);
        ghostRotationType = system.GetArchetypeChunkComponentType<Rotation>(true);
        ghostTranslationType = system.GetArchetypeChunkComponentType<Translation>(true);
    }

    public void CopyToSnapshot(ArchetypeChunk chunk, int ent, uint tick, ref CharacterSnapshotData snapshot, GhostSerializerState serializerState)
    {
        snapshot.tick = tick;
        var chunkDataAttack = chunk.GetNativeArray(ghostAttackType);
        var chunkDataDamage = chunk.GetNativeArray(ghostDamageType);
        var chunkDataMovableCharacterComponent = chunk.GetNativeArray(ghostMovableCharacterComponentType);
        var chunkDataPlayerData = chunk.GetNativeArray(ghostPlayerDataType);
        var chunkDataPrefabCreator = chunk.GetNativeArray(ghostPrefabCreatorType);
        var chunkDataRotation = chunk.GetNativeArray(ghostRotationType);
        var chunkDataTranslation = chunk.GetNativeArray(ghostTranslationType);
        snapshot.SetAttackAttackType(chunkDataAttack[ent].AttackType, serializerState);
        snapshot.SetAttackSeed(chunkDataAttack[ent].Seed, serializerState);
        snapshot.SetAttackAttackDirection(chunkDataAttack[ent].AttackDirection, serializerState);
        snapshot.SetDamageDamageType(chunkDataDamage[ent].DamageType, serializerState);
        snapshot.SetMovableCharacterComponentPlayerId(chunkDataMovableCharacterComponent[ent].PlayerId, serializerState);
        snapshot.SetPlayerDatahealth(chunkDataPlayerData[ent].health, serializerState);
        snapshot.SetPlayerDataprimarySkillId(chunkDataPlayerData[ent].primarySkillId, serializerState);
        snapshot.SetPlayerDatamaxHealth(chunkDataPlayerData[ent].maxHealth, serializerState);
        snapshot.SetPlayerDatapower(chunkDataPlayerData[ent].power, serializerState);
        snapshot.SetPlayerDatamagic(chunkDataPlayerData[ent].magic, serializerState);
        snapshot.SetPlayerDatadamageRadius(chunkDataPlayerData[ent].damageRadius, serializerState);
        snapshot.SetPrefabCreatorNameId(chunkDataPrefabCreator[ent].NameId, serializerState);
        snapshot.SetRotationValue(chunkDataRotation[ent].Value, serializerState);
        snapshot.SetTranslationValue(chunkDataTranslation[ent].Value, serializerState);
    }
}
