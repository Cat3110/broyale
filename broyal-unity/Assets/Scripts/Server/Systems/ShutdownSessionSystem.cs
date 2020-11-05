#if !UNITY_EDITOR 
#define NOT_UNITY_EDITOR
#endif

using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class ShutdownSessionSystem : ComponentSystem
{
    private EntityQuery _groupConnected;
    private EntityQuery _groupDisconnected;
    
    protected override void OnCreate()
    {
        _groupConnected = GetEntityQuery(ComponentType.ReadOnly<NetworkStreamConnection>());
        _groupDisconnected = GetEntityQuery(ComponentType.ReadOnly<NetworkStreamDisconnected>());
    }

    protected override void OnUpdate()
    {
        var connected = _groupConnected.ToEntityArray(Allocator.TempJob);
        var disconnected = _groupDisconnected.ToEntityArray(Allocator.TempJob);
       
        if (connected.Length == 1 && disconnected.Length == 1)
        {
            Debug.LogWarning($"ShutdownSessionSystem : ShutDown");
            ShutdownServer();
        }
        
        connected.Dispose();
        disconnected.Dispose();
    }

    [System.Diagnostics.Conditional("NOT_UNITY_EDITOR")]
    private void ShutdownServer()
    {
        Application.Quit();
    }
}