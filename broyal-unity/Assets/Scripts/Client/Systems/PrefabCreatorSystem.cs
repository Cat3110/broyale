using System;
using Bootstrappers;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[DisableAutoCreation]
public class PrefabCreatorSystem : ComponentSystem
{
    private EntityQuery _group;
    private MainConfig _config;
    private GameObject _player;

    protected override void OnCreate()
    {
        base.OnCreate();

        _config = ClientBootstrapper.Container.Resolve<MainConfig>();
        _group = GetEntityQuery(ComponentType.ReadOnly<PrefabCreator>());
    }

    protected override void OnUpdate()
    {
        var groupEntities = _group.ToEntityArray(Allocator.TempJob);

        foreach (var e in groupEntities)
        {
            var data = EntityManager.GetComponentData<PrefabCreator>(e);
            var gameObject = _config.AssetRefMembers[data.NameId.Id].InstantiateAsync();

            gameObject.Completed += (go) =>
            {
                var goEntity = go.Result.GetComponent<GameObjectEntity>().Entity;
                var goManager = go.Result.GetComponent<GameObjectEntity>().EntityManager;

                //EntityManager.AddComponentObject(e,goManager.GetComponentObject<Animator>(goEntity));
                //EntityManager.AddComponentObject(e,goManager.GetComponentObject<SkinnedMeshRenderer>(goEntity));
                var healthBar = go.Result.transform.Find("HealthBar");
                
                EntityManager.AddComponentData(e, new CharacterPresenter());
                EntityManager.AddComponentObject(e, goManager.GetComponentObject<Animator>(goEntity));
                EntityManager.AddComponentObject(e, healthBar.GetComponent<MeshRenderer>());
                EntityManager.AddComponentObject(e, go.Result);
            };

            //PostUpdateCommands.Add(e,goManager.GetComponentObject<Animator>(goEntity));
            PostUpdateCommands.RemoveComponent<PrefabCreator>(e);
        }

        groupEntities.Dispose();
    }
}

