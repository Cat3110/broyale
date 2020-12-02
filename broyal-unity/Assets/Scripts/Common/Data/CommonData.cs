using SocketIO.Data.Responses;

namespace Scripts.Common.Data
{
    public static class LobbyEvents
    {
        public const string COMMUNITY_CHAT = "COMMUNITY_CHAT";
        public const string USER_CONNECTED="USER_CONNECTED";
        public const string MESSAGE_RECIEVED="MESSAGE_RECIEVED";
        public const string MESSAGE_SENT="MESSAGE_SENT";
        public const string USER_DISCONNECTED="USER_DISCONNECTED";
        public const string TYPING="TYPING";
        public const string VERIFY_USER = "VERIFY_USER";
        public const string LOGIN_WITH_DEVICEID = nameof(LOGIN_WITH_DEVICEID);
        public const string GET_CHARACTERS = nameof(GET_CHARACTERS);
        public const string SET_CHARACTER = nameof(SET_CHARACTER);
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
            public OldGameData[] games { get; set; }

        }

        public interface IGlobalSession
        {
            bool IsValid { get; }
            User User { get; }
            Character Character { get; }
            Game Game { get; set; }
            Character[] CharactersInGame { get; set; }
        }
        public class GlobalSession : IGlobalSession
        {
            public bool IsValid => User != null && Character != null;
            public User User { get; set; }
            public Character Character { get; set; }
            public Game Game { get; set; }
            
            public Character[] CharactersInGame { get; set; }
        }

        public class OldGameData
        {
            public string id { get; set; }
            public string name { get; set; }
        
            public string owner { get; set; }
            public bool gameStarted { get; set; }
            public string turn { get; set; }
        
            public Serverinfo serverInfo { get; set; }
            public OldUserData[] users { get; set; }
            public string gameState { get; set; }
        }

        public class Serverinfo
        {
            public int time { get; set; }
            public string address { get; set; }
            public int port { get; set; }
        }

        public class OldUserData
        {
            public string id { get; set; }
            public string name { get; set; }
            public string icon { get; set; }
        }
    }
}