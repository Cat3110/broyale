using Unity.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenStick component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu("Input/On-Screen popup Stick")]
    public class OnScreenPopupStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;
        
        [SerializeField]
        private GameObject graphic;
        
        [SerializeField]
        private GameObject stick;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));
            
            graphic.SetActive(true);
            
            m_StartPos = eventData.pressPosition;

            ((RectTransform)graphic.transform).position = m_StartPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            
            ((RectTransform)stick.transform).position  = m_StartPos + (Vector3)delta;
            //((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            //Debug.Log(newPos);
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            graphic.SetActive(false);
            
            //((RectTransform)transform).anchoredPosition = m_StartPos;
            SendValueToControl(Vector2.zero);
        }

        /*private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;
        }*/
    }
}
