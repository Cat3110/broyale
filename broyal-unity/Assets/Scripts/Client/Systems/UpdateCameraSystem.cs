using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[Serializable]
public class CameraSettings
{
    public Rect Bounds;
}

[DisableAutoCreation]
public class UpdateCameraSystem : SystemBase
{
    private Vector3 targetPosition;
    private float smoothPosition = 0.1f;
    private Vector3 velocity = Vector3.zero;

    //private EntityQuery currentPlayerQuery;

    private Camera _mainCamera;

    private CameraSettings _cameraSettings;

    protected override void OnCreate()
    {
        base.OnCreate();

        _cameraSettings = Bootstrappers.ClientBootstrapper.Container.Resolve<CameraSettings>();
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError($"UpdateCameraSystem: Dont have main camera on scene");
            Enabled = false;
        }

        //currentPlayerQuery = GetEntityQuery(typeof(MovableCharacterComponent), typeof(PlayerInput), typeof(CharacterPresenter));
        //RequireForUpdate(currentPlayerQuery);
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerInput>().WithStructuralChanges()
            .ForEach((Entity entity, in GameObject go, in Translation translation, in MovableCharacterComponent movable, in CharacterPresenter characterPresenter) =>
            {
                MoveMainCamera(_mainCamera, go.transform.position, _cameraSettings.Bounds);
            }).Run();
    }

    private void MoveMiniMapCamera(Camera camera, float3 position)
    {
        var transform = camera.transform;
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    private void MoveMainCamera(Camera camera, float3 newPosition, Rect bounds)
    {
        var targetX = 0.0f;
        var targetZ = 0.0f;
        
        if (newPosition.x > bounds.xMax)
            targetX = (newPosition.x - bounds.xMax);
        else if (newPosition.x < bounds.xMin)
            targetX = (newPosition.x - bounds.xMin);

        if (newPosition.z > bounds.yMax)
            targetZ = (newPosition.z - bounds.yMax);
        else if (newPosition.z < bounds.yMin)
            targetZ = (newPosition.z - bounds.yMin);

        var position = camera.transform.position;
        targetPosition = new Vector3(targetX, position.y, targetZ);
        position = Vector3.SmoothDamp(position, targetPosition, ref velocity, smoothPosition);
        camera.transform.position = position;

        //camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition,
        //    UnityEngine.Time.deltaTime * smoothPosition);
    }
}