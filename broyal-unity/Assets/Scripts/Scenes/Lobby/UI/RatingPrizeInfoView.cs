
using Scripts.Common.Data;
using TMPro;
using UnityEngine;

namespace Scripts.Scenes.Lobby.UI
{
    public class RatingPrizeInfoView : MonoBehaviour
    {
        [SerializeField] private GameObject coinsBlock;
        [SerializeField] private GameObject descBlock;
        [SerializeField] private GameObject prizeCoins;
        [SerializeField] private GameObject prizeChest;
        [SerializeField] private GameObject prizeAbility;
        [SerializeField] private TextMeshProUGUI coinsValue;
        [SerializeField] private TextMeshProUGUI prizeDesc;

        public void Setup( RatingStageData data )
        {
            coinsBlock.SetActive( data.PrizeType == RatingPrizeType.Coins );
            descBlock.SetActive( data.PrizeType != RatingPrizeType.Coins );

            coinsValue.text = data.PrizeCoins.ToString();

            prizeChest.SetActive( data.PrizeType == RatingPrizeType.Chest );
            prizeAbility.SetActive( data.PrizeType == RatingPrizeType.Ability );
            prizeCoins.SetActive( data.PrizeType == RatingPrizeType.Coins );

            prizeDesc.text = data.PrizeDesc;
        }
    }
}