
using System;
using SocketIO.Data.Responses;

namespace Scripts.Common.Data
{
    public interface IUserData
    {
        void Load(Action<bool> onComplete);
        
        void Save();

        CurrentSkinData GetSkin();
        void SetSkin( CurrentSkinData skinData );

        Character[] GetCharacters();
        Character GetCurrentCharacter();

        void SetCurrentCharacter(int index);
    }
}