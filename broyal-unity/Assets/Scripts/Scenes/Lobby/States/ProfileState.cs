
using Scripts.Core.StateMachine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class ProfileState : BaseStateMachineState
    {
        [SerializeField] private Color[] colors;
        [SerializeField] private Image[] images;

        private int[] colorIndexes = { 0, 0, 0 };

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );

            UpdateView();
        }

        public override void OnEndState()
        {
            base.OnEndState();
        }

        public void OnPressedPrev( int index )
        {
            colorIndexes[ index ]--;
            if ( colorIndexes[ index ] < 0 )
            {
                colorIndexes[ index ] = colors.Length - 1;
            }

            UpdateView();
        }

        public void OnPressedNext( int index )
        {
            colorIndexes[ index ]++;
            if ( colorIndexes[ index ] == colors.Length )
            {
                colorIndexes[ index ] = 0;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            for ( int i = 0; i < colors.Length; i++ )
            {
                images[ i ].color = colors[ colorIndexes[ i ] ];
            }
        }

        public void OnPressedGoToMain()
        {
            stateMachine.SetState( ( int ) LobbyState.Main );
        }
    }
}
