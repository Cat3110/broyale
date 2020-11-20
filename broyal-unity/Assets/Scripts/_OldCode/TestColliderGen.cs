using System.Collections;
using System.Collections.Generic;
using Bootstrappers;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class TestColliderGen : MonoBehaviour
{
    public float angel = 90;
    public Vector3 rot  = Vector3.up;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.blue;
        var p1 = Vector3.zero;
        var p2 = Vector3.forward * 3;
        
        Gizmos.DrawLine(p1, p2);
        
        Gizmos.color = Color.green;
        
        Matrix4x4 matrix2 = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(angel, rot), Vector3.one);
        
        p1 = matrix2.MultiplyPoint(p1);
        p2 = matrix2.MultiplyPoint(p2);
        Gizmos.DrawLine(p1, p2);
        
       foreach (Collider collider in GetComponents<Collider>())
        {
            var colliderData = new ColliderData
            {
                //Name = collider.name,
                Position = collider.transform.position
            };
                
            switch (@collider)
            {
                case BoxCollider boxCollider:
                    colliderData.Type = ColliderType.Box;
                    colliderData.Position += boxCollider.center;

                    colliderData.Size = boxCollider.size;

                    var x1 = colliderData.Position.x - (colliderData.Size.x * 0.5f);
                    var x2 = colliderData.Position.x + (colliderData.Size.x * 0.5f);
                    var y1 = colliderData.Position.z - (colliderData.Size.z * 0.5f);
                    var y2 = colliderData.Position.z + (colliderData.Size.z * 0.5f);

                    var min = new Vector3(x1, 0, y1).RotateAroundPivot(collider.transform.position, collider.transform.rotation);
                    var max = new Vector3(x2, 0, y2).RotateAroundPivot(collider.transform.position, collider.transform.rotation);

                    colliderData.Min = new Vector2(min.x, min.z);
                    colliderData.Max = new Vector2(max.x, max.z);

                    GizmoDrawCollider(colliderData, Color.red, collider.gameObject, boxCollider);

                    break;
                case SphereCollider sphereCollider:
                    colliderData.Type = ColliderType.Sphere;
                    colliderData.Position -= sphereCollider.center;
                    colliderData.Size = (sphereCollider.radius * Vector3.one);
                    break;
                default:
                    Debug.LogError($"Unsupported type of collider {collider.name} => {collider.GetType()}", collider );
                    break;
            }
        }
    }
    
    public Matrix4x4 RotationMatrixAroundAxis(Ray axis, float rotation) {
        return Matrix4x4.TRS(-axis.origin, Quaternion.AngleAxis(rotation, axis.direction), Vector3.one)
               * Matrix4x4.TRS(axis.origin, Quaternion.identity, Vector3.one);
    }
    
    public void GizmoDrawCollider(ColliderData collider, Color color, GameObject obg, BoxCollider collider2)
    {
        if (collider.Type == ColliderType.Box)
        {
            Gizmos.color = Color.blue;
            
            var x1 = collider.Position.x - (collider.Size.x * 0.5f);
            var x2 = collider.Position.x + (collider.Size.x * 0.5f);
            var y1 = collider.Position.z - (collider.Size.z * 0.5f);
            var y2 = collider.Position.z + (collider.Size.z * 0.5f);
            
            var px1 = new Vector3(x1, 0, y1);
            var px2 = new Vector3(x2, 0, y2);

            Gizmos.DrawLine(px1, px2);
            
            Gizmos.color = Color.green;

            var pxx1 = px1.RotateAroundPivot(obg.transform.position, obg.transform.rotation);
            var pxx2 = px2.RotateAroundPivot(obg.transform.position, obg.transform.rotation);
            
            Gizmos.DrawLine(pxx1, pxx2);

            /*var p1 = new Vector3(collider.Min.x, 0, collider.Min.y);
            var p2 = new Vector3(collider.Max.x, 0, collider.Min.y);
            var p3 = new Vector3(collider.Max.x, 0, collider.Max.y);
            var p4 = new Vector3(collider.Min.x, 0, collider.Max.y);*/
            
            
            Gizmos.color = Color.red;
            // var p1 = new Vector3(collider.Position.x - collider.Min.x, 0, collider.Position.y - collider.Min.y);
            // var p2 = new Vector3(collider.Position.x + collider.Max.x, 0, collider.Position.y - collider.Min.y);
            // var p3 = new Vector3(collider.Position.x + collider.Max.x, 0, collider.Position.y + collider.Max.y);
            // var p4 = new Vector3(collider.Position.x - collider.Min.x, 0, collider.Position.y + collider.Max.y);


            var p1 = new Vector3(collider.Min.x, 0, collider.Min.y);
            var p2 = new Vector3(collider.Max.x, 0, collider.Min.y);
            var p3 = new Vector3(collider.Max.x, 0, collider.Max.y);
            var p4 = new Vector3(collider.Min.x, 0, collider.Max.y);
            
            //var center = p1
                //obg.transform.rotation

            //Matrix4x4 matrix = RotationMatrixAroundAxis(new Ray(collider.Position, obg.transform.up), angel);
            //var matrix = obg.transform.localToWorldMatrix;
            //Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(angel, rot), Vector3.one);
            
            // p1 = matrix.MultiplyPoint(p1);
            // p2 = matrix.MultiplyPoint(p2);
            // p3 = matrix.MultiplyPoint(p3);
            // p4 = matrix.MultiplyPoint(p4);
        
            
            // p1 = transformRotation * p1;
            // p2 = transformRotation * p2;
            // p3 = transformRotation * p3;
            // p4 = transformRotation * p4;

            //p1 = p1.RotateAroundPivot(collider.Position, Quaternion.AngleAxis(angel, rot));
            //p2 = p2.RotateAroundPivot(collider.Position, Quaternion.AngleAxis(angel, rot));
            //p3 = p3.RotateAroundPivot(collider.Position, Quaternion.AngleAxis(angel, rot));
            //p4 = p4.RotateAroundPivot(collider.Position, Quaternion.AngleAxis(angel, rot));
                        
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }
}


public static class RotateAroundPivotExtensions
{
    //Returns the rotated Vector3 using a Quaternion
    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle) => (Angle * (Point - Pivot)) + Pivot;
    //public static Vector2 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle) => (Angle * (Point - Pivot)) + Pivot;
    
    public static float3 RotateAroundPivot(this float3 point, float3 pivot, float4 rotation) => math.mul(rotation,point - pivot) + pivot;

    //Returns the rotated Vector3 using Euler
    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Vector3 Euler) => RotateAroundPivot(Point, Pivot, Quaternion.Euler(Euler));

    //Rotates the Transform's position using a Quaternion
    public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Quaternion Angle)
    {
        Me.position = Me.position.RotateAroundPivot(Pivot, Angle);
    }

    //Rotates the Transform's position using Euler
    public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Vector3 Euler)
    {
        Me.position = Me.position.RotateAroundPivot(Pivot, Quaternion.Euler(Euler));
    }
}