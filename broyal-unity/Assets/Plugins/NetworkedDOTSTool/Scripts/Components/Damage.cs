using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;


[GenerateAuthoringComponent]
public struct Damage : IComponentData
{
    public float3 Type;
    
    public uint PredictTick;
    
    public float Duration;
    
    public bool NeedApply;
    
    public float TransHash;

    public float Value;
    public int DamageType;
    public int Seed;
    public Entity Attacker;
}
