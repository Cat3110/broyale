using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;

// [UpdateInGroup(typeof(NetworkReceiveSystemGroup))]
// [UpdateBefore(typeof(NetworkStreamReceiveSystem))]
// [AlwaysUpdateSystem]
// public class ReconnectSystem : ComponentSystem
// {
//     public enum ConnectionState
//     {
//         Uninitialized,
//         NotConnected,
//         Connected,
//         TriggerDisconnect,
//         TriggerTimeout,
//         TriggerConnect
//     }
//
//     public ConnectionState ClientConnectionState;
//     private EntityQuery _clientConnectionGroup;
//     private NetworkEndPoint _prevEndPoint;
//
//     protected override void OnCreate()
//     {
//         _clientConnectionGroup = GetEntityQuery(
//             ComponentType.ReadWrite<NetworkStreamConnection>(),
//             ComponentType.Exclude<NetworkStreamDisconnected>());
//         
//         ClientConnectionState = ConnectionState.Uninitialized;
//     }
//
//     protected override void OnUpdate()
//     {
//         bool isConnected = !_clientConnectionGroup.IsEmptyIgnoreFilter;
//         // Trigger connect / disconnect events
//         if (ClientConnectionState == ConnectionState.TriggerDisconnect && isConnected)
//         {
//             var con = _clientConnectionGroup.ToComponentDataArray<NetworkStreamConnection>(Allocator.TempJob);
//             _prevEndPoint = World.GetExistingSystem<NetworkStreamReceiveSystem>().Driver.RemoteEndPoint(con[0].Value);
//             for (int i = 0; i < con.Length; ++i)
//             {
//                 World.GetExistingSystem<NetworkStreamReceiveSystem>().Driver.Disconnect(con[i].Value);
//             }
//
//             con.Dispose();
//             EntityManager.AddComponent(_clientConnectionGroup, ComponentType.ReadWrite<NetworkStreamDisconnected>());
//             
//             EntityManager.CreateEntity(ComponentType.ReadOnly(typeof(ClearClientGhostEntities.ClientClearGhosts)));
//             
//             // PostUpdateCommands.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<PrefabCreatorComplite>()));
//             // PostUpdateCommands.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<ItemComponent>()));
//             // PostUpdateCommands.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<GhostPrefabBuffer>()));
//             // PostUpdateCommands.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<GhostPrefabCollectionComponent>()));
//             // PostUpdateCommands.DestroyEntity(GetEntityQuery(ComponentType.ReadOnly<GhostComponent>()));
//         }
//         /*else if (ClientConnectionState == ConnectionState.TriggerTimeout && isConnected)
//         {
//             EntityManager.AddComponent(m_clientConnectionGroup, ComponentType.ReadWrite<NetworkStreamDisconnected>());
//         }*/
//         else if (ClientConnectionState == ConnectionState.TriggerConnect && !isConnected && _prevEndPoint.IsValid)
//         {
//             World.GetExistingSystem<NetworkStreamReceiveSystem>().Connect(_prevEndPoint);
//         }
//
//         // Update connection status
//         ClientConnectionState = isConnected
//             ? ConnectionState.Connected
//             : (_prevEndPoint.IsValid ? ConnectionState.NotConnected : ConnectionState.Uninitialized);
//     }
// }