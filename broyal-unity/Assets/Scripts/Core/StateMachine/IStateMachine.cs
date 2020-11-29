using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Scripts.Core.StateMachine
{
	public interface IStateMachine
	{
        int PrevState { get; }
		int State { get; }
		void SetState( int state, params object[] args );
        IStateMachineState GetState( int state );

		event Action<int,int> OnChangeState;
	}

#if UNIFIED_CONTAINER_DEFINED
	[Serializable]
	public class IStateMachineContainer : IUnifiedContainer<IStateMachine> {}
#endif
}