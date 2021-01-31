
using Adic;
using Scripts.Common.Factories;
using Scripts.Core.StateMachine;
using UnityEngine;

namespace Scripts.Scenes.Lobby.States
{
    public class BattlePassState : BaseStateMachineState
    {
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private GameObject[] states;

        private GameObject skinPerson = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            skinPerson = contentFactory.GetPlayerPerson();
            skinPerson.SetActive( false );

            OnPressedSetState( 0 );
        }

        public override void OnEndState()
        {
            base.OnEndState();

            skinPerson.SetActive( true );
        }

        public void OnPressedSetState( int state )
        {
            for ( int i = 0; i < states.Length; i++ )
            {
                states[ i ].SetActive( i == state );
            }
        }

        public void OnPressedGoToSettings()
        {
            stateMachine.SetState( ( int ) LobbyState.Settings );
        }

        public void OnPressedBackToMainmenu()
        {
            stateMachine.SetState( ( int ) LobbyState.Main );
        }
    }
}
