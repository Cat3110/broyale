using System;
using System.Collections.Generic;
using System.Linq;
using Adic;
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.UI;
using UnityEngine;

namespace Scripts.Common.Factories
{
    [Serializable]
    public class SkillIdToSprite
    {
        public string Id;
        public Sprite Sprite;
    }

    public class LobbyContentFactory : MonoBehaviour, ILobbyContentFactory
    {
        [Inject] private IUserData userData;

        [SerializeField] private GameObject[] personsPrefabs;
        [SerializeField] private Transform lobbyPersonRoot;

        [SerializeField] private List<SkillIdToSprite> namedSprites;
        [SerializeField] private GameObject abilityDraggingItemPrefab;
        [SerializeField] private Sprite defaultAbilityTexture;

        private string lobbyPlayerSkin = "";
        private GameObject lobbyPlayerPerson = null;

        public AbilityDraggingItem CreateDraggingAbility( SkillInfo skillInfo, Transform root )
        {
            var skillSprite = GetSpriteById( skillInfo.Id );

            GameObject newSkillObj = Instantiate( abilityDraggingItemPrefab, root );
            AbilityDraggingItem ret = newSkillObj.GetComponent<AbilityDraggingItem>();
            ret.Setup( skillInfo, skillSprite );

            return ret;
        }

        public bool SetupPlayerPersonFor( CurrentSkinData skinData )
        {
            GameObject personObj = GetPlayerPerson( skinData.SkinId );
            if ( personObj == null ) return false;

            CharactersBindData charBindData = personObj.GetComponent<CharactersBindData>();
            if ( charBindData == null ) return false;
            charBindData.SetSkinData( skinData );
            return true;
        }

        public Sprite GetSpriteById( string id )
        {
            if ( string.IsNullOrEmpty( id ) ) return defaultAbilityTexture;

            return namedSprites.FirstOrDefault( i => i.Id == id )?.Sprite;
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