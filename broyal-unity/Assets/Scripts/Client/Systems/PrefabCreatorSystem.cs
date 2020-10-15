using System;
using System.Collections.Generic;
using Bootstrappers;
using RemoteConfig;
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
    private AppConfig _appConfig => ServerBootstrapper.Container.Resolve<AppConfig>();
    protected override void OnCreate()
    {
        base.OnCreate();

        _config = ClientBootstrapper.Container.Resolve<MainConfig>();
        _group = GetEntityQuery(
            ComponentType.ReadOnly<PrefabCreator>(),
            ComponentType.Exclude<PrefabCreatorComplite>() );
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
                    var playerData = EntityManager.GetComponentData<PlayerData>(e);
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<Animator>(goEntity));
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<SkinnedMeshRenderer>(goEntity));
                    var healthBar = Find(go.Result, "HealthBar");
                    healthBar.gameObject.SetActive(true);
                    var animator = goManager.GetComponentObject<Animator>(goEntity);
                
                    EntityManager.AddComponentData(e, new CharacterPresenter());
                    EntityManager.AddComponentObject(e, animator);
                    EntityManager.AddComponentObject(e, healthBar.GetComponent<MeshRenderer>());

                    AttackWeapon(go.Result, playerData);
                }
                
                EntityManager.AddComponentObject(e, go.Result);
                //EntityManager.AddComponent<StateComponent>(e);

                //_map.Add(e, go.Result);
            };

            //PostUpdateCommands.Add(e,goManager.GetComponentObject<Animator>(goEntity));
            PostUpdateCommands.AddComponent<PrefabCreatorComplite>(e);
            //PostUpdateCommands.RemoveComponent<PrefabCreator>(e);
        }

        groupEntities.Dispose();
    }

    private void AttackWeapon(GameObject objResult, PlayerData playerData)
    {
        var bindData = objResult.GetComponent<SpineBindData>();
        if (bindData == null) Debug.Log("Unable to attach weapon", objResult);
        else
        {
            var skill = _appConfig.Skills[playerData.primarySkillId];
            var nameId = _config.GetNameId(skill.Id);
            var gameObject = _config.AssetRefMembers[nameId.Id].InstantiateAsync();

            gameObject.Completed += (go) =>
            {
                bindData.AttachWeapon(go.Result.transform);
            };
        }
    }

    private static Transform Find(GameObject root, string name)
    {
        foreach (var child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name) return child;
        }

        return null;
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

