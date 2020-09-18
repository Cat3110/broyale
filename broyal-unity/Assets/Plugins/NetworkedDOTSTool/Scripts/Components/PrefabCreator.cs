using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct NameId
{
    [ReadOnly] public uint Id;

    public static NameId Empty => new NameId {Id = uint.MaxValue};
}

[GenerateAuthoringComponent]
public struct PrefabCreator : IComponentData
{
    public uint NameId;
}
