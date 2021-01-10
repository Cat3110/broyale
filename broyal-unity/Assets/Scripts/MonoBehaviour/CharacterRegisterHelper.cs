using Scripts.Scenes.Client;
using UnityEngine;

public class CharacterRegisterHelper : MonoBehaviour
{
    private IGameMinimap minimap = null;

    private void Start()
    {
        minimap = GameObject.FindObjectOfType<GameMinimap>();
        if ( minimap != null )
        {
            minimap.RegisterPersonage( this.transform, true );
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


