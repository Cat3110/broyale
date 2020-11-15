
using System;
using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class ProfileState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private Button[] sexButtons;
        [SerializeField] private Color[] colors;
        [SerializeField] private Image[] images;

        private int currentSkinIndex;
        private int newSkinIndex;
        private CurrentSkinData newSkinData;
        private int[] colorIndexes = { 0, 0, 0 };

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            SetupCurrentPerson();
            UpdateView();
        }

        public override void OnEndState()
        {
            base.OnEndState();

            userData.SetSkin( newSkinData );
        }

        private void SetupCurrentPerson()
        {
            UserSkinData[] skins = gameData.Skins;
            CurrentSkinData curSkin = userData.GetSkin();
            currentSkinIndex = Array.FindIndex<UserSkinData>( skins, f => f.SkinId == curSkin.SkinId );
            newSkinIndex = currentSkinIndex;
        }

        public void OnPressedMaleFemale( int index )
        {
            if ( index == newSkinIndex ) return;

            newSkinIndex = index;
            var newSkin = gameData.Skins[ newSkinIndex ];
            newSkinData = new CurrentSkinData( newSkin );
            contentFactory.SetupPlayerPersonFor( newSkinData );

            UpdateView();
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
            for ( int i = 0; i < sexButtons.Length; i++ )
            {
                sexButtons[ i ].interactable = i != newSkinIndex;
            }

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
