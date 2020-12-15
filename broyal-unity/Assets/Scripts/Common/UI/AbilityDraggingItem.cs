
using System;
using RemoteConfig;
using Scripts.Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Scripts.Common.UI
{
    public class AbilityEventArgs : EventArgs
    {
        public SkillInfo SkillInfo { get; private set; }

        public AbilityEventArgs( SkillInfo skillInfo )
        {
            this.SkillInfo = skillInfo;
        }
    }

    public class AbilityDraggingItem : MonoBehaviour, ICustomEventSender, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public const string EVENT_DRAGGING_START = "DraggingStartEvent";
        public const string EVENT_DRAGGING_END = "DraggingEndEvent";
        public const string EVENT_SHOW_ABILITY_INFO = "ShowAbilityInfo";

        [SerializeField] private AbilitySelectedItem itemView;

        private ICustomEventListener listener = null;
        private bool itsLongPressed = false;
        private bool dragModeNow = false;
        private SkillInfo skillInfo;
        private PointerEventData pointerEvData;

        public SkillInfo GetMySkillInfo() { return skillInfo; }

        public void Setup( SkillInfo skillInfo, Sprite sprite )
        {
            this.skillInfo = skillInfo;

            itemView.Setup( sprite );
        }

        public void SetDragMode( bool mode )
        {
            itsLongPressed = false;
            dragModeNow = mode;
        }

        private void FixedUpdate()
        {
            if ( dragModeNow )
            {
#if UNITY_EDITOR // && FALSE
                if ( Mouse.current.press.isPressed )
                {
                    Vector2 pos = Mouse.current.position.ReadValue();
                    this.transform.position = pos;
                }
#else
                if ( Touchscreen.current.press.isPressed )
                {
                    Vector2 pos = Touchscreen.current.position.ReadValue();
                    this.transform.position = pos;
                }
#endif
                else
                {
                    listener.OnEvent( this, EVENT_DRAGGING_END );
                }
            }
        }

        public void SetOneListener( ICustomEventListener listener )
        {
            this.listener = listener;
        }

        public void OnDrag( PointerEventData evData )
        {
            if ( ! dragModeNow ) return;

            this.transform.position = evData.worldPosition;
        }

        public void OnPointerClick( PointerEventData evData )
        {
            if ( dragModeNow ) return;

            if ( !itsLongPressed )
            {
                listener.OnEvent( this, EVENT_SHOW_ABILITY_INFO, new AbilityEventArgs( this.skillInfo ) );
            }
        }

        public void OnPointerDown( PointerEventData evData )
        {
            if ( dragModeNow ) return;

            pointerEvData = evData;

            Invoke( "WaitLongPressed", 0.5f );
        }

        public void OnPointerUp( PointerEventData evData )
        {
            if ( dragModeNow ) return;

            CancelInvoke( "WaitLongPressed" );
        }

        private void WaitLongPressed()
        {
            pointerEvData.pointerDrag = null;
            pointerEvData.dragging = false;
            EventSystem.current.SetSelectedGameObject( null );

            itsLongPressed = true;
            listener.OnEvent( this, EVENT_DRAGGING_START );
        }
    }
}
