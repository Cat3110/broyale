using System;
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
}