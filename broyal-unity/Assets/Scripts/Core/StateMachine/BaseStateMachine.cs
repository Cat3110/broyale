using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Scripts.Core.StateMachine
{
	public class BaseStateMachine : MonoBehaviour, IStateMachine
	{
		[SerializeField] public bool AutoStart = true;

		[SerializeField] protected List<MonoBehaviour> stateRoots = new List<MonoBehaviour>();
		[SerializeField] protected bool onlyComponent = false;
		[SerializeField] private bool disablePrevState = true;
		private List<IStateMachineState> states = new List<IStateMachineState>();
		private IStateMachineState state = null;
		private int curState = -1;
        private int prevState = -1;

		private bool nowNextStateActual = false;
		private int nextStateTmp = -1;
		private object[] nextArgsTmp = null;

		public bool OnlyComponent { get { return onlyComponent; } set { onlyComponent = true; } }
		public int State { get { return curState; } }
        public int PrevState { get { return prevState; } }

        public event Action<int,int> OnChangeState = delegate{};

        protected virtual void _OnChangeState( int prevState, int newState ) {}

		protected virtual void Start ()
		{
			InitStates();

			if ( AutoStart && stateRoots.Count > 0 )
			{
				SetState( 0 );
			}
		}

		protected void InitStates()
		{
			if ( stateRoots != null )
			{
				foreach(MonoBehaviour stateRoot in stateRoots)
				{
					if (stateRoot == null)
					{
						states.Add( null );
					}
					else if (stateRoot != null && stateRoot is IStateMachineState )
					{
						states.Add( stateRoot as IStateMachineState );
						
						if (stateRoot is MonoBehaviour)
						{
							if ( ! onlyComponent )
							{
								stateRoot.gameObject.SetActive( false );
							}
							else
							{
								stateRoot.enabled = false;
							}
                        
                            BaseStateMachineState baseStateMachineState = ( BaseStateMachineState ) stateRoot;
                            if ( baseStateMachineState != null )
                            {
                                baseStateMachineState.SetActiveStateObjects( false );
                            }
                        }
                    }
				}
			}
		}

		public virtual void SetState( int _state, params object[] args )
		{
			nowNextStateActual = true;
			nextStateTmp = _state;
			nextArgsTmp = args;
		}

		public void AddState( MonoBehaviour state, bool asComponent )
		{
			if ( asComponent )
			{
				stateRoots.Add( state );
				states.Add( ( IStateMachineState ) state );
			}
		}

        public IStateMachineState GetState( int state )
        {
            if ( state < 0 || state >= states.Count ) return null;

            return states[ state ];
        }

		private void Update()
		{
			if ( ! nowNextStateActual ) return;

			nowNextStateActual = false;
			int _state = nextStateTmp;
			object[] args = nextArgsTmp;

			prevState = curState;

			if (state != null)
			{
				state.OnEndState();

				if ( disablePrevState )
				{
					if (state is MonoBehaviour)
					{
						if ( ! onlyComponent )
						{
							(state as MonoBehaviour).gameObject.SetActive( false );
						}
						else
						{
							( state as MonoBehaviour ).enabled = false;
						}
					}
				}
			}

			if ( _state != -1 && states != null && _state < states.Count )
			{
				state = states[ _state ];
				curState = _state;
			}
			else
			{
				state = null;
				curState = -1;
			}

			if (state != null)
			{
				if (state is MonoBehaviour)
				{
					if ( ! onlyComponent )
					{
						(state as MonoBehaviour).gameObject.SetActive( true );
					}
					else
					{
						( state as MonoBehaviour ).enabled = true;
					}
				}

				state.OnStartState( this, args );
			}

            _OnChangeState( prevState, curState );
			OnChangeState( prevState, curState );
		}

		protected virtual void OnDestroy()
		{
			SetState( -1 );
		}
	}
}