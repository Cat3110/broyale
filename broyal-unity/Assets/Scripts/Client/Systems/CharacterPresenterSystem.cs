using System.Linq;
using Bootstrappers;
using RemoteConfig;
using UniRx.Async.Triggers;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct CharacterPresenter : IComponentData
{
    public int Id;
    public int AttackTransId;
    public int DamageTransId;
}
public struct Link : IComponentData
{
    public Entity entity;
}
public struct StateComponent : ISystemStateComponentData
{
    public int State;
}

[DisableAutoCreation]
public class CharacterPresenterSystem : ComponentSystem
{
    private const float RotationSpeed = 10.0f;

    private EntityQuery _group;

    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    private static readonly int DamageTrigger = Animator.StringToHash("DamageTrigger");
    private EntityQuery _otherPlayers;
    private static readonly int Health = Animator.StringToHash("Health");
    private static readonly int Death = Animator.StringToHash("Death");
    
    private static readonly int Type = Animator.StringToHash("Type");

    private MainConfig _config;
    private FXData _fxData;
    private UIController _uiController;
    protected override void OnCreate()
    {
        base.OnCreate();

        _config = ClientBootstrapper.Container.Resolve<MainConfig>();
        _fxData = ClientBootstrapper.Container.Resolve<FXData>();
        _uiController = ClientBootstrapper.Container.Resolve<UIController>();
        
        _group = GetEntityQuery(
            ComponentType.ReadWrite<CharacterPresenter>(),
            ComponentType.ReadOnly<GameObject>(),
            ComponentType.ReadOnly<CharactersBindData>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<Attack>(),
            ComponentType.ReadOnly<Damage>(),
            ComponentType.ReadOnly<PlayerData>(),
            ComponentType.Exclude<DEAD>()
        );

        _otherPlayers = GetEntityQuery(
            ComponentType.ReadWrite<PlayerData>(),
            ComponentType.ReadOnly<Damage>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.Exclude<PlayerInput>(),
            ComponentType.Exclude<DEAD>()
        );
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var groupEntities = _group.ToEntityArray(Allocator.TempJob);
        var otherPlayer = _otherPlayers.ToEntityArray(Allocator.TempJob);

        foreach (var e in groupEntities)
        {
            var player = EntityManager.GetComponentData<PlayerData>(e);

            _fxData.SetDeadZoneRadius(player.damageRadius);
            
            var data = EntityManager.GetComponentData<CharacterPresenter>(e);
            var translation = EntityManager.GetComponentData<Translation>(e);

            var go = EntityManager.GetComponentObject<GameObject>(e);
            var bindData = EntityManager.GetComponentObject<CharactersBindData>(e);
            
            float3 prevPos = go.transform.position;
            var dist = math.distance(prevPos, translation.Value);

            bindData.Animator.SetFloat(Speed, dist > 0.2f ? 1.0f : 0.0f);

            go.transform.position = Vector3.Lerp(go.transform.position, translation.Value, deltaTime * 9.0f);

            _uiController.SetPlayerGo(go);
            _uiController.SetPlayerPosition(translation.Value);
            _uiController.GameUI.SetGems(player.GetItems());
            
            bindData.UpdateHealthBar(player.health / (float)player.maxHealth);
            
            //if (dist > 0.1f)
            {
                //go.transform.forward = direction;
                //go.transform.forward = Vector3.Lerp(go.transform.forward, direction, RotationSpeed * deltaTime);
            }

            // if (EntityManager.HasComponent<PlayerInput>(e))
            // {
            //     var input = EntityManager.GetBuffer<PlayerInput>(e);
            //     var lastInput = input[0];
            //     var direction = new Vector2(lastInput.horizontal,lastInput.vertical);
            //     bindData.Animator.SetFloat(Speed, direction.sqrMagnitude > 0.01 ? 1 : 0);
            // }

            var attack = EntityManager.GetComponentData<Attack>(e);
            var damage = EntityManager.GetComponentData<Damage>(e);

            if (attack.ProccesedId != 0 || attack.AttackType != 0)
            {
                if (data.AttackTransId != attack.Seed)
                {
                    Debug.LogWarning($"Client:Attack To => {attack.Target} => {attack.AttackType} => {data.AttackTransId} != {attack.Seed}");
                    bindData.Animator.SetInteger(Type, player.primarySkillId);
                    bindData.Animator.SetTrigger(AttackTrigger);
                    data.AttackTransId = attack.Seed;
                    EntityManager.SetComponentData(e, data);

                    Vector3 attackDirection = new Vector3(attack.AttackDirection.x,0,attack.AttackDirection.y);
                    if (attackDirection.sqrMagnitude < 0.01f)
                    {
                        attackDirection = go.transform.forward;
                    }
                    
                    _fxData.Start(player.primarySkillId, go, bindData.Weapom.transform, attackDirection);
                }//else  Debug.LogWarning($"Client:Attack To => {e} => {data.AttackTransId}{attack.Seed}");
                
                var target = attack.Target;
                if (target != Entity.Null && player.primarySkillId < 2 )
                {
                    var attackDirection = EntityManager.GetComponentData<Translation>(target).Value - translation.Value;
                    go.transform.forward = Vector3.Lerp(go.transform.forward, math.normalize(attackDirection), RotationSpeed * Time.DeltaTime);
                    //go.transform.forward = math.normalize(lookdirection);
                }
                else
                {
                    Vector3 attackDirection = new Vector3(attack.AttackDirection.x,0,attack.AttackDirection.y);
                    if (attackDirection.sqrMagnitude > 0.1)
                    {
                        go.transform.forward = Vector3.Lerp(go.transform.forward, attackDirection, RotationSpeed * deltaTime);
                    }
                }
            }
            else
            {
                // if (EntityManager.HasComponent<PlayerInput>(e))
                // {
                //     var input = EntityManager.GetBuffer<PlayerInput>(e);
                //     var lastInput = input[0];
                //     var direction = new Vector2(lastInput.horizontal,lastInput.vertical);
                //     if (direction.sqrMagnitude > 0.1f)
                //     {
                //         go.transform.forward = Vector3.Lerp(go.transform.forward, direction, RotationSpeed * deltaTime);
                //         //go.transform.forward = direction;
                //     }
                //     
                //     Debug.Log( $"{input[0].horizontal}:{input[0].vertical} => {input[input.Length-1].horizontal}:{input[input.Length-1].vertical}");
                // }
                // else
                // {
                    if (dist > 0.01f)
                    {
                        var direction = math.normalize(translation.Value - prevPos);
                        go.transform.forward = Vector3.Lerp(go.transform.forward, direction, RotationSpeed * deltaTime);
                    }
                //}
            }

            if (damage.DamageType != 0 && data.DamageTransId != damage.Seed )
            {
                Debug.LogWarning($"Client:Damage To => {e} => {damage.DamageType}");
                bindData.Animator.SetTrigger(DamageTrigger);
                data.DamageTransId = damage.Seed;
                EntityManager.SetComponentData(e, data);
            }

            bindData.Animator.SetBool(Death, player.health <= 0.0f);
        }

        groupEntities.Dispose();
        otherPlayer.Dispose();
    }
}

// if (attack.HaveTransition())
// {
// if (math.abs(data.TransId - attack.Seed) > 0.01f)
// {
//     //Debug.Log($"Client:Attack => {e} => {attack.Type} => {attack.TransHash}");
//     animator.SetTrigger(AttackTrigger);
//     data.TransId = attack.Seed;
//     EntityManager.SetComponentData(e, data);
// }
//
// if (math.abs(1.0f - attack.Type.x) < 0.01f)
// {
//     var distance = 10.0f;
//     var entity = otherPlayer.FirstOrDefault(x =>
//         math.distancesq(translation.Value, EntityManager.GetComponentData<Translation>(x).Value) < distance * 2);
//
//     if (entity != Entity.Null)
//     {
//         var lookdirection = EntityManager.GetComponentData<Translation>(entity).Value - translation.Value;
//         go.transform.forward = Vector3.Lerp(go.transform.forward, math.normalize(lookdirection), RotationSpeed * Time.DeltaTime);
//     }
// }
// }