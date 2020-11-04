using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using FullSerializer;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class TestLobby : MonoBehaviour
{
    [SerializeField] private SocketIOComponent _socket;
    [SerializeField] private UIController _uiController;
    
    public JSONObject user;
    public JSONObject chat;

    public GameData[] _games;
    
    IEnumerator Start()
    {
        _uiController.Lobby.Show(new string[]{},
            LobbyUI.ConnectionStatus.Disconnected,
            (roomId) => onSelectRoom(roomId), 
            () => onCreateRoom(),
            () => onStartGame() );
        
        while( !_socket.isOpen ) 
            yield return new WaitForSeconds(0.5f);
        
        //_uiController.Lobby.UpdateConnectionStatus( LobbyUI.ConnectionStatus.Connecting );
        
        //_socket.On(LobbyEvents.SERVER_UPDATE, OnServerUpdate);
        //_socket.On(LobbyEvents.GAME_UPDATE, OnGameUpdate);

        var login = $"{Application.platform}-{SystemInfo.deviceName}-{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
        _socket.Emit(LobbyEvents.VERIFY_USER, login, (userResponse) => {
            user = userResponse.list?.First()["user"];
            _socket.Emit(LobbyEvents.USER_CONNECTED, user);
            
            //_uiController.Lobby.UpdateConnectionStatus( LobbyUI.ConnectionStatus.Connected );
            
            _socket.Emit(LobbyEvents.UPDATE_LIST, (gameList) =>
            {
                //UpdateGameList(gameList["games"].list);
            });
            
            //Debug.Log(LobbyEvents.USER_CONNECTED + user);
        });
        
        yield return null;
    }

    /*private void OnGameUpdate(SocketIOEvent obj)
    {
        var gameData = ParseGame(obj.data.ToString());
        if( gameData != null && gameData.id == currentGameId )
        {
            var game = obj.data;
            Debug.Log($"{LobbyEvents.GAME_UPDATE} {game}");
            
            //_uiController.Lobby.UpdateConnectionStatus(LobbyUI.ConnectionStatus.WaitForGameStart);
            _uiController.Lobby.SetTimer(gameData.serverInfo.time);

            //GlobalSettings.ServerAddress = gameData.serverInfo.address;
            GlobalSettings.ServerPort = (ushort)gameData.serverInfo.port;
            
            StartCoroutine(FinalCountdown(gameData.serverInfo.time));
        }
    }*/
    
    /*private void OnServerUpdate(SocketIOEvent obj)
    {
        var gamesData = ParseGamesList(obj.data.ToString());
        if( gamesData != null ) UpdateGameList(gamesData);

        var games = obj.data?["games"].list;
        Debug.Log($"SERVER_UPDATE {games?.Count} {games}");
    }*/

    /*private void UpdateGameList(GamesData gamesData)
    {
        Debug.Log($"UpdateGameList {gamesData.games.Length}");
        
        _games = gamesData.games;
        
        _uiController.Lobby.UpdateRooms(_games);
    }*/

    private void onStartGame()
    {
        string encodedString = "{\"gameId\": \"previousGame\",\"previousGame\": \"none\"}";
        
        var json = new JSONObject(encodedString);
        json.SetField("user", user);
        json["gameId"].str = currentGameId;

     
        _socket.Emit(LobbyEvents.START_GAME, json, (response) =>
        {
            //{"id":"030e2ab1-9fe6-49f7-b8fb-fe54b076c3bb",
            //"name":"Game 1","gameUsers":[{"id":"28e51e1d-75fd-435f-8487-97e3e04d4996","name":"WindowsEditor-MSI","icon":"X"}]}
            // var users = response.list.First()["gameUsers"].list;
            // currentGameName = response.list.First()["name"].str;
            // currentGameId = response.list.First()["id"].str;
            // _uiController.Lobby.UpdateUsers( users.Select( u => u["name"].str) );
            // _uiController.Lobby.SetInRoom(currentGameName, true);
            Debug.Log($"START_GAME {response}");
        });
    }

    /*private IEnumerator FinalCountdown(float time)
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time -= 1;
            _uiController.Lobby.SetTimer((int)time);
        }

        SceneManager.LoadScene(1);
    }*/

    private void onCreateRoom()
    {
        /*var gameName = $"{user}{DateTime.Now}";
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
            _uiController.Lobby.UpdateUsers( users.Select( u => u["name"].str) );
            _uiController.Lobby.SetInRoom(currentGameName, true);
            Debug.Log($"CREATE_GAME {response}");
        });*/
    }

    private void onSelectRoom(string gameId)
    {
        var game = _games.FirstOrDefault(g => g.id == gameId);
        if (game == null || game.gameStarted)
        {
            Debug.LogWarning($"Game already started or not exist {game?.name}");
            return;
        }

        string encodedString = "{\"gameId\": \"previousGame\",\"previousGame\": \"none\"}";
        var json = new JSONObject(encodedString);
        // json.SetField("gameId", "str"); // WTF
        // //json.SetField("gameId", game.id);
        // json.SetField("previousGame", "none");
        json.SetField("user", user);

        json["gameId"].str = game.id;
        
        _socket.Emit(LobbyEvents.ACTIVE_GAME, json, (response) =>
        {
            var gamesData = ParseGamesList(response.ToString());
            if (gamesData != null)
            {
                game = gamesData.games.FirstOrDefault(g => g.id == gameId);
                
                currentGameName = game.name;
                currentGameId = game.id;
                //_uiController.Lobby.UpdateUsers( game.users.Select( u => u.name) );
                //_uiController.Lobby.SetInRoom(currentGameName, true);
            }
            Debug.Log($"ACTIVE_GAME {response}");
        });
    }

    private string currentGameName = null;
    private string currentGameId = null;
    
    public void CreateNewGame()
    {
        currentGameName = $"{user} + {Guid.NewGuid()}";
        var json = new JSONObject();
        json.SetField("gameName", currentGameName);
        json.SetField("user", user);
     
        _socket.Emit(LobbyEvents.CREATE_GAME, json, (response) =>
        {
            Debug.Log($"CREATE_GAME {response}");
        });
    }
    
    public void ConnectToGame(string name)
    {
        
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
    
    /*private GameData ParseGame(string str)
    {
        if (str.StartsWith("["))
        {
            str = str.TrimStart(new char[] {'['});
            str = str.TrimEnd(new char[] {']'});
        }
        fsSerializer fsSerializer = new fsSerializer();
        GameData gameData = null;
        
        fsResult result = fsJsonParser.Parse(str, out fsData fsData);
        if (result.Succeeded)
        {
            result = fsSerializer.TryDeserialize(fsData, ref gameData);
            if (!result.Succeeded) Debug.LogError($"ParseGame TryDeserialize fail {result.FormattedMessages}");
        }else Debug.LogError($"ParseGame Parse fail {result.FormattedMessages}");

        return gameData;
    }*/
    

    private IEnumerator Login()
    {
        // wait 1 seconds and continue
        yield return new WaitForSeconds(1);

        //
        // var login = $"{Application.platform}+{ SystemInfo.deviceName}";
        // Emit(VERIFY_USER, login, (userResponse) => {
        //     user = userResponse.list?.First()["user"];
        //     Emit(USER_CONNECTED, user);
        //
        //     Emit(COMMUNITY_CHAT, (chatResponse) =>
        //     {
        //         chat = chatResponse.list?.First();
        //         Debug.Log(COMMUNITY_CHAT + chat);
        //         On(MESSAGE_RECIEVED + "-" + chat["id"].str, OnMessageChat);
        //     });
        //
        //     Debug.Log(VERIFY_USER + user);
        // });
            
             
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class LobbyEvents
{
    public const string COMMUNITY_CHAT = "COMMUNITY_CHAT";
    public const string USER_CONNECTED="USER_CONNECTED";
    public const string MESSAGE_RECIEVED="MESSAGE_RECIEVED";
    public const string MESSAGE_SENT="MESSAGE_SENT";
    public const string USER_DISCONNECTED="USER_DISCONNECTED";
    public const string TYPING="TYPING";
    public const string VERIFY_USER="VERIFY_USER";
    public const string LOGOUT="LOGOUT";

    public const string CREATE_GAME = nameof(CREATE_GAME);
    public const string GAME_OVER = nameof(GAME_OVER);
    public const string SERVER_UPDATE = nameof(SERVER_UPDATE);
    public const string GAME_UPDATE = nameof(GAME_UPDATE);
    public const string PLAYER_INPUT = nameof(PLAYER_INPUT);
    public const string ACTIVE_GAME = nameof(ACTIVE_GAME);
    public const string START_GAME = nameof(START_GAME);
    public const string CLIENT_ADDED = nameof(CLIENT_ADDED);
    public const string UPDATE_LIST = nameof(UPDATE_LIST);
    public const string WINNER = nameof(WINNER); 
    public const string LOSER = nameof(LOSER);
    public const string LEAVE = nameof(LEAVE);
}

namespace Data
{
    public class GamesData
    {
        public GameData[] games { get; set; }
    }

    public class GameData
    {
        public string id { get; set; }
        public string name { get; set; }
        
        public string owner { get; set; }
        public bool gameStarted { get; set; }
        public string turn { get; set; }
        
        public Serverinfo serverInfo { get; set; }
        public UserData[] users { get; set; }
        public string gameState { get; set; }
    }

    public class Serverinfo
    {
        public int time { get; set; }
        public string address { get; set; }
        public int port { get; set; }
    }

    public class UserData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
    }
}

