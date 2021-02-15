using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;
using Unity.Collections;

public struct CharacterSnapshotData : ISnapshotData<CharacterSnapshotData>
{
    public uint tick;
    private int AttackAttackType;
    private int AttackSeed;
    private int AttackAttackDirectionX;
    private int AttackAttackDirectionY;
    private int DamageDamageType;
    private int MovableCharacterComponentPlayerId;
    private int PlayerDatahealth;
    private int PlayerDataprimarySkillId;
    private int PlayerDatamaxHealth;
    private int PlayerDatapower;
    private int PlayerDatamagic;
    private int PlayerDatadamageRadius;
    private uint PlayerDatainventory;
    private int PlayerDataattackSkillId;
    private int PlayerDatadefenceSkillId;
    private int PlayerDatautilsSkillId;
    private float PlayerDataspeedMod;
    private uint PlayerDatastun;
    private uint PrefabCreatorNameId;
    private uint PrefabCreatorSkinId;
    private NativeString64 PrefabCreatorSkinSetting;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public int GetAttackAttackType(GhostDeserializerState deserializerState)
    {
        return (int)AttackAttackType;
    }
    public int GetAttackAttackType()
    {
        return (int)AttackAttackType;
    }
    public void SetAttackAttackType(int val, GhostSerializerState serializerState)
    {
        AttackAttackType = (int)val;
    }
    public void SetAttackAttackType(int val)
    {
        AttackAttackType = (int)val;
    }
    public int GetAttackSeed(GhostDeserializerState deserializerState)
    {
        return (int)AttackSeed;
    }
    public int GetAttackSeed()
    {
        return (int)AttackSeed;
    }
    public void SetAttackSeed(int val, GhostSerializerState serializerState)
    {
        AttackSeed = (int)val;
    }
    public void SetAttackSeed(int val)
    {
        AttackSeed = (int)val;
    }
    public float2 GetAttackAttackDirection(GhostDeserializerState deserializerState)
    {
        return GetAttackAttackDirection();
    }
    public float2 GetAttackAttackDirection()
    {
        return new float2(AttackAttackDirectionX * 0.01f, AttackAttackDirectionY * 0.01f);
    }
    public void SetAttackAttackDirection(float2 val, GhostSerializerState serializerState)
    {
        SetAttackAttackDirection(val);
    }
    public void SetAttackAttackDirection(float2 val)
    {
        AttackAttackDirectionX = (int)(val.x * 100);
        AttackAttackDirectionY = (int)(val.y * 100);
    }
    public int GetDamageDamageType(GhostDeserializerState deserializerState)
    {
        return (int)DamageDamageType;
    }
    public int GetDamageDamageType()
    {
        return (int)DamageDamageType;
    }
    public void SetDamageDamageType(int val, GhostSerializerState serializerState)
    {
        DamageDamageType = (int)val;
    }
    public void SetDamageDamageType(int val)
    {
        DamageDamageType = (int)val;
    }
    public int GetMovableCharacterComponentPlayerId(GhostDeserializerState deserializerState)
    {
        return (int)MovableCharacterComponentPlayerId;
    }
    public int GetMovableCharacterComponentPlayerId()
    {
        return (int)MovableCharacterComponentPlayerId;
    }
    public void SetMovableCharacterComponentPlayerId(int val, GhostSerializerState serializerState)
    {
        MovableCharacterComponentPlayerId = (int)val;
    }
    public void SetMovableCharacterComponentPlayerId(int val)
    {
        MovableCharacterComponentPlayerId = (int)val;
    }
    public int GetPlayerDatahealth(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatahealth;
    }
    public int GetPlayerDatahealth()
    {
        return (int)PlayerDatahealth;
    }
    public void SetPlayerDatahealth(int val, GhostSerializerState serializerState)
    {
        PlayerDatahealth = (int)val;
    }
    public void SetPlayerDatahealth(int val)
    {
        PlayerDatahealth = (int)val;
    }
    public int GetPlayerDataprimarySkillId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDataprimarySkillId;
    }
    public int GetPlayerDataprimarySkillId()
    {
        return (int)PlayerDataprimarySkillId;
    }
    public void SetPlayerDataprimarySkillId(int val, GhostSerializerState serializerState)
    {
        PlayerDataprimarySkillId = (int)val;
    }
    public void SetPlayerDataprimarySkillId(int val)
    {
        PlayerDataprimarySkillId = (int)val;
    }
    public int GetPlayerDatamaxHealth(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatamaxHealth;
    }
    public int GetPlayerDatamaxHealth()
    {
        return (int)PlayerDatamaxHealth;
    }
    public void SetPlayerDatamaxHealth(int val, GhostSerializerState serializerState)
    {
        PlayerDatamaxHealth = (int)val;
    }
    public void SetPlayerDatamaxHealth(int val)
    {
        PlayerDatamaxHealth = (int)val;
    }
    public int GetPlayerDatapower(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatapower;
    }
    public int GetPlayerDatapower()
    {
        return (int)PlayerDatapower;
    }
    public void SetPlayerDatapower(int val, GhostSerializerState serializerState)
    {
        PlayerDatapower = (int)val;
    }
    public void SetPlayerDatapower(int val)
    {
        PlayerDatapower = (int)val;
    }
    public int GetPlayerDatamagic(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatamagic;
    }
    public int GetPlayerDatamagic()
    {
        return (int)PlayerDatamagic;
    }
    public void SetPlayerDatamagic(int val, GhostSerializerState serializerState)
    {
        PlayerDatamagic = (int)val;
    }
    public void SetPlayerDatamagic(int val)
    {
        PlayerDatamagic = (int)val;
    }
    public int GetPlayerDatadamageRadius(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatadamageRadius;
    }
    public int GetPlayerDatadamageRadius()
    {
        return (int)PlayerDatadamageRadius;
    }
    public void SetPlayerDatadamageRadius(int val, GhostSerializerState serializerState)
    {
        PlayerDatadamageRadius = (int)val;
    }
    public void SetPlayerDatadamageRadius(int val)
    {
        PlayerDatadamageRadius = (int)val;
    }
    public uint GetPlayerDatainventory(GhostDeserializerState deserializerState)
    {
        return (uint)PlayerDatainventory;
    }
    public uint GetPlayerDatainventory()
    {
        return (uint)PlayerDatainventory;
    }
    public void SetPlayerDatainventory(uint val, GhostSerializerState serializerState)
    {
        PlayerDatainventory = (uint)val;
    }
    public void SetPlayerDatainventory(uint val)
    {
        PlayerDatainventory = (uint)val;
    }
    public int GetPlayerDataattackSkillId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDataattackSkillId;
    }
    public int GetPlayerDataattackSkillId()
    {
        return (int)PlayerDataattackSkillId;
    }
    public void SetPlayerDataattackSkillId(int val, GhostSerializerState serializerState)
    {
        PlayerDataattackSkillId = (int)val;
    }
    public void SetPlayerDataattackSkillId(int val)
    {
        PlayerDataattackSkillId = (int)val;
    }
    public int GetPlayerDatadefenceSkillId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatadefenceSkillId;
    }
    public int GetPlayerDatadefenceSkillId()
    {
        return (int)PlayerDatadefenceSkillId;
    }
    public void SetPlayerDatadefenceSkillId(int val, GhostSerializerState serializerState)
    {
        PlayerDatadefenceSkillId = (int)val;
    }
    public void SetPlayerDatadefenceSkillId(int val)
    {
        PlayerDatadefenceSkillId = (int)val;
    }
    public int GetPlayerDatautilsSkillId(GhostDeserializerState deserializerState)
    {
        return (int)PlayerDatautilsSkillId;
    }
    public int GetPlayerDatautilsSkillId()
    {
        return (int)PlayerDatautilsSkillId;
    }
    public void SetPlayerDatautilsSkillId(int val, GhostSerializerState serializerState)
    {
        PlayerDatautilsSkillId = (int)val;
    }
    public void SetPlayerDatautilsSkillId(int val)
    {
        PlayerDatautilsSkillId = (int)val;
    }
    public float GetPlayerDataspeedMod(GhostDeserializerState deserializerState)
    {
        return PlayerDataspeedMod;
    }
    public float GetPlayerDataspeedMod()
    {
        return PlayerDataspeedMod;
    }
    public void SetPlayerDataspeedMod(float val, GhostSerializerState serializerState)
    {
        PlayerDataspeedMod = val;
    }
    public void SetPlayerDataspeedMod(float val)
    {
        PlayerDataspeedMod = val;
    }
    public bool GetPlayerDatastun(GhostDeserializerState deserializerState)
    {
        return PlayerDatastun!=0;
    }
    public bool GetPlayerDatastun()
    {
        return PlayerDatastun!=0;
    }
    public void SetPlayerDatastun(bool val, GhostSerializerState serializerState)
    {
        PlayerDatastun = val?1u:0;
    }
    public void SetPlayerDatastun(bool val)
    {
        PlayerDatastun = val?1u:0;
    }
    public uint GetPrefabCreatorNameId(GhostDeserializerState deserializerState)
    {
        return (uint)PrefabCreatorNameId;
    }
    public uint GetPrefabCreatorNameId()
    {
        return (uint)PrefabCreatorNameId;
    }
    public void SetPrefabCreatorNameId(uint val, GhostSerializerState serializerState)
    {
        PrefabCreatorNameId = (uint)val;
    }
    public void SetPrefabCreatorNameId(uint val)
    {
        PrefabCreatorNameId = (uint)val;
    }
    public uint GetPrefabCreatorSkinId(GhostDeserializerState deserializerState)
    {
        return (uint)PrefabCreatorSkinId;
    }
    public uint GetPrefabCreatorSkinId()
    {
        return (uint)PrefabCreatorSkinId;
    }
    public void SetPrefabCreatorSkinId(uint val, GhostSerializerState serializerState)
    {
        PrefabCreatorSkinId = (uint)val;
    }
    public void SetPrefabCreatorSkinId(uint val)
    {
        PrefabCreatorSkinId = (uint)val;
    }
    public NativeString64 GetPrefabCreatorSkinSetting(GhostDeserializerState deserializerState)
    {
        return PrefabCreatorSkinSetting;
    }
    public NativeString64 GetPrefabCreatorSkinSetting()
    {
        return PrefabCreatorSkinSetting;
    }
    public void SetPrefabCreatorSkinSetting(NativeString64 val, GhostSerializerState serializerState)
    {
        PrefabCreatorSkinSetting = val;
    }
    public void SetPrefabCreatorSkinSetting(NativeString64 val)
    {
        PrefabCreatorSkinSetting = val;
    }
    public quaternion GetRotationValue(GhostDeserializerState deserializerState)
    {
        return GetRotationValue();
    }
    public quaternion GetRotationValue()
    {
        return new quaternion(RotationValueX * 0.001f, RotationValueY * 0.001f, RotationValueZ * 0.001f, RotationValueW * 0.001f);
    }
    public void SetRotationValue(quaternion q, GhostSerializerState serializerState)
    {
        SetRotationValue(q);
    }
    public void SetRotationValue(quaternion q)
    {
        RotationValueX = (int)(q.value.x * 1000);
        RotationValueY = (int)(q.value.y * 1000);
        RotationValueZ = (int)(q.value.z * 1000);
        RotationValueW = (int)(q.value.w * 1000);
    }
    public float3 GetTranslationValue(GhostDeserializerState deserializerState)
    {
        return GetTranslationValue();
    }
    public float3 GetTranslationValue()
    {
        return new float3(TranslationValueX * 0.0001f, TranslationValueY * 0.0001f, TranslationValueZ * 0.0001f);
    }
    public void SetTranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetTranslationValue(val);
    }
    public void SetTranslationValue(float3 val)
    {
        TranslationValueX = (int)(val.x * 10000);
        TranslationValueY = (int)(val.y * 10000);
        TranslationValueZ = (int)(val.z * 10000);
    }

    public void PredictDelta(uint tick, ref CharacterSnapshotData baseline1, ref CharacterSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        AttackAttackType = predictor.PredictInt(AttackAttackType, baseline1.AttackAttackType, baseline2.AttackAttackType);
        AttackSeed = predictor.PredictInt(AttackSeed, baseline1.AttackSeed, baseline2.AttackSeed);
        AttackAttackDirectionX = predictor.PredictInt(AttackAttackDirectionX, baseline1.AttackAttackDirectionX, baseline2.AttackAttackDirectionX);
        AttackAttackDirectionY = predictor.PredictInt(AttackAttackDirectionY, baseline1.AttackAttackDirectionY, baseline2.AttackAttackDirectionY);
        DamageDamageType = predictor.PredictInt(DamageDamageType, baseline1.DamageDamageType, baseline2.DamageDamageType);
        MovableCharacterComponentPlayerId = predictor.PredictInt(MovableCharacterComponentPlayerId, baseline1.MovableCharacterComponentPlayerId, baseline2.MovableCharacterComponentPlayerId);
        PlayerDatahealth = predictor.PredictInt(PlayerDatahealth, baseline1.PlayerDatahealth, baseline2.PlayerDatahealth);
        PlayerDataprimarySkillId = predictor.PredictInt(PlayerDataprimarySkillId, baseline1.PlayerDataprimarySkillId, baseline2.PlayerDataprimarySkillId);
        PlayerDatamaxHealth = predictor.PredictInt(PlayerDatamaxHealth, baseline1.PlayerDatamaxHealth, baseline2.PlayerDatamaxHealth);
        PlayerDatapower = predictor.PredictInt(PlayerDatapower, baseline1.PlayerDatapower, baseline2.PlayerDatapower);
        PlayerDatamagic = predictor.PredictInt(PlayerDatamagic, baseline1.PlayerDatamagic, baseline2.PlayerDatamagic);
        PlayerDatadamageRadius = predictor.PredictInt(PlayerDatadamageRadius, baseline1.PlayerDatadamageRadius, baseline2.PlayerDatadamageRadius);
        PlayerDatainventory = (uint)predictor.PredictInt((int)PlayerDatainventory, (int)baseline1.PlayerDatainventory, (int)baseline2.PlayerDatainventory);
        PlayerDataattackSkillId = predictor.PredictInt(PlayerDataattackSkillId, baseline1.PlayerDataattackSkillId, baseline2.PlayerDataattackSkillId);
        PlayerDatadefenceSkillId = predictor.PredictInt(PlayerDatadefenceSkillId, baseline1.PlayerDatadefenceSkillId, baseline2.PlayerDatadefenceSkillId);
        PlayerDatautilsSkillId = predictor.PredictInt(PlayerDatautilsSkillId, baseline1.PlayerDatautilsSkillId, baseline2.PlayerDatautilsSkillId);
        PlayerDatastun = (uint)predictor.PredictInt((int)PlayerDatastun, (int)baseline1.PlayerDatastun, (int)baseline2.PlayerDatastun);
        PrefabCreatorNameId = (uint)predictor.PredictInt((int)PrefabCreatorNameId, (int)baseline1.PrefabCreatorNameId, (int)baseline2.PrefabCreatorNameId);
        PrefabCreatorSkinId = (uint)predictor.PredictInt((int)PrefabCreatorSkinId, (int)baseline1.PrefabCreatorSkinId, (int)baseline2.PrefabCreatorSkinId);
        RotationValueX = predictor.PredictInt(RotationValueX, baseline1.RotationValueX, baseline2.RotationValueX);
        RotationValueY = predictor.PredictInt(RotationValueY, baseline1.RotationValueY, baseline2.RotationValueY);
        RotationValueZ = predictor.PredictInt(RotationValueZ, baseline1.RotationValueZ, baseline2.RotationValueZ);
        RotationValueW = predictor.PredictInt(RotationValueW, baseline1.RotationValueW, baseline2.RotationValueW);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref CharacterSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (AttackAttackType != baseline.AttackAttackType) ? 1u : 0;
        changeMask0 |= (AttackSeed != baseline.AttackSeed) ? (1u<<1) : 0;
        changeMask0 |= (AttackAttackDirectionX != baseline.AttackAttackDirectionX ||
                                           AttackAttackDirectionY != baseline.AttackAttackDirectionY) ? (1u<<2) : 0;
        changeMask0 |= (DamageDamageType != baseline.DamageDamageType) ? (1u<<3) : 0;
        changeMask0 |= (MovableCharacterComponentPlayerId != baseline.MovableCharacterComponentPlayerId) ? (1u<<4) : 0;
        changeMask0 |= (PlayerDatahealth != baseline.PlayerDatahealth) ? (1u<<5) : 0;
        changeMask0 |= (PlayerDataprimarySkillId != baseline.PlayerDataprimarySkillId) ? (1u<<6) : 0;
        changeMask0 |= (PlayerDatamaxHealth != baseline.PlayerDatamaxHealth) ? (1u<<7) : 0;
        changeMask0 |= (PlayerDatapower != baseline.PlayerDatapower) ? (1u<<8) : 0;
        changeMask0 |= (PlayerDatamagic != baseline.PlayerDatamagic) ? (1u<<9) : 0;
        changeMask0 |= (PlayerDatadamageRadius != baseline.PlayerDatadamageRadius) ? (1u<<10) : 0;
        changeMask0 |= (PlayerDatainventory != baseline.PlayerDatainventory) ? (1u<<11) : 0;
        changeMask0 |= (PlayerDataattackSkillId != baseline.PlayerDataattackSkillId) ? (1u<<12) : 0;
        changeMask0 |= (PlayerDatadefenceSkillId != baseline.PlayerDatadefenceSkillId) ? (1u<<13) : 0;
        changeMask0 |= (PlayerDatautilsSkillId != baseline.PlayerDatautilsSkillId) ? (1u<<14) : 0;
        changeMask0 |= (PlayerDataspeedMod != baseline.PlayerDataspeedMod) ? (1u<<15) : 0;
        changeMask0 |= (PlayerDatastun != baseline.PlayerDatastun) ? (1u<<16) : 0;
        changeMask0 |= (PrefabCreatorNameId != baseline.PrefabCreatorNameId) ? (1u<<17) : 0;
        changeMask0 |= (PrefabCreatorSkinId != baseline.PrefabCreatorSkinId) ? (1u<<18) : 0;
        changeMask0 |= PrefabCreatorSkinSetting.Equals(baseline.PrefabCreatorSkinSetting) ? 0 : (1u<<19);
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<20) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<21) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(AttackAttackType, baseline.AttackAttackType, compressionModel);
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(AttackSeed, baseline.AttackSeed, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
        {
            writer.WritePackedIntDelta(AttackAttackDirectionX, baseline.AttackAttackDirectionX, compressionModel);
            writer.WritePackedIntDelta(AttackAttackDirectionY, baseline.AttackAttackDirectionY, compressionModel);
        }
        if ((changeMask0 & (1 << 3)) != 0)
            writer.WritePackedIntDelta(DamageDamageType, baseline.DamageDamageType, compressionModel);
        if ((changeMask0 & (1 << 4)) != 0)
            writer.WritePackedIntDelta(MovableCharacterComponentPlayerId, baseline.MovableCharacterComponentPlayerId, compressionModel);
        if ((changeMask0 & (1 << 5)) != 0)
            writer.WritePackedIntDelta(PlayerDatahealth, baseline.PlayerDatahealth, compressionModel);
        if ((changeMask0 & (1 << 6)) != 0)
            writer.WritePackedIntDelta(PlayerDataprimarySkillId, baseline.PlayerDataprimarySkillId, compressionModel);
        if ((changeMask0 & (1 << 7)) != 0)
            writer.WritePackedIntDelta(PlayerDatamaxHealth, baseline.PlayerDatamaxHealth, compressionModel);
        if ((changeMask0 & (1 << 8)) != 0)
            writer.WritePackedIntDelta(PlayerDatapower, baseline.PlayerDatapower, compressionModel);
        if ((changeMask0 & (1 << 9)) != 0)
            writer.WritePackedIntDelta(PlayerDatamagic, baseline.PlayerDatamagic, compressionModel);
        if ((changeMask0 & (1 << 10)) != 0)
            writer.WritePackedIntDelta(PlayerDatadamageRadius, baseline.PlayerDatadamageRadius, compressionModel);
        if ((changeMask0 & (1 << 11)) != 0)
            writer.WritePackedUIntDelta(PlayerDatainventory, baseline.PlayerDatainventory, compressionModel);
        if ((changeMask0 & (1 << 12)) != 0)
            writer.WritePackedIntDelta(PlayerDataattackSkillId, baseline.PlayerDataattackSkillId, compressionModel);
        if ((changeMask0 & (1 << 13)) != 0)
            writer.WritePackedIntDelta(PlayerDatadefenceSkillId, baseline.PlayerDatadefenceSkillId, compressionModel);
        if ((changeMask0 & (1 << 14)) != 0)
            writer.WritePackedIntDelta(PlayerDatautilsSkillId, baseline.PlayerDatautilsSkillId, compressionModel);
        if ((changeMask0 & (1 << 15)) != 0)
            writer.WritePackedFloatDelta(PlayerDataspeedMod, baseline.PlayerDataspeedMod, compressionModel);
        if ((changeMask0 & (1 << 16)) != 0)
            writer.WritePackedUIntDelta(PlayerDatastun, baseline.PlayerDatastun, compressionModel);
        if ((changeMask0 & (1 << 17)) != 0)
            writer.WritePackedUIntDelta(PrefabCreatorNameId, baseline.PrefabCreatorNameId, compressionModel);
        if ((changeMask0 & (1 << 18)) != 0)
            writer.WritePackedUIntDelta(PrefabCreatorSkinId, baseline.PrefabCreatorSkinId, compressionModel);
        if ((changeMask0 & (1 << 19)) != 0)
            writer.WritePackedStringDelta(PrefabCreatorSkinSetting, baseline.PrefabCreatorSkinSetting, compressionModel);
        if ((changeMask0 & (1 << 20)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 21)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref CharacterSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            AttackAttackType = reader.ReadPackedIntDelta(baseline.AttackAttackType, compressionModel);
        else
            AttackAttackType = baseline.AttackAttackType;
        if ((changeMask0 & (1 << 1)) != 0)
            AttackSeed = reader.ReadPackedIntDelta(baseline.AttackSeed, compressionModel);
        else
            AttackSeed = baseline.AttackSeed;
        if ((changeMask0 & (1 << 2)) != 0)
        {
            AttackAttackDirectionX = reader.ReadPackedIntDelta(baseline.AttackAttackDirectionX, compressionModel);
            AttackAttackDirectionY = reader.ReadPackedIntDelta(baseline.AttackAttackDirectionY, compressionModel);
        }
        else
        {
            AttackAttackDirectionX = baseline.AttackAttackDirectionX;
            AttackAttackDirectionY = baseline.AttackAttackDirectionY;
        }
        if ((changeMask0 & (1 << 3)) != 0)
            DamageDamageType = reader.ReadPackedIntDelta(baseline.DamageDamageType, compressionModel);
        else
            DamageDamageType = baseline.DamageDamageType;
        if ((changeMask0 & (1 << 4)) != 0)
            MovableCharacterComponentPlayerId = reader.ReadPackedIntDelta(baseline.MovableCharacterComponentPlayerId, compressionModel);
        else
            MovableCharacterComponentPlayerId = baseline.MovableCharacterComponentPlayerId;
        if ((changeMask0 & (1 << 5)) != 0)
            PlayerDatahealth = reader.ReadPackedIntDelta(baseline.PlayerDatahealth, compressionModel);
        else
            PlayerDatahealth = baseline.PlayerDatahealth;
        if ((changeMask0 & (1 << 6)) != 0)
            PlayerDataprimarySkillId = reader.ReadPackedIntDelta(baseline.PlayerDataprimarySkillId, compressionModel);
        else
            PlayerDataprimarySkillId = baseline.PlayerDataprimarySkillId;
        if ((changeMask0 & (1 << 7)) != 0)
            PlayerDatamaxHealth = reader.ReadPackedIntDelta(baseline.PlayerDatamaxHealth, compressionModel);
        else
            PlayerDatamaxHealth = baseline.PlayerDatamaxHealth;
        if ((changeMask0 & (1 << 8)) != 0)
            PlayerDatapower = reader.ReadPackedIntDelta(baseline.PlayerDatapower, compressionModel);
        else
            PlayerDatapower = baseline.PlayerDatapower;
        if ((changeMask0 & (1 << 9)) != 0)
            PlayerDatamagic = reader.ReadPackedIntDelta(baseline.PlayerDatamagic, compressionModel);
        else
            PlayerDatamagic = baseline.PlayerDatamagic;
        if ((changeMask0 & (1 << 10)) != 0)
            PlayerDatadamageRadius = reader.ReadPackedIntDelta(baseline.PlayerDatadamageRadius, compressionModel);
        else
            PlayerDatadamageRadius = baseline.PlayerDatadamageRadius;
        if ((changeMask0 & (1 << 11)) != 0)
            PlayerDatainventory = reader.ReadPackedUIntDelta(baseline.PlayerDatainventory, compressionModel);
        else
            PlayerDatainventory = baseline.PlayerDatainventory;
        if ((changeMask0 & (1 << 12)) != 0)
            PlayerDataattackSkillId = reader.ReadPackedIntDelta(baseline.PlayerDataattackSkillId, compressionModel);
        else
            PlayerDataattackSkillId = baseline.PlayerDataattackSkillId;
        if ((changeMask0 & (1 << 13)) != 0)
            PlayerDatadefenceSkillId = reader.ReadPackedIntDelta(baseline.PlayerDatadefenceSkillId, compressionModel);
        else
            PlayerDatadefenceSkillId = baseline.PlayerDatadefenceSkillId;
        if ((changeMask0 & (1 << 14)) != 0)
            PlayerDatautilsSkillId = reader.ReadPackedIntDelta(baseline.PlayerDatautilsSkillId, compressionModel);
        else
            PlayerDatautilsSkillId = baseline.PlayerDatautilsSkillId;
        if ((changeMask0 & (1 << 15)) != 0)
            PlayerDataspeedMod = reader.ReadPackedFloatDelta(baseline.PlayerDataspeedMod, compressionModel);
        else
            PlayerDataspeedMod = baseline.PlayerDataspeedMod;
        if ((changeMask0 & (1 << 16)) != 0)
            PlayerDatastun = reader.ReadPackedUIntDelta(baseline.PlayerDatastun, compressionModel);
        else
            PlayerDatastun = baseline.PlayerDatastun;
        if ((changeMask0 & (1 << 17)) != 0)
            PrefabCreatorNameId = reader.ReadPackedUIntDelta(baseline.PrefabCreatorNameId, compressionModel);
        else
            PrefabCreatorNameId = baseline.PrefabCreatorNameId;
        if ((changeMask0 & (1 << 18)) != 0)
            PrefabCreatorSkinId = reader.ReadPackedUIntDelta(baseline.PrefabCreatorSkinId, compressionModel);
        else
            PrefabCreatorSkinId = baseline.PrefabCreatorSkinId;
        if ((changeMask0 & (1 << 19)) != 0)
            PrefabCreatorSkinSetting = reader.ReadPackedStringDelta(baseline.PrefabCreatorSkinSetting, compressionModel);
        else
            PrefabCreatorSkinSetting = baseline.PrefabCreatorSkinSetting;
        if ((changeMask0 & (1 << 20)) != 0)
        {
            RotationValueX = reader.ReadPackedIntDelta(baseline.RotationValueX, compressionModel);
            RotationValueY = reader.ReadPackedIntDelta(baseline.RotationValueY, compressionModel);
            RotationValueZ = reader.ReadPackedIntDelta(baseline.RotationValueZ, compressionModel);
            RotationValueW = reader.ReadPackedIntDelta(baseline.RotationValueW, compressionModel);
        }
        else
        {
            RotationValueX = baseline.RotationValueX;
            RotationValueY = baseline.RotationValueY;
            RotationValueZ = baseline.RotationValueZ;
            RotationValueW = baseline.RotationValueW;
        }
        if ((changeMask0 & (1 << 21)) != 0)
        {
            TranslationValueX = reader.ReadPackedIntDelta(baseline.TranslationValueX, compressionModel);
            TranslationValueY = reader.ReadPackedIntDelta(baseline.TranslationValueY, compressionModel);
            TranslationValueZ = reader.ReadPackedIntDelta(baseline.TranslationValueZ, compressionModel);
        }
        else
        {
            TranslationValueX = baseline.TranslationValueX;
            TranslationValueY = baseline.TranslationValueY;
            TranslationValueZ = baseline.TranslationValueZ;
        }
    }
    public void Interpolate(ref CharacterSnapshotData target, float factor)
    {
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
