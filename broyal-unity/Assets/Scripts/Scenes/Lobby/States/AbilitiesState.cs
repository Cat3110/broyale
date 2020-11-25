
using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;

namespace Scripts.Scenes.Lobby.States
{
    public class AbilitiesState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;


        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();
        }

        public override void OnEndState()
        {
            base.OnEndState();
        }
    }
}
