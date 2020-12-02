using System;
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

        public Character[] GetCharacters() => characters;
        public Character GetCurrentCharacter() => _currentCharacter;
        public void SetCurrentCharacter(int index) => _currentCharacter = characters[index];

        public void Save()
        {
            _socket.SetCharacter(_currentCharacter, (response) =>
            {
                Debug.Log($"SetCharacters => {response}");
            }, () => Debug.LogError("SetCharacter failed"));
        }

        public void Load(Action<bool> onComplete)
        {
            _socket.GetCharacters("", (characters) =>
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
        }
    }
}
