
using System;

namespace Scripts.Core.StateMachine
{
	public interface IStateMachineState
	{
		void OnStartState( IStateMachine stateMachine, params object[] args );
		void OnEndState();
	}
}
