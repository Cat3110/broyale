
using Adic;
using Scripts.Common.Data;
using Scripts.Common.Factories;
using Scripts.Core.StateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class SettingsState : BaseStateMachineState
    {
        public enum PanelState
        {
            Common,
            ChangeLanguage,
            TermsOfUse,
            FBConnect
        }

        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private Button[] musicButtons;
        [SerializeField] private Button[] soundButtons;
        [SerializeField] private Button[] fbButtons;
        [SerializeField] private GameObject[] commonBlocks; // main and change lang

        private int prevState = -1;
        private GameObject skinPerson = null;

        private int iMusicState = 0;
        private int iSoundState = 1;
        private int iFBState = 0;
        private PanelState panelState = PanelState.Common;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            prevState = stateMachine.PrevState;

            skinPerson = contentFactory.GetPlayerPerson();
            skinPerson.SetActive( false );

            UpdateButtons();
        }

        public override void OnEndState()
        {
            base.OnEndState();

            skinPerson.SetActive( true );
        }

        public void OnPressedShowTermsOfService()
        {
            panelState = PanelState.TermsOfUse;
            UpdateButtons();
        }

        public void OnPressedCloseTermsOfService()
        {
            panelState = PanelState.Common;
            UpdateButtons();
        }

        public void OnPressedGoToMain()
        {
            stateMachine.SetState( prevState );
        }

        public void OnPressedButtonMusic( int i )
        {
            iMusicState = 1 - i;
            UpdateButtons();
        }

        public void OnPressedButtonSound( int i )
        {
            iSoundState = 1 - i;
            UpdateButtons();
        }

        public void OnPressedCancelFBConnecting()
        {
            panelState = PanelState.Common;
            UpdateButtons();
        }

        public void OnPressedFBConnectingNow()
        {
            iFBState = 1;
            panelState = PanelState.Common;
            UpdateButtons();
        }

        public void OnPressedFB( int i )
        {
            panelState = i == 0 ? PanelState.FBConnect : PanelState.Common;
            //iFBState = 1 - i;
            UpdateButtons();
        }

        public void OnPressedGoToChangeLanguage()
        {
            panelState = PanelState.ChangeLanguage;
            UpdateButtons();
        }

        public void OnPressedSelectLanguage( int iSelectedLang )
        {
            panelState = PanelState.Common;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            musicButtons[ 0 ].gameObject.SetActive( iMusicState == 0 );
            musicButtons[ 1 ].gameObject.SetActive( iMusicState == 1 );

            soundButtons[ 0 ].gameObject.SetActive( iSoundState == 0 );
            soundButtons[ 1 ].gameObject.SetActive( iSoundState == 1 );

            fbButtons[ 0 ].gameObject.SetActive( iFBState == 0 );
            fbButtons[ 1 ].gameObject.SetActive( iFBState == 1 );

            for ( int i = 0; i < commonBlocks.Length; i++ )
            {
                commonBlocks[ i ].SetActive( i == ( int ) panelState );
            }
        }
    }
}
