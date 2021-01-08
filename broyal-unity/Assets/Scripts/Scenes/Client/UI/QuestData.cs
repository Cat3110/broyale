
namespace Scripts.Scenes.Client.UI
{
    public class QuestData
    {
        public string Text { get; private set; }
        public int Coins { get; private set; }

        public QuestData()
        {
            Text = "lK JLK JASKJlkjsdal lasj la jslkdj lajs dlaj sldkj alks jlakjs laj sldj alskj dlakjs";
            Coins = UnityEngine.Random.Range( 10, 250 );
        }
    }
}