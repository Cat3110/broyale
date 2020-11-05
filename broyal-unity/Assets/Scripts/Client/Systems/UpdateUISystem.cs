using Unity.Entities;
using Unity.Transforms;

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