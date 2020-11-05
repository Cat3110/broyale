using System.Collections.Generic;
using UniRx;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEditor;
using Utils;

namespace Bootstrappers
{
    using System;
    using System.Linq;
    using UnityEngine;
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using RemoteConfig;
    using UniRx.Async;


    public class ClientBootstrapper : BaseBootStrapper
    {
        [SerializeField] private InputMaster controls;

        //[SerializeField] private MainConfig config;
        [SerializeField] private CameraSettings cameraSettings;
        [SerializeField] private UIController uiController;

        [SerializeField] private bool useLocalServer = true;
        [SerializeField] private int maxFps = Int32.MaxValue;

        [SerializeField] private FXData fxData;
        
        private World _world;
        private EntityManager _entityManager;
        
        private void OnConfigLoaded(string jsonConfig)
        {
            var appConfig = new AppConfig();
                
            if (!string.IsNullOrEmpty(jsonConfig))
            {
                appConfig.Load(jsonConfig);
            }

            if (!string.IsNullOrEmpty(GlobalSettings.ServerAddress))
            {
                appConfig.Main.ServerAddress = GlobalSettings.ServerAddress;
            }
            
            if (GlobalSettings.ServerPort.HasValue)
            {
                appConfig.Main.ServerPort = GlobalSettings.ServerPort.Value;
            }
            
            Container.Register(appConfig);
            
            uiController.LoadingUI.Hide();
            uiController.MainUI.Show(appConfig.Characters, appConfig.Skills.Take(4).Select( c => c.Id).ToList());


            uiController.MainUI.OnGameStarted += (skillId, character) =>
            {
                Container.Register(new Session
                {
                    SkillId = appConfig.Skills.FindIndex( s => s.Id == skillId),
                    Character = character,
                } );
                
                uiController.MainUI.Hide();
                uiController.LoadingUI.Show();

                //TODO: need to find way for make it better
                Observable.Timer(TimeSpan.FromSeconds(1))
                    .Subscribe(
                        (x) => { }, 
                        () => {  
                            Container.Resolve<InputMaster>().Enable();
                            uiController.LoadingUI.Hide();
                            uiController.GameUI.Show(skillId); })
                    .AddTo(this);

                InitWorlds( useLocalServer ? "127.0.0.1" : appConfig.Main.ServerAddress, appConfig.Main.ServerPort);
            };
        }

        private void InitWorlds(string address, ushort port)
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ClientSimulationSystemGroup>() == null) continue;
                
                var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
                if (ConnectToServer(network, address, port))
                {
                    InitWorld(world);
                }
            }
        }

        public override void Start()
        {
            base.Start();
            
            Application.targetFrameRate = maxFps;
            
            SRDebug.Init();
            
            controls = new InputMaster();
            controls.Disable();
            
            Container.Register(controls);
            Container.Register(config);
            Container.Register(cameraSettings);
            Container.Register(uiController);
            Container.Register(fxData);
            Container.Register(Debug.unityLogger);
            Container.RegisterSingleton<IAssetManager,AssetsManager>();
            
            uiController.Init(Container);
            uiController.LoadingUI.Show();
            
            AppConfig.LoadByUrlAsync().ContinueWith(OnConfigLoaded);
        }

        private void Update()
        {
            //Debug.Log("MainAction " + controls.Player.Movement.ReadValue<Vector2>() );
        }

        private void OnDrawGizmos()
        {
            if (Container.TryResolve(typeof(List<ColliderData>), out object obj))
            {
                List<ColliderData> colliders = (List<ColliderData>)obj;
                foreach (var collider in colliders)
                {
                    GizmoDrawCollider(collider, Color.magenta);
                    //break;
                }
            }
        }

        public static void GizmoDrawCollider(ColliderData collider, Color color)
        {
            if (collider.Type == ColliderType.Box)
            {
                Gizmos.color = color;
                // var p1 = new Vector3(collider.Position.x - collider.Min.x, 0, collider.Position.y - collider.Min.y);
                // var p2 = new Vector3(collider.Position.x + collider.Max.x, 0, collider.Position.y - collider.Min.y);
                // var p3 = new Vector3(collider.Position.x + collider.Max.x, 0, collider.Position.y + collider.Max.y);
                // var p4 = new Vector3(collider.Position.x - collider.Min.x, 0, collider.Position.y + collider.Max.y);
                        
                var p1 = new Vector3(collider.Min.x, 0, collider.Min.y);
                var p2 = new Vector3(collider.Max.x, 0, collider.Min.y);
                var p3 = new Vector3(collider.Max.x, 0,  collider.Max.y);
                var p4 = new Vector3(collider.Min.x, 0, collider.Max.y);
                        
                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
            }
        }

        private bool ConnectToServer(NetworkStreamReceiveSystem network, string address, ushort port)
        {
            NetworkEndPoint ep = NetworkEndPoint.Parse(address,port);
            var connection = network.Connect(ep);
            
            Debug.Log( $"ConnectToServer({ep.Address}) => {connection}" );
            
            return connection != Entity.Null;
        }

        private void InitWorld(World world)
        {
            _world = world;
            _entityManager = world.EntityManager;
            
            var ghostPredictionSystemGroup = world.GetOrCreateSystem<GhostPredictionSystemGroup>();
            ghostPredictionSystemGroup.AddSystemToUpdateList( world.CreateSystem<MoveSystem>() );
            

            var lateUpdateGroup = world.GetOrCreateSystem<PresentationSystemGroup>();
            lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<PrefabInstanciateSystem>() );
            lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<PrefabCreatorSystem>() );
            lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<UpdateCameraSystem>());
            lateUpdateGroup.AddSystemToUpdateList(world.CreateSystem<CharacterPresenterSystem>() );

            world.GetOrCreateSystem<CopySkinnedEntityDataToRenderEntity>().Enabled = false;
            world.GetOrCreateSystem<RenderMeshSystemV2>().Enabled = false;
            world.GetOrCreateSystem<BuildPhysicsWorld>().Enabled = false;
            //world.GetOrCreateSystem<>().Enabled = false;
            
            var localClearingSystem = World.DefaultGameObjectInjectionWorld.CreateSystem<ClearingPresenterSystem>();
            localClearingSystem.Init(world);
            var localSimulationGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            localSimulationGroup.AddSystemToUpdateList( localClearingSystem );
        }
        
        private void OnDestroy()
        {
          
        }
    }
}

public static class GlobalSettings
{
    public static string ServerAddress;
    public static ushort? ServerPort;
}