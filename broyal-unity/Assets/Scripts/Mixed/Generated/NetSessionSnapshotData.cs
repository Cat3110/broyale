using Unity.Networking.Transport;
using Unity.NetCode;
using Unity.Mathematics;

public struct NetSessionSnapshotData : ISnapshotData<NetSessionSnapshotData>
{
    public uint tick;
    private int NetSessionTimeToStart;
    uint changeMask0;

    public uint Tick => tick;
    public float GetNetSessionTimeToStart(GhostDeserializerState deserializerState)
    {
        return NetSessionTimeToStart * 0.01f;
    }
    public float GetNetSessionTimeToStart()
    {
        return NetSessionTimeToStart * 0.01f;
    }
    public void SetNetSessionTimeToStart(float val, GhostSerializerState serializerState)
    {
        NetSessionTimeToStart = (int)(val * 100);
    }
    public void SetNetSessionTimeToStart(float val)
    {
        NetSessionTimeToStart = (int)(val * 100);
    }

    public void PredictDelta(uint tick, ref NetSessionSnapshotData baseline1, ref NetSessionSnapshotData baseline2)
    {
        var predictor = new GhostDeltaPredictor(tick, this.tick, baseline1.tick, baseline2.tick);
        NetSessionTimeToStart = predictor.PredictInt(NetSessionTimeToStart, baseline1.NetSessionTimeToStart, baseline2.NetSessionTimeToStart);
    }

    public void Serialize(int networkId, ref NetSessionSnapshotData baseline, ref DataStreamWriter writer, NetworkCompressionModel compressionModel)
    {
        changeMask0 = (NetSessionTimeToStart != baseline.NetSessionTimeToStart) ? 1u : 0;
        writer.WritePackedUIntDelta(changeMask0, baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            writer.WritePackedIntDelta(NetSessionTimeToStart, baseline.NetSessionTimeToStart, compressionModel);
    }

    public void Deserialize(uint tick, ref NetSessionSnapshotData baseline, ref DataStreamReader reader,
        NetworkCompressionModel compressionModel)
    {
        this.tick = tick;
        changeMask0 = reader.ReadPackedUIntDelta(baseline.changeMask0, compressionModel);
        if ((changeMask0 & (1 << 0)) != 0)
            NetSessionTimeToStart = reader.ReadPackedIntDelta(baseline.NetSessionTimeToStart, compressionModel);
        else
            NetSessionTimeToStart = baseline.NetSessionTimeToStart;
    }
    public void Interpolate(ref NetSessionSnapshotData target, float factor)
    {
    }
}
