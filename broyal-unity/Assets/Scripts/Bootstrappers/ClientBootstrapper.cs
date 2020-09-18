using Unity.Physics;
using Unity.Physics.Systems;

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
            
            Container.Register(appConfig);
            
            uiController.LoadingUI.Hide();
            uiController.MainUI.Show(
                appConfig.Characters.Select( c => c.Id),
                appConfig.Skills.Select( c => c.Id));


            uiController.MainUI.OnGameStarted += skillId =>
            {
                uiController.MainUI.Hide();
                uiController.GameUI.Show();

                Container.Register(new Session{ SkillId = appConfig.Skills.FindIndex( s => s.Id == skillId) } );
                InitWorlds( useLocalServer ? "127.0.0.1" : appConfig.Main.ServerAddress, appConfig.Main.ServerPort);
            };
        }

        private void OnGameStarted(string skillId)
        {
            
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
            
            Container.Register(config);
            Container.Register(cameraSettings);
            Container.Register(uiController);
            Container.Register(fxData);

            uiController.LoadingUI.Show();
            AppConfig.LoadByUrlAsync().ContinueWith(OnConfigLoaded);
        }

        private void Update()
        {
            //Debug.Log("MainAction " + controls.Player.Movement.ReadValue<Vector2>() );
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
            
            var systemGroup = world.GetOrCreateSystem<ClientSimulationSystemGroup>();
                    
            //systemGroup.AddSystemToUpdateList(world.CreateSystem<PrefabInstanciateSystem>() );
            //systemGroup.AddSystemToUpdateList(world.CreateSystem<PrefabCreatorSystem>() );
            //systemGroup.AddSystemToUpdateList(world.CreateSystem<CharacterPresenterSystem>() );
            //systemGroup.AddSystemToUpdateList(world.CreateSystem<UpdateCameraSystem>());

            var lateUpdateGroup = world.GetOrCreateSystem<PresentationSystemGroup >();
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