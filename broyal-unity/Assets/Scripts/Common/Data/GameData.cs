
using UnityEngine;

namespace Scripts.Common.Data
{
    public class GameData : MonoBehaviour, IGameData
    {
        public UserSkinData[] Skins { get { return skins; } }

        [SerializeField] private UserSkinData[] skins;
    }
}