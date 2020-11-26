
using System;
using Adic;
using Scripts.Common.Data;
using UnityEngine;

namespace Scripts.Common.ViewItems
{
    public class LobbyContentFactory : MonoBehaviour, ILobbyContentFactory
    {
        [Inject] private IUserData userData;

        [SerializeField] private GameObject[] personsPrefabs;
        [SerializeField] private Transform lobbyPersonRoot;

        private string lobbyPlayerSkin = "";
        private GameObject lobbyPlayerPerson = null;

        public bool SetupPlayerPersonFor( CurrentSkinData skinData )
        {
            GameObject personObj = GetPlayerPerson( skinData.SkinId );
            if ( personObj == null ) return false;

            CharactersBindData charBindData = personObj.GetComponent<CharactersBindData>();
            if ( charBindData == null ) return false;

            charBindData.SetSkinPart( CharactersBindData.SkinPart.Head, skinData.HeadIndex, skinData.HeadColor );
            charBindData.SetSkinPart( CharactersBindData.SkinPart.Body, skinData.BodyIndex, skinData.Costume1Color, skinData.Costume2Color );
            charBindData.SetSkinPart( CharactersBindData.SkinPart.Pants, skinData.PantsIndex, skinData.Costume1Color, skinData.Costume2Color );

            return true;
        }

        public GameObject GetPlayerPerson()
        {
            CurrentSkinData skinData = userData.GetSkin();
            return GetPlayerPerson( skinData.SkinId );
        }

        public GameObject GetPlayerPerson( string skinId )
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

        private void Start()
        {
            this.Inject();
        }
    }
}