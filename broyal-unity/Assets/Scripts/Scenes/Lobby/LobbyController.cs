
using Scripts.Core.StateMachine;

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
        BattlePass,
        BattleResult
    }

    public class LobbyController : BaseStateMachine
    {
        public JSONObject user;
        public JSONObject chat;
    }
}