using System.Collections;
using System.Collections.Generic;
using Bootstrappers;
using FullSerializer;
using UnityEngine;

[ExecuteAlways]
public class DebugColliderView : MonoBehaviour
{
    public TextAsset collidersFile;

    public List<ColliderData> _colliders;
    // Start is called before the first frame update
    void Awake()
    {
        if (collidersFile == null) return;
        
        fsSerializer fsSerializer = new fsSerializer();
        var result = fsJsonParser.Parse(collidersFile.text, out fsData fsData);
        if( result.Failed ) Debug.LogError($"Unable to parse colliders info {result.FormattedMessages}");
        else
        {
            _colliders = new List<ColliderData>();
            fsSerializer.TryDeserialize(fsData, ref _colliders);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (_colliders == null || _colliders.Count == 0) return;
        
        foreach (var collider in _colliders)
        {
            ClientBootstrapper.GizmoDrawCollider(collider, Color.magenta);
            //break;
        }
    }
}
