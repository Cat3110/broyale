using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ItemSpawner : IComponentData
{
    public int itemId;
    public float duration;
    public Entity spawnedItem;
}
