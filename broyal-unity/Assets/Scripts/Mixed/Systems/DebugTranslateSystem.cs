#if UNITY_EDITOR

using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Mixed.Systems
{
    [UpdateInGroup(typeof(ClientAndServerSimulationSystemGroup), OrderLast = true)]
    public class DebugTranslateSystem : SystemBase
    {
        private bool _isServer;

        protected override void OnCreate()
        {
            base.OnCreate();
            _isServer = World.GetExistingSystem<ServerSimulationSystemGroup>() != null;
        }

        protected override void OnUpdate()
        {
            var predictionGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
            var tick = predictionGroup.PredictingTick;
            var deltaTime = Time.DeltaTime;
            var last = predictionGroup.IsFinalPredictionTick;

            Entities.ForEach((ref Entity e, ref Translation position, ref MovableCharacterComponent mv) =>
            {
                Grapher.Log((Vector2)position.Value.xz,$"{ (_isServer ? "Server" : "Client") } {e.Index}", (float)Time.ElapsedTime);
            }).WithoutBurst().Run();
        }
    }
}
#endif 
