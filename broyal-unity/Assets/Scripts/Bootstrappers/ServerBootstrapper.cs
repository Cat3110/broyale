using CUDLR;

namespace Bootstrappers
{
    using UnityEngine;
    using Unity.Entities;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using Utils;
    
    using RemoteConfig;
    using UniRx.Async;

    public class ServerBootstrapper : BaseBootStrapper
    {
        [SerializeField] private MainConfig config;

        private World _world;
        private EntityManager _entityManager;

        private static AppConfig OnConfigLoaded(string jsonConfig)
        {
            var appConfig = new AppConfig();
                
            if (!string.IsNullOrEmpty(jsonConfig))
            {
                appConfig.Load(jsonConfig);
            }
            
            Container.Register(appConfig);
            
            return appConfig;
        }


        private void InitWorlds(ushort port)
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ServerSimulationSystemGroup>() == null) continue;
                
                var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
                if (StartListenConnections(network, port))
                {
                    InitWorld(world);
                }
            }
        }

        private void Start()
        {
            Console.AddCommandByMethod(() => RealoadConfig() );
            
            Container.Register(config);
            
            Debug.Log("Remote config loading...");
            AppConfig.LoadByUrlAsync().ContinueWith( (json) =>
            {
                var loadedConfig = OnConfigLoaded(json);
                InitWorlds(loadedConfig.Main.ServerPort);
            });
        }

        // Server world automatically listens for connections from any host
        private bool StartListenConnections(NetworkStreamReceiveSystem network, ushort port)
        {
            NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
            ep.Port = port;
            
            var listenStatus = network.Listen(ep);
            
            Debug.Log( $"StartListenConnections({ep.Address}) => {listenStatus}" );
            
            return listenStatus;
        }

        private void InitWorld(World world)
        {
            _world = world;
            _entityManager = world.EntityManager;
        }
        
        [CUDLR.Command("reload сonfig", "Reload config file from gdoc")]
        public static void RealoadConfig() {
            AppConfig.LoadByUrlAsync().ContinueWith( (json) => OnConfigLoaded(json));
        }

        private void OnDestroy()
        {
          
        }
    }
}