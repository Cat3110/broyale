
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core.StateMachine;
using SocketIO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class MainState : BaseStateMachineState
    {
        [SerializeField] private TMP_Text Title;
        [SerializeField] private TMP_Text connectingTimer;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Image statusIcon;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button newGameButton;
        [SerializeField] private UsersPanel usersPanel;
        [SerializeField] private RoomPanel roomPanel;

        private ConnectionStatus status;
        private SocketIOComponent _socket;

        private JSONObject user; // temporary here

        private string currentGameName = null;
        private string currentGameId = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );

            _socket = GameObject.FindObjectOfType<SocketIOComponent>();
            UpdateConnectionStatus( ConnectionStatus.Disconnected );

            connectingTimer.text = "";
        }

        public void OnPressedCreateRoom()
        {
            var gameName = $"{user}{DateTime.Now}";
            var json = new JSONObject();
            json.SetField("gameName", gameName);
            json.SetField("user", user);
     
            _socket.Emit(LobbyEvents.CREATE_GAME, json, (response) =>
            {
                //{"id":"030e2ab1-9fe6-49f7-b8fb-fe54b076c3bb",
                //"name":"Game 1","gameUsers":[{"id":"28e51e1d-75fd-435f-8487-97e3e04d4996","name":"WindowsEditor-MSI","icon":"X"}]}
                var users = response.list.First()["gameUsers"].list;
                currentGameName = response.list.First()["name"].str;
                currentGameId = response.list.First()["id"].str;
                UpdateUsers( users.Select( u => u["name"].str ) );
                SetInRoom( currentGameName, true );
                Debug.Log($"CREATE_GAME {response}");
            });
        }

        public void SetInRoom( string roomName, bool isOwner )
        {
            Title.text = roomName;
            
            startGameButton.interactable = isOwner && status != ConnectionStatus.WaitForGameStart;
            newGameButton.interactable = false;
        
            roomPanel.Hide();
            usersPanel.Show();
        }

        public void OnPressedStartGame()
        {

        }

        public void UpdateUsers( IEnumerable<string> users )
        {
            usersPanel.Clean();
        
            foreach (var user in users)
            {
                usersPanel.Add(user);
            }
        }

        private void UpdateConnectionStatus( ConnectionStatus status )
        {
            this.status = status;
            statusText.text = status.ToString();

            if ( status == ConnectionStatus.Disconnected ||
               status == ConnectionStatus.Connecting )
            {
                statusIcon.color = Color.red;
            }
            else
            {
                statusIcon.color = Color.green;
            }
        }
    }
}