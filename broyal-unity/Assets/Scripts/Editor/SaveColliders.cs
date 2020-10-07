using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using FullSerializer;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public static class SaveColliders
{
    [MenuItem("Tools/Save Colliders to Json")]
    public static void SaveToJson()
    {
        GameObject selected = Selection.activeObject as GameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Select root object",
                "You Must Select some root object first!",
                "Ok");
            return;
        }

        

        var colliders = selected.GetComponentsInChildren<Collider>(includeInactive: false);
        
        var list = new List<ColliderData>();

        foreach (Collider collider in colliders)
        {
            var colliderData = new ColliderData
            {
                Name = collider.name,
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

                    var xx1 = new Vector3(x1, 0, y1).RotateAroundPivot(collider.transform.position, collider.transform.rotation);
                    var xx2 = new Vector3(x2, 0, y2).RotateAroundPivot(collider.transform.position, collider.transform.rotation);

                    colliderData.Min = new float2(math.min(xx1.x, xx2.x), math.min(xx1.z, xx2.z));
                    colliderData.Max = new float2(math.max(xx1.x, xx2.x), math.max(xx1.z, xx2.z));
                    
                    list.Add( colliderData );
                    break;
                case SphereCollider sphereCollider:
                    colliderData.Type = ColliderType.Sphere;
                    colliderData.Position -= sphereCollider.center;
                    colliderData.Size = (sphereCollider.radius * Vector3.one);
                    list.Add( colliderData );
                    break;
                default:
                    Debug.LogError($"Unsupported type of collider {collider.name} => {collider.GetType()}", collider );
                    break;
            }
        }

        fsSerializer fsSerializer = new fsSerializer();
        fsSerializer.TrySerialize(list, out fsData data);
        var json = fsJsonPrinter.PrettyJson(data);
        
            
        var path = EditorUtility.SaveFilePanel(
            "Save as JSON",
            "",
            selected.name + ".json",
            "json");

        if (path.Length == 0) return;
        
        File.WriteAllBytes( path, Encoding.ASCII.GetBytes(json) );
        
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
    }
}

