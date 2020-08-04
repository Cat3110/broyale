using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct MovableCharacterComponent : IComponentData
{
    [GhostDefaultField]
    public int PlayerId;
}
