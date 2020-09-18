namespace Bootstrappers
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.Physics;
    using Unity.Transforms;
    using Unity.Rendering;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using Utils;

    public class PreviewBootstrapper : MonoBehaviour
    {
        public static IContainer Container = new Container();
        
       // [SerializeField] private MainConfig Config;
        
       // [SerializeField] private GameObject Player;
        
        //[SerializeField] private CameraSettings _cameraSettings;
        
        private World _world;
        private BlobAssetStore _store;
        
        private void Start()
        {
           // Container.Register(Config);
          //  Container.Register(_cameraSettings);
            
            foreach (var world in World.All)
            {
                var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
                if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
                {
//#if CLIENT_BUILD                  
                    // Client worlds automatically connect to localhost
                    NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
                    ep.Port = 7979;
                    network.Connect(ep);

                    var systemGroup = world.GetOrCreateSystem<ClientSimulationSystemGroup>();
                    
                    //systemGroup.AddSystemToUpdateList(world.CreateSystem<PrefabInstanciateSystem>() );
                    //systemGroup.AddSystemToUpdateList(world.CreateSystem<PrefabCreatorSystem>() );
                    //systemGroup.AddSystemToUpdateList(world.CreateSystem<CharacterPresenterSystem>() );
                    //systemGroup.AddSystemToUpdateList(world.CreateSystem<UpdateCameraSystem>());

                    var lateUpdateGroup = world.GetOrCreateSystem<PresentationSystemGroup>();
                    lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<PrefabInstanciateSystem>() );
                    lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<PrefabCreatorSystem>() );
                    lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<UpdateCameraSystem>());
                    lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<CharacterPresenterSystem>() );

                    world.GetOrCreateSystem<CopySkinnedEntityDataToRenderEntity>().Enabled = false;
                    world.GetOrCreateSystem<RenderMeshSystemV2>().Enabled = false;
//#endif
                }
// #if UNITY_EDITOR && !CLIENT_BUILD
//                 else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
//                 {
//                     // Server world automatically listens for connections from any host
//                     NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
//                     ep.Port = 7979;
//                     network.Listen(ep);
//                 }
// #endif
            }



            /*var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore());
            EPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Player, settings);
                    
            World.DefaultGameObjectInjectionWorld.EntityManager.Instantiate(EPrefab);*/

            //var entity = Instantiate(Player).GetComponent<GameObjectEntity>().Entity;

            //World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PlayerData>(entity);
        }
        
        private void OnDestroy()
        {
          
        }

        private void InitWorld()
        {
            _store = new BlobAssetStore();
            _world = new World("Common");

            World.DefaultGameObjectInjectionWorld = _world;
            var entityManager = _world.EntityManager;

            //_world.CreateSystem<Game>();
            //_world.CreateSystem<GoInGameServerSystem>();

            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(_world);

            // Create Archetype.
            var archetype = entityManager.CreateArchetype(
                ComponentType.ReadWrite<PlayerData>(),
                ComponentType.ReadWrite<PhysicsCollider>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>());

            // Create Prefab Entities.
            var prefabEntity = entityManager.CreateEntity(archetype);

            //entityManager.SetSharedComponentData(prefabEntity, this._meshInstanceRenderer);
            // Create Entities.
            for (int i = 0; i < 10; ++i)
            {
                var entity = entityManager.Instantiate(prefabEntity);
                entityManager.SetComponentData(entity, new Translation {Value = UnityEngine.Random.Range(1.0f,3.0f)});
                entityManager.SetComponentData(entity, new Rotation {Value = UnityEngine.Random.rotation});
            }
        }

        private void DestoyWorld()
        {
            _store.Dispose();
            _world.Dispose();
        }
    }
}