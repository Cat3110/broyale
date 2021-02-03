using Adic;
using Scripts.Common.Data;
using Scripts.Common.Factories;

namespace Scripts.Scenes.Client
{
	public class ClientContextRoot : Adic.ContextRoot
	{

        public override void SetupContainers()
		{
			var container = this.AddContainer<InjectionContainer>();

			// global
			container.Bind<IUserData>().ToGameObject<UserDataFromServer>( "UserData" );

			// local
			container.Bind<IGameContentFactory>().ToGameObject<GameContentFactory>( "GameContentFactory" );
		}

		public override void Init()
		{
        }
	}
}