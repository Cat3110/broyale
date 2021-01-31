
namespace Scripts.Scenes.Client.UI
{
    public class RewardData
    {
        public QuestData[] questDatas { get; private set; }
        public int SkillReward { get; private set; }
        public int CoinsReward { get; private set; }
        public int Place { get; private set; }

        public RewardData()
        {
            GenerateRandomReward();
        }

        private void GenerateRandomReward()
        {
            Place = UnityEngine.Random.Range( 1, 4 );

            int countQuests = UnityEngine.Random.Range( 1, 3 + 1 );
            questDatas = new QuestData[ countQuests ];
            for ( int i = 0; i < countQuests; i++ )
            {
                questDatas[ i ] = new QuestData();
            }

            SkillReward = UnityEngine.Random.Range( 10, 95 );
            CoinsReward = 10 * UnityEngine.Random.Range( 3, 20 );
        }
    }
}
