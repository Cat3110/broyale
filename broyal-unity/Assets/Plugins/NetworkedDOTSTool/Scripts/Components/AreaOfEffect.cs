using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AreaOfEffect : IComponentData
{
   public float radius;
   public float angle;
   public float2 direction;
}
