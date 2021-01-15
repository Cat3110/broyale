
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public interface IGameMinimap
	{
        void RegisterPersonage( Transform tr, MinimapEntityType entityType, int entParam = -1 );
        void UnregisterPersonage( Transform tr );
	}
}