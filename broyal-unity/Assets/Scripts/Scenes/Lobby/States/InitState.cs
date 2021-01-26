using System.Collections;
using Adic;
using Bootstrappers;
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.Data.Data;
using Scripts.Common.Tools.UI;
using Scripts.Common.Factories;
using Scripts.Core.StateMachine;
using SocketIO;
using SocketIOExt;
using UniRx.Async;
using UnityEngine;

namespace Scripts.Scenes.Lobby.States
{
    public class InitState : BaseStateMachineState
    {
        public static bool ShowBattleResult = false; // temporary solution

        [Inject] private IUserData userData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private CharacterRotator[] charRotators;

        private SocketIOComponent _socket;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();
            
            //userData.Load();

            _socket = GameObject.FindObjectOfType<SocketIOComponent>();

            AppConfig.LoadByUrlAsync().ContinueWith( json =>
            {
                var config = BaseBootStrapper.OnConfigLoaded(json);
                _socket.url = config.Main.LobbyAddress;
                _socket.Connect();
                
                StartCoroutine( _InitNetworkReady() );
            });
        }

        public override void OnEndState()
        {
            base.OnEndState();

            // setup user person
            CurrentSkinData skinData = userData.GetSkin();
            GameObject skinPerson = contentFactory.GetPlayerPerson( skinData.SkinId );
            contentFactory.SetupPlayerPersonFor( skinData );

            foreach ( var cr in charRotators )
            {
                cr.SetCharacter( skinPerson.transform );
            }
        }

        private IEnumerator _InitNetworkReady()
        {
            while ( !_socket.isOpen )
            {
                yield return new WaitForSeconds( 0.5f );
            }

            userData.Load( ( success ) =>
            {
                if ( success )
                {
                    var newSession = new GlobalSession
                    {
                        User = userData.GetUserInfo(),
                        Character = userData.GetCurrentCharacter()
                    };
                    MainContainer.Container.Register<IGlobalSession>( newSession );

                    if ( ShowBattleResult ) // TODO FIXIT
                    {
                        stateMachine.SetState( ( int ) LobbyState.BattleResult );
                    }
                    else
                    {
                        ToNextState();
                    }

                    ShowBattleResult = false;
                }
                else
                {
                    Debug.LogError( "LoginWithDeviceId failed" );
                    // send to show error state
                }
            } );
        }
    }
}