
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Common.UI
{
    public class AbilitySelectedItem : MonoBehaviour
    {
        [SerializeField] private Image itemImg;
        [SerializeField] private GameObject[] unlightLightFrames;

        public void Setup( Sprite sprite )
        {
            itemImg.sprite = sprite;
        }

        public void SetLighted( bool flag )
        {
            unlightLightFrames[ 0 ].SetActive( ! flag );
            unlightLightFrames[ 1 ].SetActive( flag );
        }
    }
}
