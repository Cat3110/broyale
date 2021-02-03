namespace Bootstrappers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    
    using UnityEngine;
    using Unity.Physics.Systems;
    using Unity.Entities;
    using Unity.Rendering;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using RemoteConfig;
    
    using UniRx;
    using UniRx.Async;
    using Utils;
    
    using Scripts.Common.Data.Data;
    using SocketIO.Data.Responses;

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
        private AppConfig _appConfig;
        
        private void OnConfigLoaded(string jsonConfig)
        {
            _appConfig = new AppConfig();
                
            if (!string.IsNullOrEmpty(jsonConfig))
            {
                _appConfig.Load(jsonConfig);
            }

            if (!string.IsNullOrEmpty(GlobalSettings.ServerAddress))
            {
                _appConfig.Main.ServerAddress = GlobalSettings.ServerAddress;
            }
            
            if (GlobalSettings.ServerPort.HasValue)
            {
                _appConfig.Main.ServerPort = GlobalSettings.ServerPort.Value;
            }
            
            Container.Register(_appConfig);
            
            uiController.LoadingUI.Hide();

            Container.TryResolve(out IGlobalSession globalSession);

            if (globalSession!= null && globalSession.IsValid) StartBattle(_appConfig, globalSession.User, globalSession.Character);
            else
            {
                uiController.MainUI.Show(_appConfig.Characters, 
                    _appConfig.Skills.Where( s => s.IsEnabled && s.Type == SkillType.Main).Select(c => c.Id).ToList(),
                    _appConfig.Skills.Where( s => s.IsEnabled && s.Type == SkillType.Attack).Select(c => c.Id).ToList()
                    );
                uiController.MainUI.OnGameStarted += StartLocalBattle;
            }
        }
        
        private void StartBattle(AppConfig appConfig, User globalUser, Character globalCharacter)
        {
            var mainSkill = _appConfig.GetSkillConfigById(globalCharacter.skill_set.main_skill);
            var attackSkill = _appConfig.GetSkillConfigById(globalCharacter.skill_set.attack_skill);
            var defenceSkill = _appConfig.GetSkillConfigById(globalCharacter.skill_set.defence_skill);
            var utilsSkill = _appConfig.GetSkillConfigById(globalCharacter.skill_set.utils_skill);

            var skills = new[] {mainSkill, attackSkill, defenceSkill, utilsSkill}
                .Where(skill => skill.Type != SkillType.None).ToArray();

            Container.Register(new Session {UserId = globalUser._id} );

            uiController.LoadingUI.Show();

            //TODO: need to find way for make it better
            Observable.Timer(TimeSpan.FromSeconds(1))
                .Subscribe(
                    (x) => { }, 
                    () => {  
                        Container.Resolve<InputMaster>().Enable();
                        uiController.LoadingUI.Hide();
                        uiController.GameUI.Show(skills); })
                .AddTo(this);

            InitWorlds( useLocalServer ? "127.0.0.1" : appConfig.Main.ServerAddress, useLocalServer ? (ushort)7979 : _appConfig.Main.ServerPort);
        } 


        private void StartLocalBattle(string skillId, string attackSkillId, RemoteConfig.CharacterInfo character)
        {
            var mainSkill = _appConfig.GetSkillConfigById(skillId);
            var attackSkill = _appConfig.GetSkillConfigById(attackSkillId);
            
            Container.Register(new Session {
                MainSkill = mainSkill,
                AttackSkill = attackSkill,
                Character = character
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
                        uiController.GameUI.Show(new []{ mainSkill, attackSkill }); })
                .AddTo(this);

            InitWorlds( useLocalServer ? "127.0.0.1" : _appConfig.Main.ServerAddress, useLocalServer ? (ushort)7979 : _appConfig.Main.ServerPort);
        }

        private void InitWorlds(string address, ushort port)
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ClientSimulationSystemGroup>() == null) continue;
                
                //world.EntityManager.DestroyEntity(world.EntityManager.UniversalQuery);
                
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
            world.GetOrCreateSystem<StepPhysicsWorld>().Enabled = false;
            world.GetOrCreateSystem<PhysicsWorldHistory>().Enabled = false;
            world.GetOrCreateSystem<ExportPhysicsWorld>().Enabled = false;
            //world.GetOrCreateSystem<>().Enabled = false;
            
            #if !UNITY_EDITOR
            var localClearingSystem = World.DefaultGameObjectInjectionWorld.CreateSystem<ClearingPresenterSystem>();
            localClearingSystem.Init(world);
            var localSimulationGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<SimulationSystemGroup>();
            localSimulationGroup.AddSystemToUpdateList( localClearingSystem );
            #endif
        }

        private void OnDestroy()
        {
            //_entityManager.CompleteAllJobs();
            //_entityManager.DestroyEntity(_entityManager.UniversalQuery);
            
            ConnectionSystem.disconnectRequested = true;
            
            // foreach (var world in World.All)
            // {
            //     var conSystem = world.GetExistingSystem<ReconnectSystem>();
            //     if (conSystem != null)
            //     {
            //         if (conSystem.ClientConnectionState == ReconnectSystem.ConnectionState.Connected)
            //         {
            //             conSystem.ClientConnectionState = ReconnectSystem.ConnectionState.TriggerDisconnect;
            //         }
            //     }
            // }
            
            var lateUpdateGroup = _world.GetExistingSystem<PresentationSystemGroup>();
            lateUpdateGroup.RemoveSystemFromUpdateList(_world.GetExistingSystem<PrefabInstanciateSystem>() );
            lateUpdateGroup.RemoveSystemFromUpdateList(_world.GetExistingSystem<PrefabCreatorSystem>() );
            lateUpdateGroup.RemoveSystemFromUpdateList(_world.GetExistingSystem<UpdateCameraSystem>());
            lateUpdateGroup.RemoveSystemFromUpdateList(_world.GetExistingSystem<CharacterPresenterSystem>() );
            
            _world.DestroySystem(_world.GetExistingSystem<PrefabInstanciateSystem>());
            _world.DestroySystem(_world.GetExistingSystem<PrefabCreatorSystem>());
            _world.DestroySystem(_world.GetExistingSystem<UpdateCameraSystem>());
            _world.DestroySystem(_world.GetExistingSystem<CharacterPresenterSystem>());
        }
    }
}

public static class GlobalSettings
{
    public static string ServerAddress;
    public static ushort? ServerPort;
}