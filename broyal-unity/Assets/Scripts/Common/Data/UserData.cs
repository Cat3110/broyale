
using System;
using RemoteConfig;
using SocketIO.Data.Responses;
using UnityEngine;

namespace Scripts.Common.Data
{
    public class UserData : MonoBehaviour, IUserData
    {
        private const string USER_PROFILE = "UserProfile";

        [SerializeField] private CurrentSkinData myCurrentSkin;

        public CurrentSkinData GetSkin()
        {
            return myCurrentSkin;
        }

        public void SetSkin( CurrentSkinData skinData )
        {
            this.myCurrentSkin = skinData;

            Save();
        }

        public void SetSkill(string skillId)
        {
            throw new NotImplementedException();
        }

        public void SetSkillByIndex( string skillId, int index )
        {
            throw new NotImplementedException();
        }

        public Character[] GetCharacters()
        {
            throw new System.NotImplementedException();
        }

        public User GetUserInfo()
        {
            throw new System.NotImplementedException();
        }

        public Character GetCurrentCharacter()
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentCharacter(int index)
        {
            throw new System.NotImplementedException();
        }

        public void SetSkillConfig(AppConfig appConfig)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            PlayerPrefs.SetString( USER_PROFILE, myCurrentSkin.ToString() );
        }

        public void Load(Action<bool> onComplete)
        {
            if ( PlayerPrefs.HasKey( USER_PROFILE ) )
            {
                string localUserProfile = PlayerPrefs.GetString( USER_PROFILE );
                myCurrentSkin = new CurrentSkinData( localUserProfile );
            }
            else
            {
                //myCurrentSkin = new CurrentSkinData();
                ColorUtility.TryParseHtmlString( "#53800b", out myCurrentSkin.HeadColor );
                ColorUtility.TryParseHtmlString( "#916ae5", out myCurrentSkin.Costume1Color );
                ColorUtility.TryParseHtmlString( "#ce9260", out myCurrentSkin.Costume2Color );
            }
            
            onComplete?.Invoke(true);
        }
    }
}
