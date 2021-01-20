
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public class GameMinimap : MonoBehaviour, IGameMinimap
	{
        [SerializeField] private RectTransform mapImage;
        [SerializeField] private GameObject[] crystalEntityPrefabs;

        private Dictionary<Transform,GameObject> dictMapView = new Dictionary<Transform, GameObject>();
        private Transform mainTr = null;
        private Coroutine updateCoroutine = null;
        private Vector2 offsetKoeff = Vector2.one;

        private IEnumerator _UpdateMainPersonPos()
        {
            while ( true )
            {
                yield return new WaitForSeconds( 0.1f );

                Vector2 pos = new Vector2( - mainTr.position.x * offsetKoeff.x, - mainTr.position.z * offsetKoeff.y );
                mapImage.anchoredPosition = pos;
            }
        }

        public void RegisterPersonage( Transform tr, MinimapEntityType entityType, int entParam = -1 )
        {
            if ( entityType == MinimapEntityType.LocalCharacter )
            {
                mainTr = tr;
                updateCoroutine = StartCoroutine( _UpdateMainPersonPos() );
            }
            else if ( entityType == MinimapEntityType.OtherCharacter )
            {
            }
            else // if ( entityType == MinimapEntityType.Crystal )
            {
                GameObject crystalPrefab = crystalEntityPrefabs[ entParam ];
                CreateEntityView( tr, crystalPrefab );
            }
        }

        private void CreateEntityView( Transform tr, GameObject prefab )
        {
            GameObject entViewObj = Instantiate( prefab, mapImage.transform );
            entViewObj.transform.localScale = Vector3.one;
            entViewObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                tr.position.x * offsetKoeff.x,
                tr.position.z * offsetKoeff.y );

            dictMapView[ tr ] = entViewObj;
        }

        public void UnregisterPersonage( Transform tr )
        {
            if ( dictMapView.ContainsKey( tr ) )
            {
                GameObject.Destroy( dictMapView[ tr ] );
                dictMapView.Remove( tr );
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

        private void Start()
        {
            Vector2 realMapSize = new Vector2( 223, 216 );
            Vector2 imgMapSize = new Vector2( 1024f, 1024f );

            offsetKoeff = new Vector2( imgMapSize.x / realMapSize.x, imgMapSize.y / realMapSize.y );
        }
    }
}
