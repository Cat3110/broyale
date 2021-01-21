
using Scripts.Core.StateMachine;

namespace Scripts.Scenes.Lobby.States
{
    public class BattleResultState : BaseStateMachineState
    {
        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();
        }

        public override void OnEndState()
        {
            base.OnEndState();
        }

        public void OnPressedGoToMain()
        {
            stateMachine.SetState( ( int ) LobbyState.Main );
        }
    }
}
