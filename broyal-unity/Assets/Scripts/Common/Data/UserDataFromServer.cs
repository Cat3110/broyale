using System;
using System.Linq;
using RemoteConfig;
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
        private User _userInfo;
        private AppConfig _appConfig;

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
            var skillInfo = _appConfig.GetSkillConfigById(skillId);
            switch (skillInfo.Type)
            {
                case SkillType.Main:
                    _currentCharacter.skill_set.main_skill = skillId;
                    break;
                case SkillType.Attack:
                    _currentCharacter.skill_set.attack_skill = skillId;
                    break;
                case SkillType.Defence:
                    _currentCharacter.skill_set.defence_skill = skillId;
                    break;
                case SkillType.Passive:
                    _currentCharacter.skill_set.passive_skill = skillId;
                    break;
                case SkillType.Utils:
                    _currentCharacter.skill_set.utils_skill = skillId;
                    break;
                default:
                    Debug.LogError($"Unknown skill type:{skillInfo.Type}");
                    return;
            }
            
            Save();
        }

        public User GetUserInfo() => _userInfo;
        public Character[] GetCharacters() => characters;
        public Character GetCurrentCharacter() => _currentCharacter;
        public void SetCurrentCharacter(int index) => _currentCharacter = characters[index];
        public void SetSkillConfig(AppConfig appConfig)
        {
            _appConfig = appConfig;
        }

        public void Save()
        {
            _socket.SetUserInfo( _userInfo, ( resp ) =>
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
                _userInfo = resp;
                Debug.Log($"userInfo => {_userInfo}");

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
