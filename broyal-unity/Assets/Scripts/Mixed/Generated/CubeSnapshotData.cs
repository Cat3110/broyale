using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct CubeSnapshotData : ISnapshotData<CubeSnapshotData>
{
    public uint tick;
    private int MovableCharacterComponentPlayerId;
    private int PlayerDatahealth;
    private int TranslationValueX;
    private int TranslationValueY;
    private int TranslationValueZ;
    uint changeMask0;

    public uint Tick => tick;
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

    public void PredictDelta(uint tick, ref CubeSnapshotData baseline1, ref CubeSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        MovableCharacterComponentPlayerId = predictor.PredictInt(MovableCharacterComponentPlayerId, baseline1.MovableCharacterComponentPlayerId, baseline2.MovableCharacterComponentPlayerId);
        PlayerDatahealth = predictor.PredictInt(PlayerDatahealth, baseline1.PlayerDatahealth, baseline2.PlayerDatahealth);
        TranslationValueX = predictor.PredictInt(TranslationValueX, baseline1.TranslationValueX, baseline2.TranslationValueX);
        TranslationValueY = predictor.PredictInt(TranslationValueY, baseline1.TranslationValueY, baseline2.TranslationValueY);
        TranslationValueZ = predictor.PredictInt(TranslationValueZ, baseline1.TranslationValueZ, baseline2.TranslationValueZ);
    }

    public void Serialize(int networkId, ref CubeSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (MovableCharacterComponentPlayerId != baseline.MovableCharacterComponentPlayerId) ? 1u : 0;
        changeMask0 |= (PlayerDatahealth != baseline.PlayerDatahealth) ? (1u<<1) : 0;
        changeMask0 |= (TranslationValueX != baseline.TranslationValueX ||
                                           TranslationValueY != baseline.TranslationValueY ||
                                           TranslationValueZ != baseline.TranslationValueZ) ? (1u<<2) : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(MovableCharacterComponentPlayerId, baseline.MovableCharacterComponentPlayerId, compressionModel);
        if ((changeMask0 & (1 << 1)) != 0)
            writer.WritePackedIntDelta(PlayerDatahealth, baseline.PlayerDatahealth, compressionModel);
        if ((changeMask0 & (1 << 2)) != 0)
        {
            writer.WritePackedIntDelta(TranslationValueX, baseline.TranslationValueX, compressionModel);
            writer.WritePackedIntDelta(TranslationValueY, baseline.TranslationValueY, compressionModel);
            writer.WritePackedIntDelta(TranslationValueZ, baseline.TranslationValueZ, compressionModel);
        }
    }

    public void Deserialize(uint tick, ref CubeSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            MovableCharacterComponentPlayerId = reader.ReadPackedIntDelta(baseline.MovableCharacterComponentPlayerId, compressionModel);
        else
            MovableCharacterComponentPlayerId = baseline.MovableCharacterComponentPlayerId;
        if ((changeMask0 & (1 << 1)) != 0)
            PlayerDatahealth = reader.ReadPackedIntDelta(baseline.PlayerDatahealth, compressionModel);
        else
            PlayerDatahealth = baseline.PlayerDatahealth;
        if ((changeMask0 & (1 << 2)) != 0)
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
    public void Interpolate(ref CubeSnapshotData target, float factor)
    {
        SetTranslationValue(math.lerp(GetTranslationValue(), target.GetTranslationValue(), factor));
    }
}
