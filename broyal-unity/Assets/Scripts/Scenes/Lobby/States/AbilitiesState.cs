
using System;
using System.Collections.Generic;
using System.Linq;
using Adic;
using Bootstrappers;
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.Factories;
using Scripts.Common.UI;
using Scripts.Core.Events;
using Scripts.Core.StateMachine;
using Scripts.Scenes.Lobby.UI;
using SocketIO.Data.Responses;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class AbilitiesState : BaseStateMachineState, ICustomEventListener
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private GameObject commonView;
        [SerializeField] private AbilityViewPopup abilityViewPopup;

        [SerializeField] private AbilitySelectedItem abilityMainIcon;
        [SerializeField] private Transform mainAbilityRoot;

        [SerializeField] private GameObject draggingBlock;
        [SerializeField] private GameObject mainAbilityLeftBlock;
        [SerializeField] private ScrollRect mainAbilityScrollRect;

        private GameObject skinPerson = null;
        private List<GameObject> curMainAbilities = new List<GameObject>();

        private GameObject leftBlockCopy = null;
        private AbilityDraggingItem draggedItem = null;
        private AbilityDraggingItem draggedItemCopy = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            CloseAbilityViewPopup();

            skinPerson = contentFactory.GetPlayerPerson();
            skinPerson.SetActive( false );

            SetupMainAbilities();
        }

        public override void OnEndState()
        {
            base.OnEndState();

            skinPerson.SetActive( true );
        }

        private void SetupMainAbilities()
        {
            ClearAbilities( curMainAbilities );

            abilityMainIcon.Setup( contentFactory.GetSpriteById( userData.GetCurrentCharacter().skill_set.main_skill ) );

            var appConfig = MainContainer.Container.Resolve<AppConfig>();
            var availableSkills = appConfig.Skills.Where( s => s.IsEnabled).ToList();

            foreach ( var skill in availableSkills )
            {
                AbilityDraggingItem newItem = contentFactory.CreateDraggingAbility( skill, mainAbilityRoot );
                newItem.SetOneListener( this );
                curMainAbilities.Add( newItem.gameObject );
            }
        }

        public void OnEvent( Component sender, string evName, EventArgs args )
        {
            if ( evName == AbilityDraggingItem.EVENT_DRAGGING_START )
            {
                SetDraggingMode( true, sender.gameObject );
            }
            else if ( evName == AbilityDraggingItem.EVENT_DRAGGING_END )
            {
                SetDraggingMode( false, sender.gameObject );
            }
            else if ( evName == AbilityDraggingItem.EVENT_SHOW_ABILITY_INFO )
            {
                AbilityEventArgs evArgs = ( AbilityEventArgs ) args;

                commonView.SetActive( false );
                abilityViewPopup.gameObject.SetActive( true );
                abilityViewPopup.Setup( evArgs.SkillInfo );
            }
        }

        private void SetDraggingMode( bool flag, GameObject draggingObj )
        {
            draggingBlock.SetActive( flag );

            if ( flag )
            {
                leftBlockCopy = Instantiate( mainAbilityLeftBlock, draggingBlock.transform );
                leftBlockCopy.GetComponent<RectTransform>().anchorMax = Vector2.one * 0.5f;
                leftBlockCopy.GetComponent<RectTransform>().anchorMin = Vector2.one * 0.5f;
                leftBlockCopy.GetComponent<RectTransform>().sizeDelta = mainAbilityLeftBlock.GetComponent<RectTransform>().sizeDelta;
                leftBlockCopy.transform.position = mainAbilityLeftBlock.transform.position;

                AbilitySelectedItem[] copySelItems = leftBlockCopy.GetComponentsInChildren<AbilitySelectedItem>();
                foreach ( var ab in copySelItems )
                {
                    ab.SetLighted( true );
                }

                EventSystem.current.SetSelectedGameObject( null );
                mainAbilityScrollRect.OnEndDrag( new PointerEventData( EventSystem.current ) );
                mainAbilityScrollRect.horizontal = false;

                draggedItem = draggingObj.GetComponent<AbilityDraggingItem>();
                draggedItem.SetDragMode( false );

                GameObject copyDraggedAbility = Instantiate( draggingObj, draggingBlock.transform );
                copyDraggedAbility.transform.position = draggingObj.transform.position;
                draggedItemCopy = copyDraggedAbility.GetComponent<AbilityDraggingItem>();
                draggedItemCopy.SetOneListener( this );
                draggedItemCopy.SetDragMode( true );
                EventSystem.current.SetSelectedGameObject( copyDraggedAbility );
            }
            else
            {
                float dist = Vector2.Distance( abilityMainIcon.transform.position, draggingObj.transform.position );
                //Debug.Log( "Dist: " + dist );
                if ( dist < 50f )
                {
                    // set new ability
                    SkillInfo draggingSkillInfo = draggedItem.GetMySkillInfo();
                    userData.SetSkill( draggingSkillInfo.Id );
                    abilityMainIcon.Setup( contentFactory.GetSpriteById( draggingSkillInfo.Id ) );
                }

                AbilitySelectedItem[] copySelItems = leftBlockCopy.GetComponentsInChildren<AbilitySelectedItem>();
                foreach ( var ab in copySelItems )
                {
                    ab.SetLighted( false );
                }

                GameObject.Destroy( leftBlockCopy.gameObject );
                GameObject.Destroy( draggingObj );
                EventSystem.current.SetSelectedGameObject( null );

                mainAbilityScrollRect.horizontal = true;
            }
        }

        public void OnPressedReplaceAbility( AbilityViewPopup popup )
        {
            CloseAbilityViewPopup();
        }

        public void OnPressedUpgradeAbility( AbilityViewPopup popup )
        {
            CloseAbilityViewPopup();
        }

        public void OnPressedGoToSettings()
        {
            stateMachine.SetState( ( int ) LobbyState.Settings );
        }

        public void OnPressedBackToMainmenu()
        {
            stateMachine.SetState( ( int ) LobbyState.Main );
        }

        private void CloseAbilityViewPopup()
        {
            abilityViewPopup.gameObject.SetActive( false );
            commonView.SetActive( true );
        }

        private void ClearAbilities( List<GameObject> list )
        {
            foreach ( var obj in list )
            {
                GameObject.Destroy( obj );
            }

            list.Clear();
        }
    }
}
