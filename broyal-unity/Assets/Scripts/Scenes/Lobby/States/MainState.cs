using System;
using System.Linq;
using Scripts.Core.StateMachine;
using SocketIO;
using TMPro;
using UnityEngine;
using System.Collections;
using Adic;
using Bootstrappers;
using UnityEngine.SceneManagement;
using Scripts.Common.Data.Data;
using Scripts.Common.Data;
using SocketIO.Data.Responses;
using SocketIOExt;
using Scripts.Common.Factories;
using Scripts.Common;
using Scripts.Common.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class MainState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        [Inject] private ILobbyContentFactory contentFactory;
        
        [SerializeField] private TMP_Text connectingTimer;
        [SerializeField] private GameObject stateLocker;

        [SerializeField] private Transform personRoot;

        [SerializeField] private AbilitySelectedItem mainAbilityIcon;

        private SocketIOComponent _socket;
        private Game[] _games;
        private string _currentGameId = null;
       
        public override void OnStartState( IStateMachine stateMachine, params object[] args )
        {
            base.OnStartState( stateMachine, args );
            this.Inject();
            
            stateLocker.SetActive( false );

            _socket = GameObject.FindObjectOfType<SocketIOComponent>();

            _socket.On( LobbyEvents.SERVER_UPDATE, OnServerUpdate );
            _socket.On( LobbyEvents.GAME_UPDATE, OnGameUpdate );
            _socket.On( LobbyEvents.GAME_STARTED, OnGameStarted );

            //connectingTimer.text = "";
            mainAbilityIcon.Setup( contentFactory.GetSpriteById(userData.GetCurrentCharacter().skill_set.main_skill) );
        }

        public override void OnEndState()
        {
            base.OnEndState();
            
            _socket.Off( LobbyEvents.SERVER_UPDATE, OnServerUpdate );
            _socket.Off( LobbyEvents.GAME_UPDATE, OnGameUpdate );
            _socket.Off( LobbyEvents.GAME_STARTED, OnGameStarted );
        }

        private void OnGameStarted(SocketIOEvent obj)
        {
            var game = obj.data.Deserialize<Game>();
            if (game == null || game.id != _currentGameId) return;
            
            Debug.Log($"{LobbyEvents.GAME_STARTED} {game}");
            
            if( _socket.useDevServer ) SceneManager.LoadScene( Constants.SCENE_CLIENTANDSERVER );
            else  SceneManager.LoadScene( Constants.SCENE_CLIENT );
        }

        public void OnPressedPlay()
        {
            stateLocker.SetActive( true );
            
            _socket.GetGames( (getGamesData) =>
            {
                var availableGame = getGamesData?.games?.FirstOrDefault(g => !g.isStarted);
                if (availableGame != null )
                {
                    _socket.JoinGame(availableGame.id,response =>
                    {
                        PrepareForGameStart(response.game, response.eta );
                    },() => Debug.LogError($"Join game{availableGame.id} failed"));
                }
                else
                {
                    var gameName = $"{DateTime.Now}";
            
                    _socket.CreateGame(gameName, (response) => 
                    {
                        PrepareForGameStart(response.game, response.eta );
                    }, () => Debug.LogError("CreateGame failed"));
                }
                
            }, () => Debug.LogError("GetGames failed"));
        }

        private void PrepareForGameStart(Game game, int eta)
        {
            _currentGameId = game.id;
                
            MainContainer.Container.Resolve<IGlobalSession>().Game = game;
            //GlobalSettings.ServerAddress = gameData.serverInfo.address;
            GlobalSettings.ServerPort = (ushort)game.serverInfo.port;
            SetTimer( eta );
            StartCoroutine(FinalCountdown(eta));
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
  
        private void OnServerUpdate( SocketIOEvent obj )
        {
            var data = obj.data.Deserialize<UpdateGamesListEvent>();
            if (data?.games != null)
            {
                UpdateGameList( data.games );
                Debug.Log($"SERVER_UPDATE {data.games.Length} {data.games}");
            }
        }
        private void OnGameUpdate(SocketIOEvent obj)
        {
            var game = obj.data.Deserialize<Game>();
            if( game != null && game.id == _currentGameId )
            {
                Debug.Log($"{LobbyEvents.GAME_UPDATE} {game}");
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
        }

        private void SetTimer( int time )
        {
            connectingTimer.text = time > 0 ? time.ToString() : "";
            //startGameButton.interactable = false;
        }

        private void UpdateGameList(Game[] games)
        {
            Debug.Log($"UpdateGameList {games.Length}");
            this._games = games;
        }
    }
}
