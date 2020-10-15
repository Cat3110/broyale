using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct Attack : IComponentData
{
    public float3 Type;
    
    public uint PredictTick;

    public float TransHash;

    public int AttackType;
    public float Duration;
    public int Seed;
    
    public bool NeedApplyDamage;
    
    public int ProccesedId;

    //public float3 PredTrans;
    
    public float2 AttackDirection;

    public float DamageTime;

    public Entity Target;

    public int BackAttackType;
}

public struct DEAD : IComponentData
{
    public uint Reasone;
}

public static class ExtensionComponent
{
    public static bool HaveTransition(this Attack attack) => math.abs(attack.Duration) > 0.1f;
    public static bool HaveTransition(this Damage damage) => math.abs(damage.Type.x) > 0.1f;
}
