
using System;
using UnityEngine;

namespace Scripts.Common.ViewItems
{
    public class LobbyContentFactory : MonoBehaviour, ILobbyContentFactory
    {
        [SerializeField] private GameObject[] personsPrefabs;
        [SerializeField] private Transform lobbyPersonRoot;

        private string lobbyPlayerSkin = "";
        private GameObject lobbyPlayerPerson = null;

        public GameObject GetLobbyPlayerPerson( string skinId )
        {
            if ( skinId != lobbyPlayerSkin )
            {
                ClearCurrentLobbyPlayerPerson();
            }

            if ( lobbyPlayerPerson != null )
            {
                return lobbyPlayerPerson;
            }

            GameObject personPrefab = Array.Find<GameObject>( personsPrefabs, f => f.name == skinId );
            if ( personPrefab == null ) return null;

            lobbyPlayerPerson = Instantiate( personPrefab, lobbyPersonRoot );
            lobbyPlayerSkin = skinId;

            return lobbyPlayerPerson;
        }

        private void ClearCurrentLobbyPlayerPerson()
        {
            if ( lobbyPlayerPerson != null )
            {
                GameObject.DestroyObject( lobbyPlayerPerson );
                lobbyPlayerPerson = null;
            }

            lobbyPlayerSkin = "";
        }
    }
}