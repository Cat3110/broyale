
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Common.Tools.UI
{
    public class CharacterRotator : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        [SerializeField] private Transform trCharacter;
        [SerializeField] private GameObject pressedTarget;
        [SerializeField] private string pressedTargetCommand;

        private float curSpeed = 0f;
        private Vector2 startPos;
        private float startAngle = 0f;
        private float startTime = 0f;

        public void OnPointerDown( PointerEventData eventData )
        {
            startTime = Time.time;
            startPos = eventData.position;
            if ( trCharacter != null )
            {
                startAngle = trCharacter.rotation.eulerAngles.y;
            }
        }

        public void OnDrag( PointerEventData eventData )
        {
            float posXDelta = eventData.position.x - startPos.x;

            float rotValue = startAngle - posXDelta * 0.2f;
            if ( trCharacter != null )
            {
                Quaternion qRot = Quaternion.identity;
                qRot.eulerAngles = new Vector3( 0, rotValue, 0 );
                trCharacter.rotation = qRot;
            }
        }

        public void OnEndDrag( PointerEventData eventData )
        {
        }

        public void OnPointerUp( PointerEventData eventData )
        {
            if ( Time.time - startTime < 0.5f && Mathf.Abs( trCharacter.eulerAngles.y - startAngle ) < 1f && pressedTarget != null && pressedTargetCommand != "" )
            {
                pressedTarget.SendMessage( pressedTargetCommand, SendMessageOptions.DontRequireReceiver );
            }
        }

        public void SetCharacter( Transform ch )
        {
            if ( trCharacter == ch ) return;

            this.trCharacter = ch;

            if ( this.trCharacter != null )
            {
                startAngle = this.trCharacter.rotation.eulerAngles.y;
            }
        }
    }
}
