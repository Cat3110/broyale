
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public interface IGameMinimap
	{
        void RegisterPersonage( Transform tr, bool isMain );
        void UnregisterPersonage( Transform tr );
	}
}