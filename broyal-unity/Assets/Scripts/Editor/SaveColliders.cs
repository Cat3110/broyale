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
                    colliderData.Position -= boxCollider.center;
                    
                    var dot = Vector3.Dot(collider.transform.forward, Vector3.forward);
                    if (Mathf.Abs(dot) < 0.2)
                    {
                        Debug.Log($"Flip => {dot}", collider.gameObject);
                        colliderData.Size = new Vector3(boxCollider.size.z, 0, boxCollider.size.x);
                    } else  colliderData.Size = boxCollider.size;

                    var x1 = colliderData.Position.x - (colliderData.Size.x * 0.5f);
                    var x2 = colliderData.Position.x + (colliderData.Size.x * 0.5f);
                    var y1 = colliderData.Position.z - (colliderData.Size.z * 0.5f);
                    var y2 = colliderData.Position.z + (colliderData.Size.z * 0.5f);

                    colliderData.Min = new Vector2(math.min(x1, x2), math.min(y1, y2));
                    colliderData.Max = new Vector2(math.max(x1, x2), math.max(y1, y2));

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

