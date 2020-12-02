using Adic;
using Scripts.Common.Data;
using Scripts.Common.ViewItems;

namespace Scripts.Scenes.Lobby
{
	public class LobbyContextRoot : Adic.ContextRoot
	{

        public override void SetupContainers()
		{
			var container = this.AddContainer<InjectionContainer>();

			// global
			container.Bind<IUserData>().ToGameObject<UserDataFromServer>( "UserData" );
			container.Bind<IGameData>().ToGameObject<GameData>( "GameData" );

			// local
			container.Bind<ILobbyContentFactory>().ToGameObject<LobbyContentFactory>( "LobbyContentFactory" );
		}

		public override void Init()
		{
        }
	}
}