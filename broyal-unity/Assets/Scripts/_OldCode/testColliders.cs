using System.Collections;
using System.Collections.Generic;
using Bootstrappers;
using FullSerializer;
using Unity.Mathematics;
using UnityEngine;


[ExecuteInEditMode]
public class testColliders : MonoBehaviour
{
    public TextAsset collidersFile;

    public List<ColliderData> _colliders;
    // Start is called before the first frame update
    void Awake()
    {
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
        if (_colliders != null)
        {
            foreach (var collider in _colliders)
            {
                ClientBootstrapper.GizmoDrawCollider(collider, Color.magenta);
            }
        }

        Gizmos.color = collided ? Color.red : Color.green;
        Gizmos.DrawCube(transform.position, new float3(0.5f));
    }


    // Update is called once per frame

    private bool collided;
    void Update()
    {
        collided = false;
            
        //if (_isServer)
        {
            if (_colliders == null) return;
            for (int i = 0; i < _colliders.Count; i++)
            {
                var collider = _colliders[i];
                if (collider.Type == ColliderType.Box)
                {
                    collided = MoveSystem.IntersectWithCircle(new float3(transform.position).xz, 0.5f,
                        new float3(collider.Position).xz, new float3(collider.Size).xz);
                    //collided = MoveSystem.Intersect(collider.Min, collider.Max, transform.position, new float3(0.5f));
                    if (collided)
                    {
                        break;
                    }
                }
            }
        }
    }
}
