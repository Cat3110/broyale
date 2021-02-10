

using Scripts.Core.Common;
using TMPro;
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public class GameTopPanel : MonoBehaviour, IGameTopPanel
	{
        [SerializeField] private TextMeshProUGUI timerTxt;

        public void SetCountdownValue( int val )
        {
            timerTxt.text = Converter.SecondsToTimeString( val );
        }

        private void Start()
        {
            timerTxt.text = "";
        }
    }
}