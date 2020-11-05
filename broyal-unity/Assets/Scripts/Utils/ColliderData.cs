using UnityEngine;

public enum ColliderType
{
    Box,
    Sphere
}
public struct ColliderData
{
    //public string Name;
    public Vector3 Position;
    //public Vector3 Center;
    public Vector3 Size;
    public ColliderType Type;
    public Vector2 Min;
    public Vector2 Max;
}