
using System;
using RemoteConfig;
using SocketIO.Data.Responses;

namespace Scripts.Common.Data
{
    public interface IUserData
    {
        void Load(Action<bool> onComplete);
        
        void Save();

        CurrentSkinData GetSkin();
        void SetSkin( CurrentSkinData skinData );
        void SetSkill( string skillId );
        Character[] GetCharacters();
        Character GetCurrentCharacter();
        User GetUserInfo();

        void SetCurrentCharacter(int index);
        void SetSkillConfig(AppConfig appConfig);
    }
}