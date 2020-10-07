using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct NetSession : IComponentData {
    [GhostDefaultField(100,false)]
    public float TimeToStart;
}
