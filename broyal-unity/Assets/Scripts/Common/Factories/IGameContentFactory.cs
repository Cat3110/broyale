
using UnityEngine;

namespace Scripts.Common.Factories
{
    public interface IGameContentFactory
    {
        Sprite GetSpriteById( string id );
    }
}