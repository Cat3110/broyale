using System;
using System.Collections;
using Adic;
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.Factories;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;

////TODO: custom icon for OnScreenStick component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu("Input/Skill Button with popup Stick")]
    public class SkillButtonWithPopupStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [Inject] private IUserData userData;
        [Inject] private IGameContentFactory gameContentFactory;

        [SerializeField] private SkillType skillType;

        [SerializeField] private RectTransform parent;
        
        [SerializeField] public Image icon;
        [SerializeField] public TMPro.TMP_Text timerText;
        [SerializeField] public Image cooldownTimer;
        
        [SerializeField] private RectTransform joystickBg;
        [SerializeField] private RectTransform joystickHandler;
        
        [SerializeField] private float dragDelay = 0.150f;
        [SerializeField] private float movementRange = 50;
        [SerializeField] private float offset;
        
        [InputControl(layout = "Vector2")]
        [SerializeField] private string controlPath;
     
        private Vector3 _startPos;
        private Vector2 _pointerDownPos;

        private Coroutine waitForDrag = null;
        private bool isDragStarted = false;
        
        public float MovementRange
        {
            get => movementRange;
            set => movementRange = value;
        }
        
        public event Action<Vector2> OnAcceptAction = (x) => { };
        public event Action<Vector2> OnDragging = (x) => { };

        protected override string controlPathInternal
        {
            get => controlPath;
            set => controlPath = value;
        }
        
        private void Start()
        {
            this.Inject();

            joystickBg.gameObject.SetActive(false);

            SetupSkillIcon();
        }

        private void SetupSkillIcon()
        {
            string skillId = userData.GetCurrentCharacter().skill_set.main_skill;
            if ( skillType == SkillType.Attack ) { skillId = userData.GetCurrentCharacter().skill_set.attack_skill; }
            else if ( skillType == SkillType.Defence ) { skillId = userData.GetCurrentCharacter().skill_set.defence_skill; }
            else if ( skillType == SkillType.Utils ) { skillId = userData.GetCurrentCharacter().skill_set.utils_skill; }

            icon.sprite = gameContentFactory.GetSpriteById( skillId );
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            // if (waitForDrag != null)
            // {
            //     StopCoroutine(waitForDrag);
            // }

            //waitForDrag = StartCoroutine(WaitForDrag(dragDelay));
            //SendValueToControl(1.0f);
        }

        private IEnumerator WaitForDrag()
        {
            return null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            // RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //     parent, eventData.position, eventData.pressEventCamera, out var position);
            //
            // var delta = position - _pointerDownPos;
            // delta = Vector2.ClampMagnitude(delta, movementRange);
            //
            // joystickHandler.position  = _startPos + (Vector3)delta;
            //((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;
            //
            // PointPosition = new Vector2(
            //     (eventData.position.x - Background.position.x) / ((Background.rect.size.x - Knob.rect.size.x) / 2),
            //     (eventData.position.y - Background.position.y) / ((Background.rect.size.y - Knob.rect.size.y) / 2));
            //
            // PointPosition = (PointPosition.magnitude>1.0f)? PointPosition.normalized : PointPosition;
            //
            // Knob.transform.position = new Vector2(
            //     (PointPosition.x *((Background.rect.size.x-Knob.rect.size.x)/2)*offset) + Background.position.x, 
            //     (PointPosition.y* ((Background.rect.size.y-Knob.rect.size.y)/2) *offset) + Background.position.y);
            
            PointPosition = new Vector2(
                (eventData.position.x - joystickBg.position.x) / ((joystickBg.rect.size.x - joystickHandler.rect.size.x) / 2),
                (eventData.position.y - joystickBg.position.y) / ((joystickBg.rect.size.y - joystickHandler.rect.size.y) / 2));
     
            PointPosition = (PointPosition.magnitude > 1.0f) ? PointPosition.normalized : PointPosition;
     
            joystickHandler.position = new Vector2(
                (PointPosition.x * ((joystickBg.rect.size.x-joystickHandler.rect.size.x)/2) * offset) + joystickBg.position.x, 
                (PointPosition.y * ((joystickBg.rect.size.y-joystickHandler.rect.size.y)/2) * offset) + joystickBg.position.y);

            OnDragging?.Invoke(PointPosition);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if( !isDragStarted )
                OnAcceptAction.Invoke(Vector2.zero);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            joystickBg.gameObject.SetActive(true);
            isDragStarted = true;

            _startPos = Vector3.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, eventData.position, eventData.pressEventCamera, out _pointerDownPos);
        }

        Vector2 PointPosition;
        public void OnEndDrag(PointerEventData eventData)
        {
            joystickBg.gameObject.SetActive(false);
            isDragStarted = false;
            
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            var position = PointPosition;
            
            PointPosition = new Vector2(0f,0f);
            joystickHandler.position = joystickBg.position;

            //OnStopDrag(new Vector2(Horizontal, Vertical));
            OnAcceptAction.Invoke(position);
        }
        
        void Update () {
            Horizontal = PointPosition.x;
            Vertical = PointPosition.y;
        }

        public Vector2 Coordinate()
        {
            return new Vector2(Horizontal,Vertical);
        }
        
        [Header("Input Values")] 
        public float Horizontal = 0;
        public float Vertical = 0;

    }
}
