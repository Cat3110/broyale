
using Adic;
using RemoteConfig;
using Scripts.Common.Factories;
using Scripts.Common.UI;
using TMPro;
using UnityEngine;

namespace Scripts.Scenes.Lobby.UI
{
    public class AbilityViewPopup : MonoBehaviour
    {
        [Inject] private ILobbyContentFactory contentFactory;

        [SerializeField] private TextMeshProUGUI abilityTitle;
        [SerializeField] private AbilitySelectedItem itemView;

        public void Setup( SkillInfo skillInfo )
        {
            this.Inject();

            abilityTitle.text = skillInfo.Id;
            itemView.Setup( contentFactory.GetSpriteById( skillInfo.Id ) );
        }
    }
}