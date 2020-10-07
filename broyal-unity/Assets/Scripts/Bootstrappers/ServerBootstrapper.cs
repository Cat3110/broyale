﻿namespace Bootstrappers
{
    using CUDLR;

    using UnityEngine;
    using Unity.Entities;
    using Unity.NetCode;
    using Unity.Networking.Transport;
    using RemoteConfig;
    using UniRx.Async;

    public class ServerBootstrapper : BaseBootStrapper
    {
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

        public override void Start()
        {
            base.Start();
            
            Console.AddCommandByMethod(() => RealoadConfig() );

            Container.Register(config);
            
            Debug.Log("Remote config loading...");
            AppConfig.LoadByUrlAsync().ContinueWith( (json) =>
            {
                var loadedConfig = OnConfigLoaded(json);
                
                var globalPort = CommandLine.GetArg("--port");
                if (globalPort != null)
                {
                    loadedConfig.Main.ServerPort = ushort.Parse(globalPort);
                }
                
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
            
            var serverSimulationSystemGroup = world.GetOrCreateSystem<ServerSimulationSystemGroup >();
            serverSimulationSystemGroup.AddSystemToUpdateList(world.CreateSystem<ItemSpawnerSystem>());
            serverSimulationSystemGroup.AddSystemToUpdateList( world.CreateSystem<LootItemSystem>());
            serverSimulationSystemGroup.AddSystemToUpdateList( world.CreateSystem<ZoneDamageSystem>());
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

public static class CommandLine
{
    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}