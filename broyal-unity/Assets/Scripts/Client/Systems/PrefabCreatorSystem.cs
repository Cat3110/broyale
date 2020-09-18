using System;
using System.Collections.Generic;
using Bootstrappers;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
public class PrefabCreatorSystem : ComponentSystem
{
    private EntityQuery _group;
    private MainConfig _config;
    private GameObject _player;

    private Dictionary<Entity, GameObject> _map = new Dictionary<Entity, GameObject>();

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
            var gameObject = _config.AssetRefMembers[data.NameId].InstantiateAsync();

            gameObject.Completed += (go) =>
            {
                //Debug.Log($"InstantiateAsync => {data.NameId} {go.Result}");
                var translate = EntityManager.GetComponentData<Translation>(e);
                var goEntity = go.Result.GetComponent<GameObjectEntity>().Entity;
                var goManager = go.Result.GetComponent<GameObjectEntity>().EntityManager;
                go.Result.transform.position = translate.Value;

                goManager.AddComponentData(goEntity, new Link{entity = e});

                if (!EntityManager.HasComponent<ItemComponent>(e))
                {
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<Animator>(goEntity));
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<SkinnedMeshRenderer>(goEntity));
                    var healthBar = go.Result.transform.Find("HealthBar");
                    var animator = goManager.GetComponentObject<Animator>(goEntity);
                
                    EntityManager.AddComponentData(e, new CharacterPresenter());
                    EntityManager.AddComponentObject(e, animator);
                    EntityManager.AddComponentObject(e, healthBar.GetComponent<MeshRenderer>());
                }
                
                EntityManager.AddComponentObject(e, go.Result);
                //EntityManager.AddComponent<StateComponent>(e);

                //_map.Add(e, go.Result);
            };

            //PostUpdateCommands.Add(e,goManager.GetComponentObject<Animator>(goEntity));
            PostUpdateCommands.RemoveComponent<PrefabCreator>(e);
        }

        groupEntities.Dispose();
    }
}

[DisableAutoCreation]
public class ClearingPresenterSystem : ComponentSystem
{
    private EntityQuery _group;
    private MainConfig _config;
    private GameObject _player;
    private World _connectedWorld;
    
    protected override void OnCreate()
    {
        base.OnCreate();

        _config = ClientBootstrapper.Container.Resolve<MainConfig>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, ref Link link) =>
        {
            if (_connectedWorld.EntityManager.Exists(link.entity)) return;
            
            var go = EntityManager.GetComponentObject<Transform>(e);
            GameObject.Destroy(go.gameObject);
            PostUpdateCommands.DestroyEntity(e);
        });
    }

    public void Init(World world)
    {
        _connectedWorld = world;
    }
}

