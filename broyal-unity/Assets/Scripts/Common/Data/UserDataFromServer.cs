﻿using System;
using System.Linq;
using SocketIO;
using SocketIO.Data.Responses;
using SocketIOExt;
using UnityEngine;

namespace Scripts.Common.Data
{
    public class UserDataFromServer : MonoBehaviour, IUserData
    {
        [SerializeField] private Character[] characters;

        private Character _currentCharacter;
        private SocketIOComponent _socket;
        private User userInfo;

        private void Awake()
        {
            _socket = FindObjectOfType<SocketIOComponent>();
        }

        public CurrentSkinData GetSkin() => new CurrentSkinData(_currentCharacter);

        public void SetSkin( CurrentSkinData skinData )
        {
            skinData.ToCharacterData(ref _currentCharacter);
            Save();
        }

        public void SetSkill(string skillId)
        {
            _currentCharacter.skill_set.main_skill = skillId;
            Save();
        }

        public User GetUserInfo() => userInfo;
        public Character[] GetCharacters() => characters;
        public Character GetCurrentCharacter() => _currentCharacter;
        public void SetCurrentCharacter(int index) => _currentCharacter = characters[index];

        public void Save()
        {
            _socket.SetUserInfo( userInfo, ( resp ) =>
            {
                Debug.Log($"SetUserInfo => { resp }");
            },
            () => Debug.LogError("SetUserInfo failed") );

            _socket.SetCharacter(_currentCharacter, (response) =>
            {
                Debug.Log($"SetCharacters => {response}");
            },
            () => Debug.LogError("SetCharacter failed"));
        }

        public void Load( Action<bool> onComplete )
        {
            string deviceId = Constants.GetDeviceID();
            _socket.LoginWithDeviceId( deviceId, ( resp ) =>
            {
                userInfo = resp;
                Debug.Log($"userInfo => {userInfo}");

                _socket.GetCharacters( "", ( characters ) =>
                {
                    Debug.Log($"GetCharacters => {characters}");
                    this.characters = characters;
                    _currentCharacter = this.characters.First();
                    onComplete?.Invoke(true);
                }, () =>
                {
                    Debug.LogError("GetCharacters failed");
                    onComplete?.Invoke(false);
                });
            },
            () =>
            {
                Debug.LogError("LoginWithDeviceId failed");
                onComplete?.Invoke( false );
            } );
        }
    }
}
