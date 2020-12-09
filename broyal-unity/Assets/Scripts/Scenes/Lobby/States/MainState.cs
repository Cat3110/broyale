using System;
using System.Linq;
using Scripts.Core.StateMachine;
using SocketIO;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Adic;
using Bootstrappers;
using RemoteConfig;
using UnityEngine.SceneManagement;
using Scripts.Common.Data.Data;
using Scripts.Common.Data;
using SocketIO.Data.Responses;
using SocketIOExt;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.States
{
    public class MainState : BaseStateMachineState
    {
        [Inject] private IUserData userData;
        
        [SerializeField] private TMP_Text connectingTimer;
        [SerializeField] private GameObject stateLocker;

        [SerializeField] private Transform personRoot;

        //TODO:remove me 
        [SerializeField] private List<UIController.SkillIdToSprite> namedSprites;
        [SerializeField] private Button ButtonChangeMainSkill;//
        
        
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
            ButtonChangeMainSkill.GetComponent<Image>().sprite = GetSpriteById(userData.GetCurrentCharacter().skill_set.main_skill);
            
            ButtonChangeMainSkill.onClick.RemoveAllListeners();
            ButtonChangeMainSkill.onClick.AddListener(OnChangeMainSkill);
        }

        private void OnGameStarted(SocketIOEvent obj)
        {
            var game = obj.data.Deserialize<Game>();
            if (game == null || game.id != _currentGameId) return;
            
            Debug.Log($"{LobbyEvents.GAME_STARTED} {game}");
//#if UNITY_EDITOR
//              SceneManager.LoadScene("PreviewClientServer");
//#else
            SceneManager.LoadScene("Client");
//#endif
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

        private Sprite GetSpriteById(string id) => namedSprites.FirstOrDefault(i => i.Id == id)?.Sprite;

        private void OnChangeMainSkill()
        {
            var character = userData.GetCurrentCharacter();
            
            var appConfig = MainContainer.Container.Resolve<AppConfig>();
            var availableSkills = appConfig.Skills.Take(4).ToList();
            var index = availableSkills.FindIndex( x => x.Id == character.skill_set.main_skill);
            var nextIndex = index + 1 >= availableSkills.Count ? 0 : index + 1;
            var nextSkillId = availableSkills[nextIndex].Id;
            
            userData.SetSkill(nextSkillId);
            ButtonChangeMainSkill.GetComponent<Image>().sprite = GetSpriteById(nextSkillId);
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
