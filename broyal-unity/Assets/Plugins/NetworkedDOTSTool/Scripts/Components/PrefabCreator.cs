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
    
    public uint SkinId;
    public NativeString64 SkinSetting;
}

public struct PrefabCreatorComplite : IComponentData
{ }
