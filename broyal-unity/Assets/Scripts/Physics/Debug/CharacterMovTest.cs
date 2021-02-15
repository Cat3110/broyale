using System.Collections.Generic;
using Bootstrappers;
using FullSerializer;
using RemoteConfig;
using Unity.Mathematics;
using UnityEngine;

namespace PhysicsDebug
{
    public class CharacterMovTest : MonoBehaviour
    {
        public float speed = 10.0f;
        public float size = 0.5f;
        public TextAsset collidersFile;

        public List<ColliderData> colliders;

        public PhysicsCollisionResolveType collisionResolveType;
        public Box? sweptBox;

        // Start is called before the first frame update
        void Awake()
        {
            _inputMaster = new InputMaster();
            _inputMaster.Enable();

            fsSerializer fsSerializer = new fsSerializer();
            var result = fsJsonParser.Parse(collidersFile.text, out fsData fsData);
            if (result.Failed) Debug.LogError($"Unable to parse colliders info {result.FormattedMessages}");
            else
            {
                colliders = new List<ColliderData>();
                fsSerializer.TryDeserialize(fsData, ref colliders);
            }

            transform.localScale = Vector3.one * size;
        }

        private void OnDrawGizmos()
        {
            if (colliders == null) return;

            for (var index = 0; index < colliders.Count; index++)
            {
                var collider = colliders[index];
                if (_withColliderIndex == index) ClientBootstrapper.GizmoDrawCollider(collider, Color.red);
                else ClientBootstrapper.GizmoDrawCollider(collider, Color.magenta);

                Gizmos.DrawCube(collider.Position, Vector3.one * 0.1f);
            }

            Gizmos.color = _collided ? Color.red : Color.green;
            Gizmos.DrawCube(transform.position, new float3(size));

            if (sweptBox.HasValue)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(new Vector3(sweptBox.Value.x, 0, sweptBox.Value.y), new float3(0.5f));
            }
        }


        // Update is called once per frame

        private bool _collided;
        private int _withColliderIndex;
        private InputMaster _inputMaster;

        private List<Vector3> debug_positions = new List<Vector3>();

        void FixedUpdate()
        {
            if(  Camera.main != null)
                Camera.main.transform.position = new Vector3(transform.position.x,  Camera.main.transform.position.y, transform.position.z);
        }
        
        void Update()
        {
            
            _collided = false;
            _withColliderIndex = -1;

            if (debug_positions.Count > 1)
            {
                for (int i = 1; i < debug_positions.Count; i++)
                {
                    Debug.DrawLine(debug_positions[i - 1], debug_positions[i], Color.yellow);
                }
            }

            var input = _inputMaster.Player.Movement.ReadValue<Vector2>();
            var action = (short) _inputMaster.Player.MainAction.ReadValue<float>();

            float3 position = transform.position;

            //var direction = new float2(input.x, input.y);

            if (input.sqrMagnitude < 0.01) return;
            //Debug.Log($"Input {input}");
            //var vx = (input.x * Time.deltaTime * speed) * Time.realtimeSinceStartup;
            //var vy = (input.y * Time.deltaTime * speed) * Time.realtimeSinceStartup;

            var vx = input.x * Time.deltaTime * speed;
            var vy = input.y * Time.deltaTime * speed;

            var box = new Box(position.x - size * 0.5f, position.z - size * 0.5f, size, size, vx, vy);
            //var box = new Box(position.x, position.z, size, size, vx, vy );

            float moveX = 0.0f;
            float moveY = 0.0f;
            float normalX = 0.0f;
            float normalY = 0.0f;
            float collisionTime = 1.0f;
            Vector2 velocity = Vector2.zero;

            for (var index = 0; index < colliders.Count; index++)
            {
                var collider = colliders[index];
                if (collider.Type != ColliderType.Box) continue;

                var broadPhaseBox = SweptAABB.GetBroadPhaseBox(box);
                var block = collider.ToBox();

                if (SweptAABB.CheckAABB(broadPhaseBox, block, ref normalX, ref normalY))
                {
                    _collided = true;
                    _withColliderIndex = index;

                    collisionTime = SweptAABB.Swept(box, block, ref normalX, ref normalY);
                    //     box.x += box.vx * collisiontime;
                    //     box.y += box.vy * collisiontime;

                    Debug.Log($"Collided {collisionTime} {normalX} {normalY}");
                    break;
                }
            }

            var newPosition = Vector3.zero;
            if (_collided)
            {
                var remainingtime = 1.0f - collisionTime;

                if (collisionResolveType == PhysicsCollisionResolveType.Push)
                {
                    float magnitude = math.sqrt((box.vx * box.vx + box.vy * box.vy)) * remainingtime; 
                    float dotprod = box.vx * normalY + box.vy * normalX; 
                    
                    if (dotprod > 0.0f) dotprod = 1.0f; 
                    else if (dotprod < 0.0f) dotprod = -1.0f; 
                    
                    box.vx = dotprod * normalY * magnitude; 
                    box.vy = dotprod * normalX * magnitude;
                    
                    newPosition = new Vector3(position.x + (box.vx), 0, position.z + (box.vy));
                }
                else if (collisionResolveType == PhysicsCollisionResolveType.Slide)
                {
                    float dotprod = (box.vx * normalY + box.vy * normalX) * remainingtime;

                    box.vx = dotprod * normalY;
                    box.vy = dotprod * normalX;

                    newPosition = new Vector3(position.x + (box.vx), 0, position.z + (box.vy));
                }
            }
            else
            {
                newPosition = new Vector3(position.x + (box.vx * collisionTime), 0,
                    position.z + (box.vy * collisionTime));
            }

            transform.position = newPosition;
            debug_positions.Add(newPosition);
        }
    }
}
    