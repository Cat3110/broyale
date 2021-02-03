
using System.Collections;
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
            StartCoroutine( _RegisterOnTheMinimap() );
        }

        private IEnumerator _RegisterOnTheMinimap()
        {
            while ( minimap == null )
            {
                minimap = GameObject.FindObjectOfType<GameMinimap>();
                if ( minimap != null )
                {
                    minimap.RegisterPersonage( this.transform, entityType, entitySecondParameter );
                }

                yield return new WaitForSeconds( 0.1f );
            }
        }

        private void OnDisable()
        {
            ClearOnTheMinimap();
        }

        private void OnDestroy()
        {
            ClearOnTheMinimap();
        }

        private void ClearOnTheMinimap()
        {
            if ( minimap != null )
            {
                minimap.UnregisterPersonage( this.transform );
            }
        }
    }
}

