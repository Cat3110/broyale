// When server receives go in game request, go in game and delete request
using System;
using System.Linq;
using Bootstrappers;
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.Data.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class PlayerSpawnServerSystem : ComponentSystem
{
    private EntityQuery _query;
    private AppConfig _appConfig => ServerBootstrapper.Container.Resolve<AppConfig>();
    private MainConfig _mainConfig => BaseBootStrapper.Container.Resolve<MainConfig>();
    private IGlobalSession _globalSession => MainContainer.Container.Resolve<IGlobalSession>();
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableDOTSGhostSendSystemComponent>();
        _query = GetEntityQuery(
            ComponentType.ReadWrite<SpawnPoint>(),
            ComponentType.ReadOnly<Translation>());
    }
    
    protected override void OnUpdate()
    {
        var spawners = _query.ToEntityArray(Allocator.TempJob);
        
        Entities.WithNone<SendRpcCommandRequestComponent>()
            .ForEach((Entity reqEnt, ref PlayerSpawnRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
            {
                var characterInfo  = _appConfig.Characters[0];
                
                var userId = req.userId.ToString();
                
                var mainSkillId = req.skillId;
                var attackSkillId = req.skill2Id;
                var defenceSkillId = req.skill3Id;
                var utilsSkillId = req.skill4Id;
                
                var characterNameId = req.characterId;
                var skinId = req.skinId;
                NativeString64 skinSetting = "";

                if (!string.IsNullOrEmpty(userId))
                {
                    var character = _globalSession.CharactersInGame.First(c => c.user_id == userId);
                    var skinData = new CurrentSkinData(character);
                    
                    mainSkillId = (short)_appConfig.Skills.FindIndex(s => s.Id == character.skill_set.main_skill);
                    attackSkillId = (short)_appConfig.Skills.FindIndex(s => s.Id == character.skill_set.attack_skill);
                    defenceSkillId = (short)_appConfig.Skills.FindIndex(s => s.Id == character.skill_set.defence_skill);
                    utilsSkillId = (short)_appConfig.Skills.FindIndex(s => s.Id == character.skill_set.utils_skill);
                    
                    characterNameId = _mainConfig.GetNameId(character.sex == "male" ? "ID_MALE" : "ID_FEMALE").Id;
                    skinSetting = new CurrentSkinData(character).ToString();
                }
                
                
                UnityEngine.Debug.Log(
                    $"Server setting connection {EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value} to in game");
                
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);

                var ghostCollection = GetSingleton<GhostPrefabCollectionComponent>();
                
                var ghostId = DOTSGhostSerializerCollection.FindGhostType<CharacterSnapshotData>();
                var prefab = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection.serverPrefabs)[ghostId].Value;
                var player = EntityManager.Instantiate(prefab);

                var spawnPoint = spawners.OrderBy(s => EntityManager.GetComponentData<SpawnPoint>(s).spawnCount)
                    .First();
                var spawnComponent =  EntityManager.GetComponentData<SpawnPoint>(spawnPoint);
                var spawnPosition = EntityManager.GetComponentData<Translation>(spawnPoint);
                
                EntityManager.SetComponentData(spawnPoint, 
                    new SpawnPoint
                    {
                        radius = spawnComponent.radius,
                        spawnCount = spawnComponent.spawnCount + 1
                    });
                
                EntityManager.SetComponentData(player, new Translation{ Value = spawnPosition.Value});
                EntityManager.SetComponentData(player, new PlayerData{ 
                    maxHealth = characterInfo.Health, 
                    health = characterInfo.Health,
                    power = characterInfo.Power,
                    magic = characterInfo.Magic,
                    primarySkillId = mainSkillId,
                    attackSkillId = attackSkillId,
                    defenceSkillId = defenceSkillId,
                    utilsSkillId = utilsSkillId,
                    speedMod = 1.0f
                });
                EntityManager.SetComponentData(player, new PrefabCreator
                {
                    NameId = characterNameId,
                    SkinId = skinId, 
                    SkinSetting = skinSetting
                });
                
                EntityManager.SetComponentData(player, 
                    new MovableCharacterComponent
                    {
                        PlayerId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value
                    });

                PostUpdateCommands.AddBuffer<PlayerInput>(player);
                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });
                PostUpdateCommands.DestroyEntity(reqEnt);
            });
        
        spawners.Dispose();
    }
}