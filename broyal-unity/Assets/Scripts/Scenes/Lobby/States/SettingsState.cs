
using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class SettingsState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private Button[] musicButtons;
        [SerializeField] private Button[] soundButtons;
        [SerializeField] private Button[] fbButtons;

        private int prevState = -1;
        private GameObject skinPerson = null;

        private int iMusicState = 0;
        private int iSoundState = 1;
        private int iFBState = 0;

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

        public void OnPressedFB( int i )
        {
            iFBState = 1 - i;
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
        }
    }
}
