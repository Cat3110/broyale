using Unity.Entities;
using Unity.Rendering;
 
namespace Systems.Initialization {
    /// <summary>
    /// Suppresses the error: "ArgumentException: A component with type:BoneIndexOffset has not been added to the entity.", until the Unity bug is fixed.
    /// </summary>
    // [UpdateInGroup(typeof(InitializationSystemGroup))]
    // public class DisableCopySkinnedEntityDataToRenderEntitySystem : ComponentSystem {
    //     protected override void OnCreate() {
    //         var copySkinnedSystem = World.GetOrCreateSystem<CopySkinnedEntityDataToRenderEntity>();
    //         if (copySkinnedSystem != null)
    //         {
    //             copySkinnedSystem.Enabled = false;
    //         }
    //         Enabled = false;
    //         World.DestroySystem(this);
    //     }
    //     
    //     protected override void OnUpdate() => throw new System.NotImplementedException();
    // }
}

