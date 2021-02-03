
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Common.Factories
{
    public class GameContentFactory : MonoBehaviour, IGameContentFactory
    {
        [SerializeField] private List<SkillIdToSprite> namedSprites;
        [SerializeField] private Sprite defaultAbilityTexture;

        public Sprite GetSpriteById( string id )
        {
            if ( string.IsNullOrEmpty( id ) ) return defaultAbilityTexture;

            return namedSprites.FirstOrDefault( i => i.Id == id )?.Sprite;
        }
    }
}