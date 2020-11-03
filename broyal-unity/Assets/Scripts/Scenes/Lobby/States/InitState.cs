
using Scripts.Core.StateMachine;

namespace Scripts.Scenes.Lobby.States
{
    public class InitState : BaseStateMachineState
    {
        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );

            Invoke( "ToNextState", 2f );
        }

        public override void OnEndState()
        {
            base.OnEndState();
        }
    }
}