
using Adic;
using Scripts.Common.Factories;
using Scripts.Core.StateMachine;
using UnityEngine;

namespace Scripts.Scenes.Lobby.States
{
    public class ShopState : BaseStateMachineState
    {
        [Inject] private ILobbyContentFactory contentFactory;

        private GameObject skinPerson = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            skinPerson = contentFactory.GetPlayerPerson();
            skinPerson.SetActive( false );
        }

        public override void OnEndState()
        {
            base.OnEndState();

            skinPerson.SetActive( true );
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
