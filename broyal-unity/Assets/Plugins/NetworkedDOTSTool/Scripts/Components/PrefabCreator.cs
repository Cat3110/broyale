using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.AddressableAssets;


[Serializable]
public struct NameId
{
    [ReadOnly] public uint Id;
}

[GenerateAuthoringComponent]
public struct PrefabCreator : IComponentData
{
    public NameId NameId;
}
