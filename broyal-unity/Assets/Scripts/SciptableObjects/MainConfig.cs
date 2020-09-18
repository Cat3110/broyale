using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;


[Serializable]
public class NameIdToString
{
    public NameId Id;
    public string Name;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MainConfig", order = 1)]
public class MainConfig : ScriptableObject
{
    public string[] Names;
    public AssetReference[] AssetRefMembers;

    public NameId GetNameId(string id)
    {
        var index = Array.IndexOf(Names, id);
        return index < 0 ? NameId.Empty : new NameId { Id = (uint)index };
    }
}