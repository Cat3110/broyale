
namespace Scripts.Common.Data
{
    public interface IUserData
    {
        void Load();
        void Save();

        CurrentSkinData GetSkin();
        void SetSkin( CurrentSkinData skinData );
    }
}