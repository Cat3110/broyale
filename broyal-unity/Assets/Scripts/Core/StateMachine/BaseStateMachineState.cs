
using UnityEngine;

namespace Scripts.Core.StateMachine
{
	public class BaseStateMachineState : MonoBehaviour, IStateMachineState
	{
		[SerializeField] protected IStateMachine stateMachine;
        [SerializeField] protected GameObject[] stateObjects;

		public virtual void OnStartState( IStateMachine stateMachine, params object[] args )
		{
			this.stateMachine = stateMachine;
            SetActiveStateObjects( true );
		}

		public virtual void OnEndState()
		{
            SetActiveStateObjects( false );
		}

		public void ToNextState()
		{
			ToNextState( null );
		}

		public virtual void ToNextState( params object[] args )
		{
			int nextState = this.stateMachine.State + 1;
			stateMachine.SetState( nextState, args );
		}

        public void SetActiveStateObjects( bool flag )
        {
            foreach ( GameObject obj in stateObjects )
            {
                if ( obj != null )
                {
                    obj.SetActive( flag );
                }
            }
        }
    }
}
