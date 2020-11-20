
using System;
using System.Collections;
using System.Linq;
using Adic;
using Scripts.Common.Data;
using Scripts.Common.Tools.UI;
using Scripts.Common.ViewItems;
using Scripts.Core.StateMachine;
using SocketIO;
using UnityEngine;

namespace Scripts.Scenes.Lobby.States
{
    public class InitState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private CharacterRotator[] charRotators;

        private SocketIOComponent _socket;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();

            userData.Load();

            _socket = GameObject.FindObjectOfType<SocketIOComponent>();

            StartCoroutine( _InitNetworkReady() );
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
        
            var login = $"{Application.platform}-{SystemInfo.deviceName}-{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
            _socket.Emit( LobbyEvents.VERIFY_USER, login, (userResponse) => {

                ( stateMachine as LobbyController ).user = userResponse.list?.First()["user"]; // ERESH fix it
                _socket.Emit( LobbyEvents.USER_CONNECTED, ( stateMachine as LobbyController ).user ); // ERESH fix it
            
                //Debug.Log(LobbyEvents.USER_CONNECTED + user);
                ToNextState();
            });

        }
    }
}