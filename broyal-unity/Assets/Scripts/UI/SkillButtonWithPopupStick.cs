using System;
using System.Collections;

using AnimeRx;
using RemoteConfig;
using UniRx;
using Unity.Animation;
using Unity.Mathematics;

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
        
        [Header("Input Values")] 
        public float Horizontal = 0;
        public float Vertical = 0;
     
        private Vector3 _startPos;
        private Vector2 _pointerDownPos;

        private Coroutine waitForDrag = null;
        private bool isDragStarted = false;
        private Vector2 PointPosition;

        private Button button;
        
        public float MovementRange
        {
            get => movementRange;
            set => movementRange = value;
        }
        public Vector2 Coordinate => new Vector2(Horizontal,Vertical);
        
        public Action<Vector2> OnAcceptAction = (x) => { };
        public Action<Vector2> OnDragging = (x) => { };
        public Action<Vector2> OnBeginDragging = (x) => { };

        protected override string controlPathInternal
        {
            get => controlPath;
            set => controlPath = value;
        }

        public bool IsInteractable
        {
            get => button.interactable;
            set => button.interactable = value;
        }

        public bool UseDrag { get; set; }

        private void Start()
        {
            this.Inject();
            button = GetComponent<Button>();

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

        public void StartCooldownTimer(float time)
        {
            IsInteractable = false;
            cooldownTimer.gameObject.SetActive(true);
            
            var animator = Easing.Linear(time);

            Anime.Play(0.0f, 1.0f, animator)
                .Subscribe( 
                    value => cooldownTimer.fillAmount = value,
                    onCompleted: () => { 
                        IsInteractable = true;
                        cooldownTimer.gameObject.SetActive(false); })
                .AddTo(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            if (!isDragStarted) OnAcceptAction.Invoke(Vector2.zero);

            if ( isDragStarted )
            {
                OnEndDrag( eventData );
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            PointPosition = new Vector2(
                (eventData.position.x - joystickBg.position.x) / ((joystickBg.rect.size.x - joystickHandler.rect.size.x) / 2),
                (eventData.position.y - joystickBg.position.y) / ((joystickBg.rect.size.y - joystickHandler.rect.size.y) / 2));

            OnBeginDrag( eventData );
            OnBeginDragging?.Invoke( PointPosition );
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            if (!UseDrag) return;
            
            joystickBg.gameObject.SetActive(true);
            isDragStarted = true;

            _startPos = Vector3.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent, eventData.position, eventData.pressEventCamera, out _pointerDownPos);

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            if (!UseDrag) return;
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

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsInteractable) return;
            if (!UseDrag) return;
            
            joystickBg.gameObject.SetActive(false);
            isDragStarted = false;
            
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            var position = PointPosition;
            
            PointPosition = new Vector2(0f,0f);
            joystickHandler.position = joystickBg.position;

            //OnStopDrag(new Vector2(Horizontal, Vertical));
            OnAcceptAction.Invoke(position);
        }
        
        private void Update () {
            Horizontal = PointPosition.x;
            Vertical = PointPosition.y;
        }
    }
}
