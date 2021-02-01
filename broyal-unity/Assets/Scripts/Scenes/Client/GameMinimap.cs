
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public class GameMinimap : MonoBehaviour, IGameMinimap
	{
        [SerializeField] private RectTransform mapImage;
        [SerializeField] private GameObject[] crystalEntityPrefabs;
        [SerializeField] private GameObject otherPlayerPrefab;
        [SerializeField] private RectTransform[] damageZoneSides;

        private Dictionary<Transform,RectTransform> dictMapView = new Dictionary<Transform, RectTransform>();
        private Transform mainTr = null;
        private List<Transform> otherPlayersTr = new List<Transform>();
        private Coroutine updateCoroutine = null;
        private Vector2 offsetKoeff = Vector2.one;

        public void SetDeadZoneRadius( int radius )
        {
            //Debug.Log( "SetDeadZoneRadius - " + radius );

            Vector2 leftZonePos = new Vector2( -radius * offsetKoeff.x, 0 );
            damageZoneSides[ 0 ].anchoredPosition = leftZonePos;

            Vector2 rightZonePos = new Vector2( radius * offsetKoeff.x, 0 );
            damageZoneSides[ 1 ].anchoredPosition = rightZonePos;

            Vector2 downZonePos = new Vector2( 0, -radius * offsetKoeff.y );
            damageZoneSides[ 2 ].anchoredPosition = downZonePos;

            Vector2 upZonePos = new Vector2( 0, radius * offsetKoeff.y );
            damageZoneSides[ 3 ].anchoredPosition = upZonePos;
        }

        public void RegisterPersonage( Transform tr, MinimapEntityType entityType, int entParam = -1 )
        {
            if ( entityType == MinimapEntityType.LocalCharacter )
            {
                mainTr = tr;
                updateCoroutine = StartCoroutine( _UpdateCharactersPos() );
            }
            else if ( entityType == MinimapEntityType.OtherCharacter )
            {
                CreateEntityView( tr, otherPlayerPrefab );
                otherPlayersTr.Add( tr );
            }
            else // if ( entityType == MinimapEntityType.Crystal )
            {
                GameObject crystalPrefab = crystalEntityPrefabs[ entParam ];
                CreateEntityView( tr, crystalPrefab );
            }
        }

        private IEnumerator _UpdateCharactersPos()
        {
            while ( true )
            {
                yield return new WaitForSeconds( 0.1f );

                Vector2 pos = new Vector2( - mainTr.position.x * offsetKoeff.x, - mainTr.position.z * offsetKoeff.y );
                mapImage.anchoredPosition = pos;

                foreach ( var tr in otherPlayersTr )
                {
                    RectTransform trView = dictMapView[ tr ];

                    trView.anchoredPosition = new Vector2(
                        tr.position.x * offsetKoeff.x,
                        tr.position.z * offsetKoeff.y );
                }
            }
        }

        private GameObject CreateEntityView( Transform tr, GameObject prefab )
        {
            GameObject entViewObj = Instantiate( prefab, mapImage.transform );
            entViewObj.SetActive( true );
            entViewObj.transform.localScale = Vector3.one;
            entViewObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                tr.position.x * offsetKoeff.x,
                tr.position.z * offsetKoeff.y );

            dictMapView[ tr ] = entViewObj.GetComponent<RectTransform>();

            return entViewObj;
        }

        public void UnregisterPersonage( Transform tr )
        {
            if ( dictMapView.ContainsKey( tr ) )
            {
                GameObject.Destroy( dictMapView[ tr ] );
                dictMapView.Remove( tr );
            }

            if ( otherPlayersTr.Contains( tr ) )
            {
                otherPlayersTr.Remove( tr );
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
