
using Scripts.Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Common.UI
{
    public class AbilitySelectedItem : MonoBehaviour, IPointerClickHandler, ICustomEventSender
    {
        public const string EVENT_TAP_LIGHTED_ABILITY = "TapLightedAbility";

        public AbilitySelectedItem SourceItem { get; private set; }
        public int SourceItemIndex { get; private set; }

        [SerializeField] private Image itemImg;
        [SerializeField] private GameObject[] unlightLightFrames;

        protected ICustomEventListener listener = null;
        private bool meLighted = false;

        public void Setup( Sprite sprite )
        {
            itemImg.sprite = sprite;
        }

        public void SetSourceItem( AbilitySelectedItem src, int index )
        {
            SourceItem = src;
            SourceItemIndex = index;
        }

        public void SetLighted( bool flag )
        {
            unlightLightFrames[ 0 ].SetActive( ! flag );
            unlightLightFrames[ 1 ].SetActive( flag );

            meLighted = flag;
        }

        public virtual void OnPointerClick( PointerEventData evData )
        {
            if ( listener != null )
            {
                listener.OnEvent( this, EVENT_TAP_LIGHTED_ABILITY );
            }
        }

        public void SetOneListener( ICustomEventListener listener )
        {
            this.listener = listener;
        }
    }
}
