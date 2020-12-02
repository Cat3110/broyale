using System;
using System.Collections.Generic;
using Bootstrappers;
using RemoteConfig;
using Scripts.Common.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Utils;

[DisableAutoCreation]
public class PrefabCreatorSystem : ComponentSystem
{
    private EntityQuery _group;
    private AppConfig  _appConfig;
    private GameObject _player;

    private Dictionary<Entity, GameObject> _map = new Dictionary<Entity, GameObject>();
    private IAssetManager _assetManager;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        _appConfig = ServerBootstrapper.Container.Resolve<AppConfig>();
        _assetManager = ClientBootstrapper.Container.Resolve<IAssetManager>();
        
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
            
            _assetManager.LoadAssetByIdAsync(data.NameId, (go) =>
            {
                var translate = EntityManager.GetComponentData<Translation>(e);
                var goEntity = go.GetComponent<GameObjectEntity>().Entity;
                var goManager = go.GetComponent<GameObjectEntity>().EntityManager;
                go.transform.position = translate.Value;

                goManager.AddComponentData(goEntity, new Link{entity = e});

                var isCharacter = !EntityManager.HasComponent<ItemComponent>(e);

                if (isCharacter)
                {
                    var skinSetting = data.SkinSetting.ToString();
                    var haveSkinSetting = !string.IsNullOrEmpty(skinSetting);
                    
                    var playerData = EntityManager.GetComponentData<PlayerData>(e);
                    var bindData = go.GetComponent<CharactersBindData>();
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<Animator>(goEntity));
                    //EntityManager.AddComponentObject(e,goManager.GetComponentObject<SkinnedMeshRenderer>(goEntity));
                    
                    bindData.SetHealthBarType(CharactersBindData.HealthBarType.Friendly);
                    bindData.SetSkinType(data.SkinId);
                    
                    if(haveSkinSetting) bindData.SetSkinData( new CurrentSkinData(skinSetting) );

                    EntityManager.AddComponentData(e, new CharacterPresenter());
                    EntityManager.AddComponentObject(e, bindData);

                    AttachWeapon(go, playerData);
                }
                
                EntityManager.AddComponentObject(e, go);
            });

            //PostUpdateCommands.Add(e,goManager.GetComponentObject<Animator>(goEntity));
            PostUpdateCommands.AddComponent<PrefabCreatorComplite>(e);
            //PostUpdateCommands.RemoveComponent<PrefabCreator>(e);
        }

        groupEntities.Dispose();
    }

    private void AttachWeapon(GameObject objResult, PlayerData playerData)
    {
        var bindData = objResult.GetComponent<CharactersBindData>();
        if (bindData == null) Debug.Log("Unable to attach weapon", objResult);
        else
        {
            var skill = _appConfig.Skills[playerData.primarySkillId];
            _assetManager.LoadAssetByNameAsync(skill.Id,
                (go) =>
                {
                    bindData.AttachWeapon(go);
                }
            );
        }
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

