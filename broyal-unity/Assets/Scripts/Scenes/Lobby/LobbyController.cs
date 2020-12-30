
using Scripts.Core.StateMachine;
using SocketIO.Data.Responses;

namespace Scripts.Scenes.Lobby
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        WaitForGameStart
    }

    public enum LobbyState
    {
        Init,
        Main,
        Profile,
        Abilities,
        Settings,
        Rating,
        Shop,
        BattlePass
    }

    public class LobbyController : BaseStateMachine
    {
        public JSONObject user;
        public JSONObject chat;
        public User userData;
    }
}