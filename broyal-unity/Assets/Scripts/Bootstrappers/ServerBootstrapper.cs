using System;
using FullSerializer;
using Scripts.Common.Data.Data;
using SocketIO.Data.Responses;
using UnityEngine.Networking;

namespace Bootstrappers
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

        private void InitWorlds(ushort port)
        {
            foreach (var world in World.All)
            {
                if (world.GetExistingSystem<ServerSimulationSystemGroup>() == null) continue;
                
                //world.EntityManager.DestroyEntity(world.EntityManager.UniversalQuery);
                
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
            
            Console.AddCommandByMethod(() => ReloadConfig() );

            Container.Register(config);
            
            Container.TryResolve<IGlobalSession>( out IGlobalSession globalSession);

            Debug.Log("Remote config loading...");
            AppConfig.LoadByUrlAsync().ContinueWith( (json) =>
            {
                var loadedConfig = OnConfigLoaded(json);
                
                var globalPort = CommandLine.GetArg("--port");
                if (globalPort != null)
                {
                    loadedConfig.Main.ServerPort = ushort.Parse(globalPort);
                }
                
                var gameId = CommandLine.GetArg("--gameid");
                if (globalSession?.Game != null) //TODO: Have separate logic(client\serv\editor)
                {
                    LoadGameData(globalSession.Game.id)
                        .ContinueWith(data =>
                        {
                            Container.Resolve<IGlobalSession>().Game = data.Item1;
                            Container.Resolve<IGlobalSession>().CharactersInGame = data.Item2;
                            
                            //InitWorlds(Container.Resolve<IGlobalSession>().Game);
                            InitWorlds(loadedConfig.Main.ServerPort);
                        });
                }
                else if (gameId != null)
                {
                    LoadGameData(gameId)
                        .ContinueWith(data =>
                        {
                            var (game, characters) = data;
                            var newSession = new GlobalSession
                            {
                                Game = game,
                                CharactersInGame = characters
                            };
                            MainContainer.Container.Register<IGlobalSession>(newSession);
                            InitWorlds(loadedConfig.Main.ServerPort);
                        });
                }else InitWorlds(loadedConfig.Main.ServerPort);
            });
        }
        
        static readonly fsSerializer Serializer = new fsSerializer();
        public static async UniTask<(Game,Character[])> LoadGameData(string gameId)
        {
            var jsonGames = await GetJsonByUrl($"http://localhost:3000/game/{gameId}/full");
            Game game = null;
            Character[] characters = null;

            var result = fsJsonParser.Parse(jsonGames, out fsData fsData)
                .Merge(Serializer.TryDeserialize(fsData.AsDictionary["game"], ref game))
                .Merge(Serializer.TryDeserialize(fsData.AsDictionary["characters"], ref characters));
            
            if (result.Succeeded) return (game, characters);
            else throw result.AsException;
        }
        
        public static async UniTask<string> GetJsonByUrl(string url)
        { 
            await UniTask.SwitchToMainThread();
            
            using (var req = UnityWebRequest.Get(url))
            {
                var op = await req.SendWebRequest();
                if (string.IsNullOrEmpty(op.error))
                {
                    return op.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("[App] GetJsonByUrl Result:" + op.error);
                    return null;
                }
            }  
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
            
            var ghostPredictionSystemGroup = world.GetOrCreateSystem<GhostPredictionSystemGroup>();
            ghostPredictionSystemGroup.AddSystemToUpdateList( world.CreateSystem<MoveSystem>());
            
            var serverSimulationSystemGroup = world.GetOrCreateSystem<ServerSimulationSystemGroup >();
            serverSimulationSystemGroup.AddSystemToUpdateList(world.CreateSystem<ItemSpawnerSystem>());
            serverSimulationSystemGroup.AddSystemToUpdateList( world.CreateSystem<LootItemSystem>());
            serverSimulationSystemGroup.AddSystemToUpdateList( world.CreateSystem<ZoneDamageSystem>());
        }
        
        [CUDLR.Command("reload сonfig", "Reload config file from gdoc")]
        public static void ReloadConfig() {
            AppConfig.LoadByUrlAsync().ContinueWith(OnConfigLoaded);
        }

        private void OnDestroy()
        {
            //_entityManager.CompleteAllJobs();
            
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(World.DefaultGameObjectInjectionWorld
                .EntityManager.UniversalQuery);
            
            _entityManager.DestroyEntity(_entityManager.UniversalQuery);
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