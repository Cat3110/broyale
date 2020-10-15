using System;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[Serializable]
public struct NameId
{
    [ReadOnly] public uint Id;

    public static NameId Empty => new NameId {Id = uint.MaxValue};
}

[GenerateAuthoringComponent]
public struct PrefabCreator : IComponentData
{
    [GhostDefaultField]
    public uint NameId;
}

public struct PrefabCreatorComplite : IComponentData
{ }
