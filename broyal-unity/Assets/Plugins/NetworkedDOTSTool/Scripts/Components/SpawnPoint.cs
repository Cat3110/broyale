using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[GenerateAuthoringComponent]
public struct SpawnPoint : IComponentData {
    public float radius;
    public int spawnCount;
}