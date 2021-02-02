
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

        [SerializeField] private AbilitySelectedItem[] activeAbilityIcons;
        [SerializeField] private Transform activeAbilitiesRoot;

        [SerializeField] private GameObject draggingBlock;
        [SerializeField] private GameObject mainAbilityLeftBlock;
        [SerializeField] private GameObject activeAbilityLeftBlock;

        private GameObject skinPerson = null;
        private List<GameObject> curMainAbilities = new List<GameObject>();
        private List<GameObject> curActiveAbilities = new List<GameObject>();

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
            SetupActiveAbilities();
        }

        public override void OnEndState()
        {
            base.OnEndState();

            skinPerson.SetActive( true );
        }

        private void SetupActiveAbilities()
        {
            ClearAbilities( curActiveAbilities );

            var activeSkills = userData.GetCurrentCharacter().skill_set.active_skills;
            for ( int i = 0; i < activeAbilityIcons.Length && i < activeSkills.Length; i++ )
            {
                activeAbilityIcons[ i ].Setup( contentFactory.GetSpriteById( activeSkills[ i ] ) );
            }


            var appConfig = MainContainer.Container.Resolve<AppConfig>();
            var availableSkills = appConfig.Skills.Where( s => s.IsEnabled && s.Type != SkillType.Main && s.Type != SkillType.Passive ).ToList();

            foreach ( var skill in availableSkills )
            {
                AbilityDraggingItem newItem = contentFactory.CreateDraggingAbility( skill, activeAbilitiesRoot );
                newItem.SetOneListener( this );
                curActiveAbilities.Add( newItem.gameObject );
            }
        }

        private void SetupMainAbilities()
        {
            ClearAbilities( curMainAbilities );

            abilityMainIcon.Setup( contentFactory.GetSpriteById( userData.GetCurrentCharacter().skill_set.main_skill ) );

            var appConfig = MainContainer.Container.Resolve<AppConfig>();
            var availableSkills = appConfig.Skills.Where( s => s.IsEnabled && s.Type == SkillType.Main ).ToList();
            
            //TODO: just for test
            userData.SetSkillConfig(appConfig);

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

                bool canReplace = CanReplaceThisAbility( evArgs.SkillInfo );
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

        private bool CanReplaceThisAbility( SkillInfo skillInfo )
        {
            if ( skillInfo.Type == SkillType.Main )
            {
                return userData.GetCurrentCharacter().skill_set.main_skill != skillInfo.Id;
            }

            return ! userData.GetCurrentCharacter().skill_set.active_skills.Contains( skillInfo.Id );
        }

        private void LightSkillsForSelect( SkillInfo skillInfo, GameObject senderObj )
        {
            CloseAbilityViewPopup();

            draggingBlock.SetActive( true );

            var leftAbilitiesBlock = GetLeftAbilitiesBlock( skillInfo );
            var leftAbilitiesSourceItems = GetLeftSourceItems( skillInfo );

            leftBlockCopy = Instantiate( leftAbilitiesBlock, draggingBlock.transform );
            leftBlockCopy.GetComponent<RectTransform>().anchorMax = Vector2.one * 0.5f;
            leftBlockCopy.GetComponent<RectTransform>().anchorMin = Vector2.one * 0.5f;
            leftBlockCopy.GetComponent<RectTransform>().sizeDelta = mainAbilityLeftBlock.GetComponent<RectTransform>().sizeDelta;
            leftBlockCopy.transform.position = leftAbilitiesBlock.transform.position;

            AbilitySelectedItem[] copySelItems = leftBlockCopy.GetComponentsInChildren<AbilitySelectedItem>();
            for ( int i = 0; i < copySelItems.Length && i < leftAbilitiesSourceItems.Length; i++ )
            {
                var ab = copySelItems[ i ];

                ab.SetSourceItem( leftAbilitiesSourceItems[ i ], i );
                ab.SetOneListener( this );
                ab.SetLighted( true );
            }

            GameObject copyDraggedAbility = Instantiate( senderObj, draggingBlock.transform );
            copyDraggedAbility.transform.position = senderObj.transform.position;
            rightItemCopy = copyDraggedAbility.GetComponent<AbilityDraggingItem>();
            rightItemCopy.SetOneListener( this );

            selectedSkillInfo = skillInfo;
        }

        private AbilitySelectedItem[] GetLeftSourceItems( SkillInfo skillInfo )
        {
            if ( skillInfo.Type == SkillType.Main ) return new AbilitySelectedItem[] { abilityMainIcon };

            return activeAbilityIcons;
        }

        private GameObject GetLeftAbilitiesBlock( SkillInfo skillInfo )
        {
            if ( skillInfo.Type == SkillType.Main ) return mainAbilityLeftBlock;

            return activeAbilityLeftBlock;
        }

        private void ApplyAbilityTo( AbilitySelectedItem selItem )
        {
            userData.SetSkillByIndex( selectedSkillInfo.Id, selItem.SourceItemIndex );
            selItem.SourceItem.Setup( contentFactory.GetSpriteById( selectedSkillInfo.Id ) );

            GameObject.Destroy( leftBlockCopy ); leftBlockCopy = null;
            GameObject.Destroy( rightItemCopy.gameObject ); rightItemCopy = null;
            draggingBlock.SetActive( false );
        }

        public void OnPressedUpgradeAbility( AbilityViewPopup popup )
        {
            CloseAbilityViewPopup();
        }

        public void OnPressedClosePopup()
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
