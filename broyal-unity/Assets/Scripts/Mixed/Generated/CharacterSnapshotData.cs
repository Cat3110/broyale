using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct CharacterSnapshotData : ISnapshotData<CharacterSnapshotData>
{
    public uint tick;
    private int AttackTypeX;
    private int AttackTypeY;
    private int AttackTypeZ;
    private int DamageTypeX;
    private int DamageTypeY;
    private int DamageTypeZ;
    private int MovableCharacterComponentPlayerId;
    private int PlayerDatahealth;
    private int PlayerDataprimarySkillId;
    private int RotationValueX;
    private int RotationValueY;
    private int RotationValueZ;
    private int RotationValueW;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
    public float3 GetAttackType(GhostDeserializerState deserializerState)
    {
        return GetAttackType();
    }
    public float3 GetAttackType()
    {
        return new float3(AttackTypeX * 0.01f, AttackTypeY * 0.01f, AttackTypeZ * 0.01f);
    }
    public void SetAttackType(float3 val, GhostSerializerState serializerState)
    {
        SetAttackType(val);
    }
    public void SetAttackType(float3 val)
    {
        AttackTypeX = (int)(val.x * 100);
        AttackTypeY = (int)(val.y * 100);
        AttackTypeZ = (int)(val.z * 100);
    }
    public float3 GetDamageType(GhostDeserializerState deserializerState)
    {
        return GetDamageType();
    }
    public float3 GetDamageType()
    {
        return new float3(DamageTypeX * 0.01f, DamageTypeY * 0.01f, DamageTypeZ * 0.01f);
    }
    public void SetDamageType(float3 val, GhostSerializerState serializerState)
    {
        SetDamageType(val);
    }
    public void SetDamageType(float3 val)
    {
        DamageTypeX = (int)(val.x * 100);
        DamageTypeY = (int)(val.y * 100);
        DamageTypeZ = (int)(val.z * 100);
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
        return new float3(TranslationValueX * 0.01f, TranslationValueY * 0.01f, TranslationValueZ * 0.01f);
    }
    public void SetTranslationValue(float3 val, GhostSerializerState serializerState)
    {
        SetTranslationValue(val);
    }
    public void SetTranslationValue(float3 val)
    {
        TranslationValueX = (int)(val.x * 100);
        TranslationValueY = (int)(val.y * 100);
        TranslationValueZ = (int)(val.z * 100);
    }

    public void PredictDelta(uint tick, ref CharacterSnapshotData baseline1, ref CharacterSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        AttackTypeX = predictor.PredictInt(AttackTypeX, baseline1.AttackTypeX, baseline2.AttackTypeX);
        AttackTypeY = predictor.PredictInt(AttackTypeY, baseline1.AttackTypeY, baseline2.AttackTypeY);
        AttackTypeZ = predictor.PredictInt(AttackTypeZ, baseline1.AttackTypeZ, baseline2.AttackTypeZ);
        DamageTypeX = predictor.PredictInt(DamageTypeX, baseline1.DamageTypeX, baseline2.DamageTypeX);
        DamageTypeY = predictor.PredictInt(DamageTypeY, baseline1.DamageTypeY, baseline2.DamageTypeY);
        DamageTypeZ = predictor.PredictInt(DamageTypeZ, baseline1.DamageTypeZ, baseline2.DamageTypeZ);
        MovableCharacterComponentPlayerId = predictor.PredictInt(MovableCharacterComponentPlayerId, baseline1.MovableCharacterComponentPlayerId, baseline2.MovableCharacterComponentPlayerId);
        PlayerDatahealth = predictor.PredictInt(PlayerDatahealth, baseline1.PlayerDatahealth, baseline2.PlayerDatahealth);
        PlayerDataprimarySkillId = predictor.PredictInt(PlayerDataprimarySkillId, baseline1.PlayerDataprimarySkillId, baseline2.PlayerDataprimarySkillId);
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
        changeMask0 = (AttackTypeX != baseline.AttackTypeX ||
                                          AttackTypeY != baseline.AttackTypeY ||
                                          AttackTypeZ != baseline.AttackTypeZ) ? 1u : 0;
        changeMask0 |= (DamageTypeX != baseline.DamageTypeX ||
                                           DamageTypeY != baseline.DamageTypeY ||
                                           DamageTypeZ != baseline.DamageTypeZ) ? (1u<<1) : 0;
        changeMask0 |= (MovableCharacterComponentPlayerId != baseline.MovableCharacterComponentPlayerId) ? (1u<<2) : 0;
        changeMask0 |= (PlayerDatahealth != baseline.PlayerDatahealth) ? (1u<<3) : 0;
        changeMask0 |= (PlayerDataprimarySkillId != baseline.PlayerDataprimarySkillId) ? (1u<<4) : 0;
        changeMask0 |= (RotationValueX != baseline.RotationValueX ||
                                           RotationValueY != baseline.RotationValueY ||
                                           RotationValueZ != baseline.RotationValueZ ||
                                           RotationValueW != baseline.RotationValueW) ? (1u<<5) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<6) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        bool isPredicted = GetMovableCharacterComponentPlayerId() == networkId;
        writer.WritePackedUInt(isPredicted?1u:0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
        {
            writer.WritePackedIntDelta(AttackTypeX, baseline.AttackTypeX, compressionModel);
            writer.WritePackedIntDelta(AttackTypeY, baseline.AttackTypeY, compressionModel);
            writer.WritePackedIntDelta(AttackTypeZ, baseline.AttackTypeZ, compressionModel);
        }
        if ((changeMask0 & (1 << 1)) != 0)
        {
            writer.WritePackedIntDelta(DamageTypeX, baseline.DamageTypeX, compressionModel);
            writer.WritePackedIntDelta(DamageTypeY, baseline.DamageTypeY, compressionModel);
            writer.WritePackedIntDelta(DamageTypeZ, baseline.DamageTypeZ, compressionModel);
        }
        if ((changeMask0 & (1 << 3)) != 0)
            writer.WritePackedIntDelta(PlayerDatahealth, baseline.PlayerDatahealth, compressionModel);
        if ((changeMask0 & (1 << 4)) != 0)
            writer.WritePackedIntDelta(PlayerDataprimarySkillId, baseline.PlayerDataprimarySkillId, compressionModel);
        if ((changeMask0 & (1 << 5)) != 0)
        {
            writer.WritePackedIntDelta(RotationValueX, baseline.RotationValueX, compressionModel);
            writer.WritePackedIntDelta(RotationValueY, baseline.RotationValueY, compressionModel);
            writer.WritePackedIntDelta(RotationValueZ, baseline.RotationValueZ, compressionModel);
            writer.WritePackedIntDelta(RotationValueW, baseline.RotationValueW, compressionModel);
        }
        if ((changeMask0 & (1 << 6)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
        if (isPredicted)
        {
            if ((changeMask0 & (1 << 2)) != 0)
                writer.WritePackedIntDelta(MovableCharacterComponentPlayerId, baseline.MovableCharacterComponentPlayerId, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref CharacterSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        bool isPredicted = reader.ReadPackedUInt(compressionModel)!=0;
        if ((changeMask0 & (1 << 0)) != 0)
        {
            AttackTypeX = reader.ReadPackedIntDelta(baseline.AttackTypeX, compressionModel);
            AttackTypeY = reader.ReadPackedIntDelta(baseline.AttackTypeY, compressionModel);
            AttackTypeZ = reader.ReadPackedIntDelta(baseline.AttackTypeZ, compressionModel);
        }
        else
        {
            AttackTypeX = baseline.AttackTypeX;
            AttackTypeY = baseline.AttackTypeY;
            AttackTypeZ = baseline.AttackTypeZ;
        }
        if ((changeMask0 & (1 << 1)) != 0)
        {
            DamageTypeX = reader.ReadPackedIntDelta(baseline.DamageTypeX, compressionModel);
            DamageTypeY = reader.ReadPackedIntDelta(baseline.DamageTypeY, compressionModel);
            DamageTypeZ = reader.ReadPackedIntDelta(baseline.DamageTypeZ, compressionModel);
        }
        else
        {
            DamageTypeX = baseline.DamageTypeX;
            DamageTypeY = baseline.DamageTypeY;
            DamageTypeZ = baseline.DamageTypeZ;
        }
        if ((changeMask0 & (1 << 3)) != 0)
            PlayerDatahealth = reader.ReadPackedIntDelta(baseline.PlayerDatahealth, compressionModel);
        else
            PlayerDatahealth = baseline.PlayerDatahealth;
        if ((changeMask0 & (1 << 4)) != 0)
            PlayerDataprimarySkillId = reader.ReadPackedIntDelta(baseline.PlayerDataprimarySkillId, compressionModel);
        else
            PlayerDataprimarySkillId = baseline.PlayerDataprimarySkillId;
        if ((changeMask0 & (1 << 5)) != 0)
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
        if ((changeMask0 & (1 << 6)) != 0)
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
        if (isPredicted)
        {
            if ((changeMask0 & (1 << 2)) != 0)
                MovableCharacterComponentPlayerId = reader.ReadPackedIntDelta(baseline.MovableCharacterComponentPlayerId, compressionModel);
            else
                MovableCharacterComponentPlayerId = baseline.MovableCharacterComponentPlayerId;
        }
    }
    public void Interpolate(ref CharacterSnapshotData target, float factor)
    {
        SetRotationValue(math.slerp(GetRotationValue(), target.GetRotationValue(), factor));
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
