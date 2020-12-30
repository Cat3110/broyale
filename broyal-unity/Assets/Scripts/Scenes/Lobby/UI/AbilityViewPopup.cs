
using System;
using Adic;
using RemoteConfig;
using Scripts.Common.Factories;
using Scripts.Common.UI;
using Scripts.Core.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Scenes.Lobby.UI
{
    public class AbilityViewEventArgs : EventArgs
    {
        public SkillInfo SkillInfo;
        public GameObject abilityObj;
    }

    public class AbilityViewPopup : MonoBehaviour, ICustomEventSender
    {
        public const string EVENT_APPLY_ABILITY = "ApplyAbility";

        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private TextMeshProUGUI abilityTitle;
        [SerializeField] private AbilitySelectedItem itemView;
        [SerializeField] private Button replaceButton;

        private ICustomEventListener listener = null;
        private SkillInfo skillInfo;
        private GameObject senderObj;

        public void Setup( SkillInfo skillInfo, GameObject senderObj, bool canReplace )
        {
            this.Inject();

            this.skillInfo = skillInfo;
            this.senderObj = senderObj;

            abilityTitle.text = skillInfo.Id;
            itemView.Setup( contentFactory.GetSpriteById( skillInfo.Id ) );

            replaceButton.interactable = canReplace;
        }

        public void SetOneListener( ICustomEventListener listener )
        {
            this.listener = listener;
        }

        public void OnPressedReplaceAbility()
        {
            AbilityViewEventArgs args = new AbilityViewEventArgs();
            args.SkillInfo = skillInfo;
            args.abilityObj = senderObj;
            listener.OnEvent( this, EVENT_APPLY_ABILITY, args );
        }
    }
}
