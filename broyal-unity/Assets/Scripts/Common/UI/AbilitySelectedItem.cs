
using System;
using Scripts.Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Common.UI
{
    public class AbilitySelectedItem : MonoBehaviour, IPointerClickHandler, ICustomEventSender
    {
        public const string EVENT_TAP_LIGHTED_ABILITY = "TapLightedAbility";

        [SerializeField] private Image itemImg;
        [SerializeField] private GameObject[] unlightLightFrames;

        private ICustomEventListener listener = null;
        private bool meLighted = false;

        public void Setup( Sprite sprite )
        {
            itemImg.sprite = sprite;
        }

        public void SetLighted( bool flag )
        {
            unlightLightFrames[ 0 ].SetActive( ! flag );
            unlightLightFrames[ 1 ].SetActive( flag );

            meLighted = flag;
        }

        public void OnPointerClick( PointerEventData evData )
        {
            if ( ! meLighted ) return;

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
