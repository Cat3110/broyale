using System;
using Unity.Entities;
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
    private float smoothPosition = 0.05f;
    private Vector3 velocity = Vector3.zero;

    //private EntityQuery currentPlayerQuery;

    private CameraSettings _cameraSettings;

    protected override void OnCreate()
    {
        base.OnCreate();

        _cameraSettings = Bootstrappers.ClientBootstrapper.Container.Resolve<CameraSettings>();

        //currentPlayerQuery = GetEntityQuery(typeof(MovableCharacterComponent), typeof(PlayerInput), typeof(CharacterPresenter));
        //RequireForUpdate(currentPlayerQuery);
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerInput>().WithStructuralChanges()
            .ForEach((Entity entity, in Translation translation, in MovableCharacterComponent movable,
                in CharacterPresenter characterPresenter) =>
            {
                MoveCamera(translation);
            }).Run();
    }

    private void MoveCamera(Translation localToWorld)
    {
        var camera = Camera.main;
        if(camera == null ) throw new Exception("Dont have Main Camera");
        
        var cameraBounds = _cameraSettings.Bounds;

        var targetX = 0.0f;

        if (localToWorld.Value.x > cameraBounds.xMax)
            targetX = (localToWorld.Value.x - cameraBounds.xMax);
        else if (localToWorld.Value.x < cameraBounds.xMin)
            targetX = (localToWorld.Value.x - cameraBounds.xMin);

        var targetZ = 0.0f;

        if (localToWorld.Value.z > cameraBounds.yMax)
            targetZ = (localToWorld.Value.z - cameraBounds.yMax);
        else if (localToWorld.Value.z < cameraBounds.yMin)
            targetZ = (localToWorld.Value.z - cameraBounds.yMin);

        var position = camera.transform.position;
        targetPosition = new Vector3(targetX, position.y, targetZ);
        position = Vector3.SmoothDamp(position, targetPosition, ref velocity, smoothPosition);
        camera.transform.position = position;

        //camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition,
        //    UnityEngine.Time.deltaTime * smoothPosition);
    }
}

[DisableAutoCreation]
public class UpdateUISystem : SystemBase
{
    private UIController _uiController;
    protected override void OnCreate()
    {
        base.OnCreate();

        _uiController = Bootstrappers.ClientBootstrapper.Container.Resolve<UIController>();
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerInput>().WithStructuralChanges()
            .ForEach((Entity entity, in Translation translation, in PlayerData playerData, in MovableCharacterComponent movable,
                in CharacterPresenter characterPresenter) =>
            {
                _uiController.GameUI.SetHealth(playerData.health);
            }).Run();
    }
}
