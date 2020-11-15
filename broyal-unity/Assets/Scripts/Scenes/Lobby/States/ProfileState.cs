
using System;
using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using static CharactersBindData;

namespace Scripts.Scenes.Lobby.States
{
    public class ProfileState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private IGameData gameData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private Button[] sexButtons;

        private int currentSkinIndex;
        private int newSkinIndex;
        private CurrentSkinData newSkinData;
        private UserSkinData currentSkin;
        private uint[] skinPartIndexes = { 0, 0, 0 };

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
            newSkinData = new CurrentSkinData( curSkin );
            currentSkinIndex = Array.FindIndex<UserSkinData>( skins, f => f.SkinId == curSkin.SkinId );
            newSkinIndex = currentSkinIndex;
            currentSkin = skins[ currentSkinIndex ];

            skinPartIndexes[ 0 ] = curSkin.HeadIndex;
            skinPartIndexes[ 1 ] = curSkin.BodyIndex;
            skinPartIndexes[ 2 ] = curSkin.PantsIndex;
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
            if ( skinPartIndexes[ index ] > 0 )
            {
                skinPartIndexes[ index ]--;
            }
            else
            {
                if ( index == ( int ) SkinPart.Body )
                {
                    skinPartIndexes[ index ] = ( uint ) ( currentSkin.Bodies.Length - 1 );
                }
                else if ( index == ( int ) SkinPart.Head )
                {
                    skinPartIndexes[ index ] = ( uint ) ( currentSkin.Heads.Length - 1 );
                }
                else if ( index == ( int ) SkinPart.Pants )
                {
                    skinPartIndexes[ index ] = ( uint ) ( currentSkin.Pants.Length - 1 );
                }
            }

            UpdateView();
        }

        public void OnPressedNext( int index )
        {
            string[] skinPart = index == ( int ) SkinPart.Head ? currentSkin.Heads : ( index == ( int ) SkinPart.Body ? currentSkin.Bodies : currentSkin.Pants );

            if ( skinPartIndexes[ index ] + 1 >= skinPart.Length )
            {
                skinPartIndexes[ index ] = 0;
            }
            else
            {
                skinPartIndexes[ index ]++;
            }

            UpdateView();
        }

        private void UpdateView()
        {
            for ( int i = 0; i < sexButtons.Length; i++ )
            {
                sexButtons[ i ].interactable = i != newSkinIndex;
            }

            newSkinData.HeadIndex = skinPartIndexes[ 0 ];
            newSkinData.BodyIndex = skinPartIndexes[ 1 ];
            newSkinData.PantsIndex = skinPartIndexes[ 2 ];
            contentFactory.SetupPlayerPersonFor( newSkinData );

            userData.SetSkin( newSkinData );
        }

        public void OnPressedGoToMain()
        {
            stateMachine.SetState( ( int ) LobbyState.Main );
        }
    }
}
