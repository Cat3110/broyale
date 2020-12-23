
using Scripts.Common.Data;
using TMPro;
using UnityEngine;

namespace Scripts.Scenes.Lobby.UI
{
    public class RatingStageScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI score;

        public void Setup( RatingStageData data )
        {
            score.text = data.Score.ToString();
        }
    }
}