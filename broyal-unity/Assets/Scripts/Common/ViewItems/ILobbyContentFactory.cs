
using UnityEngine;

namespace Scripts.Common.ViewItems
{
    public interface ILobbyContentFactory
    {
        GameObject GetLobbyPlayerPerson( string skinId );
    }
}