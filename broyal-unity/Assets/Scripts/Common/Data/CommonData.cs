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
        public const string JOIN_GAME = nameof(JOIN_GAME);
        //public const string START_GAME = nameof(START_GAME);
        public const string CLIENT_ADDED = nameof(CLIENT_ADDED);
        public const string GET_GAMES = nameof(GET_GAMES);
        public const string WINNER = nameof(WINNER); 
        public const string LOSER = nameof(LOSER);
        public const string LEAVE = nameof(LEAVE);
        
        public const string GAME_STARTED = nameof(GAME_STARTED);
    }

    namespace Data
    {
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
    }
}