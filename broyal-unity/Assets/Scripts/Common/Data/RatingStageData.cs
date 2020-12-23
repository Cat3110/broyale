
namespace Scripts.Common.Data
{
    public enum RatingPrizeType
    {
        Coins,
        Chest,
        Ability
    }

    public class RatingStageData
    {
        public RatingPrizeType PrizeType;
        public int Score;
        public int PrizeCoins;
        public string PrizeDesc;

        public RatingStageData( int i )
        {
            RandomSelfGenerate( i );
        }

        private void RandomSelfGenerate( int i )
        {
            this.Score = 200 * ( i + 1 );
            this.PrizeCoins = 350 * ( i + 1 );
            this.PrizeType = i < 5 ? RatingPrizeType.Coins : ( UnityEngine.Random.Range( 0, 1000 ) < 800 ? RatingPrizeType.Chest : RatingPrizeType.Ability );
            this.PrizeDesc = "Prize description";
        }
    }
}
