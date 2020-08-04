using RemoteConfig;
using UniRx.Async;

namespace Bootstrappers
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Entities;
    using Unity.Physics;
    using Unity.Mathematics;
    using Unity.Transforms;
    using Unity.Rendering;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using Utils;

    public class ClientBootstrapper : BaseBootStrapper
    {
        [SerializeField] private InputMaster controls;

        [SerializeField] private MainConfig config;
        [SerializeField] private CameraSettings cameraSettings;
        [SerializeField] private UIController uiController;
        
        [SerializeField] private bool useLocalServer = true;
        [SerializeField] private int maxFps = Int32.MaxValue;
        
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
            
            InitWorlds( useLocalServer ? "127.0.0.1" : appConfig.Main.ServerAddress, appConfig.Main.ServerPort);
        }

        private void InitWorlds(string address, ushort port)
        {
            uiController.LoadingUI.Show(false);
            
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

        private void Start()
        {
            Application.targetFrameRate = maxFps;
            
            SRDebug.Init();
            
            Container.Register(config);
            Container.Register(cameraSettings);
            Container.Register(uiController);

            uiController.LoadingUI.Show(true);
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
        }
        
        private void OnDestroy()
        {
          
        }
    }
}