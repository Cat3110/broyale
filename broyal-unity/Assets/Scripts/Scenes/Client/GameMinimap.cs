
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public class GameMinimap : MonoBehaviour, IGameMinimap
	{
        [SerializeField] private RectTransform mapImage;

        private List<Transform> trackPersons = new List<Transform>(); // first is a main
        private Transform mainTr;
        private Coroutine updateCoroutine = null;

        private IEnumerator _UpdateMainPersonPos()
        {
            while ( true )
            {
                yield return new WaitForSeconds( 0.1f );

                Vector2 offsetKoeff = new Vector2( 2f, 2f );
                Vector2 pos = new Vector2( - mainTr.position.x * offsetKoeff.x, - mainTr.position.z * offsetKoeff.y );
                mapImage.anchoredPosition = pos;
            }
        }

        public void RegisterPersonage( Transform tr, bool isMain )
        {
            trackPersons.Add( tr );

            if ( isMain )
            {
                mainTr = tr;

                updateCoroutine = StartCoroutine( _UpdateMainPersonPos() );
            }
        }

        public void UnregisterPersonage( Transform tr )
        {
            if ( trackPersons.Contains( tr ) )
            {
                trackPersons.Remove( tr );
            }

            if ( mainTr == tr )
            {
                mainTr = null;

                if ( updateCoroutine != null )
                {
                    StopCoroutine( updateCoroutine );
                    updateCoroutine = null;
                }
            }
        }
	}
}
