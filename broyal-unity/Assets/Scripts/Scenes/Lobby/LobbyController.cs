
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
        Main
    }

    public class LobbyController : BaseStateMachine
    {

    }
}