
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
using UnityEngine;
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
        private AbilityDraggingItem rightItemCopy = null;
        private SkillInfo selectedSkillInfo;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            leftBlockCopy = null;
            rightItemCopy = null;
            abilityViewPopup.SetOneListener( this );

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
            if ( evName == AbilitySelectedItem.EVENT_TAP_LIGHTED_ABILITY && leftBlockCopy == null )
            {
                AbilityDraggingEventArgs evArgs = ( AbilityDraggingEventArgs ) args;

                bool canReplace = userData.GetCurrentCharacter().skill_set.main_skill != evArgs.SkillInfo.Id;
                commonView.SetActive( false );
                abilityViewPopup.gameObject.SetActive( true );
                abilityViewPopup.Setup( evArgs.SkillInfo, sender.gameObject, canReplace );
            }
            else if ( evName == AbilityViewPopup.EVENT_APPLY_ABILITY )
            {
                AbilityViewEventArgs evArgs = ( AbilityViewEventArgs ) args;
                LightSkillsForSelect( evArgs.SkillInfo, evArgs.abilityObj );
            }
            else if ( evName == AbilitySelectedItem.EVENT_TAP_LIGHTED_ABILITY && leftBlockCopy != null )
            {
                if ( rightItemCopy.gameObject != sender.gameObject )
                {
                    ApplyAbilityTo( sender as AbilitySelectedItem );
                }
            }
        }

        private void LightSkillsForSelect( SkillInfo skillInfo, GameObject senderObj )
        {
            CloseAbilityViewPopup();

            draggingBlock.SetActive( true );

            leftBlockCopy = Instantiate( mainAbilityLeftBlock, draggingBlock.transform );
            leftBlockCopy.GetComponent<RectTransform>().anchorMax = Vector2.one * 0.5f;
            leftBlockCopy.GetComponent<RectTransform>().anchorMin = Vector2.one * 0.5f;
            leftBlockCopy.GetComponent<RectTransform>().sizeDelta = mainAbilityLeftBlock.GetComponent<RectTransform>().sizeDelta;
            leftBlockCopy.transform.position = mainAbilityLeftBlock.transform.position;

            AbilitySelectedItem[] copySelItems = leftBlockCopy.GetComponentsInChildren<AbilitySelectedItem>();
            foreach ( var ab in copySelItems )
            {
                ab.SetOneListener( this );
                ab.SetLighted( true );
            }

            GameObject copyDraggedAbility = Instantiate( senderObj, draggingBlock.transform );
            copyDraggedAbility.transform.position = senderObj.transform.position;
            rightItemCopy = copyDraggedAbility.GetComponent<AbilityDraggingItem>();
            rightItemCopy.SetOneListener( this );

            selectedSkillInfo = skillInfo;
        }

        private void ApplyAbilityTo( AbilitySelectedItem selItem )
        {
            userData.SetSkill( selectedSkillInfo.Id );
            abilityMainIcon.Setup( contentFactory.GetSpriteById( selectedSkillInfo.Id ) );

            GameObject.Destroy( leftBlockCopy ); leftBlockCopy = null;
            GameObject.Destroy( rightItemCopy.gameObject ); rightItemCopy = null;
            draggingBlock.SetActive( false );
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
