
using UnityEngine;

namespace Scripts.Scenes.Client
{
    public enum MinimapEntityType
    {
        LocalCharacter,
        OtherCharacter,
        Crystal
    }

    public class EntityMinimapViewHelper : MonoBehaviour
    {
        [SerializeField] private MinimapEntityType entityType;
        [SerializeField] private int entitySecondParameter;

        private IGameMinimap minimap = null;

        public void SetEntityType( MinimapEntityType type )
        {
            entityType = type;
        }

        private void Start()
        {
            minimap = GameObject.FindObjectOfType<GameMinimap>();
            if ( minimap != null )
            {
                minimap.RegisterPersonage( this.transform, entityType, entitySecondParameter );
            }
        }

        private void OnDestroy()
        {
            if ( minimap != null )
            {
                minimap.UnregisterPersonage( this.transform );
            }
        }
    }
}

