
using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;
using UnityEngine;

namespace Scripts.Scenes.Lobby.States
{
    public class SettingsState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        private int prevState = -1;
        private GameObject skinPerson = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            prevState = stateMachine.PrevState;

            skinPerson = contentFactory.GetPlayerPerson();
            skinPerson.SetActive( false );
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
    }
}
