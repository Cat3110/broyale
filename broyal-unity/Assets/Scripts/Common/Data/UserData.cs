﻿
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

        public void Save()
        {
            PlayerPrefs.SetString( USER_PROFILE, myCurrentSkin.ToString() );
        }

        public void Load()
        {
            if ( PlayerPrefs.HasKey( USER_PROFILE ) )
            {
                string localUserProfile = PlayerPrefs.GetString( USER_PROFILE );
                myCurrentSkin = new CurrentSkinData( localUserProfile );
            }
        }
    }
}