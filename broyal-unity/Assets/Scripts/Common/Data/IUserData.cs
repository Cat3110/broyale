
namespace Scripts.Common.Data
{
    public interface IUserData
    {
        CurrentSkinData GetSkin();
        void SetSkin( CurrentSkinData skinData );
    }
}