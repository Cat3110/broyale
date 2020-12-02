
using System;
using System.Linq;
using Scripts.Core.StateMachine;
using SocketIO;
using TMPro;
using FullSerializer;
using UnityEngine;
using System.Collections;
using Bootstrappers;
using UnityEngine.SceneManagement;
using Scripts.Common.Data.Data;
using Scripts.Common.Data;
using SocketIOExt;

namespace Scripts.Scenes.Lobby.States
{
    public class MainState : BaseStateMachineState
    {
        [SerializeField] private TMP_Text connectingTimer;
        [SerializeField] private GameObject stateLocker;

        [SerializeField] private Transform personRoot;

        private ConnectionStatus status;
        private SocketIOComponent _socket;

        public OldGameData[] _games;

        private string currentGameName = null;
        private string currentGameId = null;

        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );

            stateLocker.SetActive( false );

            _socket = GameObject.FindObjectOfType<SocketIOComponent>();

            _socket.On( LobbyEvents.SERVER_UPDATE, OnServerUpdate );
            _socket.On( LobbyEvents.GAME_UPDATE, OnGameUpdate );
            
            _socket.Emit(LobbyEvents.UPDATE_LIST, (gameList) =>
            {
                //UpdateGameList(gameList["games"].list);
            });

            //connectingTimer.text = "";
        }

        public void OnPressedPlay()
        {
            OnPressedCreateRoom();
        }

        public void OnPressedGoToProfile()
        {
            stateMachine.SetState( ( int ) LobbyState.Profile );
        }

        public void OnPresssedGoToAbilities()
        {
            stateMachine.SetState( ( int ) LobbyState.Abilities );
        }

        public void OnPressedGoToSettings()
        {
            stateMachine.SetState( ( int ) LobbyState.Settings );
        }

        private void OnPressedCreateRoom()
        {
            stateLocker.SetActive( true );

            var user = ( stateMachine as LobbyController ).userData._id;
            var gameName = $"{user}{DateTime.Now}";
            
            _socket.CreateGame(gameName, (response) =>
            {
                Debug.Log($"CreateGame => {response}");
                
                var users = response.game.gameUsers;
                currentGameName = response.game.name;
                currentGameId = response.game.id;
                
                MainContainer.Container.Resolve<IGlobalSession>().Game = response.game;
                
                OnPressedStartGame();
            }, () => Debug.LogError("CreateGame failed"));
        }

        private void OnPressedStartGame()
        {
            _socket.StartGame(currentGameId, (response) =>
            {
                Debug.Log($"StartGame => {response}");
            }, () => Debug.LogError("SetCharacter failed"));
        }
    
        private void OnServerUpdate( SocketIOEvent obj )
        {
            var gamesData = ParseGamesList(obj.data.ToString());
            if( gamesData != null ) UpdateGameList(gamesData);

            var games = obj.data?["games"].list;
            Debug.Log($"SERVER_UPDATE {games?.Count} {games}");
        }

        private void OnGameUpdate(SocketIOEvent obj)
        {
            var gameData = ParseGame(obj.data.ToString());
            if( gameData != null && gameData.id == currentGameId )
            {
                var game = obj.data;
                Debug.Log($"{LobbyEvents.GAME_UPDATE} {game}");

                //_uiController.Lobby.UpdateConnectionStatus(LobbyUI.ConnectionStatus.WaitForGameStart);
                SetTimer( gameData.serverInfo.time );

                //GlobalSettings.ServerAddress = gameData.serverInfo.address;
                GlobalSettings.ServerPort = (ushort)gameData.serverInfo.port;
            
                StartCoroutine(FinalCountdown(gameData.serverInfo.time));
            }
        }

        private IEnumerator FinalCountdown(float time)
        {
            while (time > 0)
            {
                yield return new WaitForSeconds(1);
                time -= 1;
                SetTimer((int)time);
            }
#if UNITY_EDITOR
            SceneManager.LoadScene("PreviewClientServer");
            
#else
            SceneManager.LoadScene("Client");
#endif
        }

        private void SetTimer( int time )
        {
            connectingTimer.text = time > 0 ? time.ToString() : "";
            
            //startGameButton.interactable = false;
        }

        private OldGameData ParseGame(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.TrimStart(new char[] {'['});
                str = str.TrimEnd(new char[] {']'});
            }
            fsSerializer fsSerializer = new fsSerializer();
            OldGameData gameData = null;
        
            fsResult result = fsJsonParser.Parse(str, out fsData fsData);
            if (result.Succeeded)
            {
                result = fsSerializer.TryDeserialize(fsData, ref gameData);
                if (!result.Succeeded) Debug.LogError($"ParseGame TryDeserialize fail {result.FormattedMessages}");
            }else Debug.LogError($"ParseGame Parse fail {result.FormattedMessages}");

            return gameData;
        }

        private void UpdateGameList(GamesData gamesData)
        {
            Debug.Log($"UpdateGameList {gamesData.games.Length}");
        
            _games = gamesData.games;
        
            // UpdateRooms( _games );
        }

        private GamesData ParseGamesList(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.TrimStart(new char[] {'['});
                str = str.TrimEnd(new char[] {']'});
            }
            fsSerializer fsSerializer = new fsSerializer();
            GamesData gamesData = null;
        
            fsResult result = fsJsonParser.Parse(str, out fsData fsData);
            if (result.Succeeded)
            {
                result = fsSerializer.TryDeserialize(fsData, ref gamesData);
                if (!result.Succeeded) Debug.LogError($"ParseGamesList TryDeserialize fail {result.FormattedMessages}");
            }else Debug.LogError($"ParseGamesList Parse fail {result.FormattedMessages}");

            return gamesData;
        }
    }
}
