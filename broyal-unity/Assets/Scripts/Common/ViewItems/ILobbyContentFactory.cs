
using Scripts.Common.Data;
using UnityEngine;

namespace Scripts.Common.ViewItems
{
    public interface ILobbyContentFactory
    {
        GameObject GetPlayerPerson( string skinId );
        bool SetupPlayerPersonFor( CurrentSkinData skinData );
    }
}