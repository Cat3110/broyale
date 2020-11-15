
using UnityEngine;

namespace Scripts.Common.Data
{
    public class UserData : MonoBehaviour, IUserData
    {
        [SerializeField] private CurrentSkinData myCurrentSkin;

        public CurrentSkinData GetSkin()
        {
            return myCurrentSkin;
        }

        public void SetSkin( CurrentSkinData skinData )
        {
            this.myCurrentSkin = skinData;
        }
    }
}
