
using UnityEngine;

namespace Scripts.Scenes.Client
{
	public class DamageAreaController : MonoBehaviour
	{
        [SerializeField] private Color[] lavaColorsMult;

        private Material material;
        private float timer = 0;

        private void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        private void FixedUpdate()
        {
            timer += Time.deltaTime;

            Vector2 offset = new Vector2( timer, timer ) * 0.1f;
            material.SetTextureOffset( "_MainTex", offset );

            float colorK = ( Mathf.Sin( timer * 2f ) + 1f ) / 2f;
            Color c = lavaColorsMult[ 0 ] + ( lavaColorsMult[ 1 ] - lavaColorsMult[ 0 ] ) * colorK;
            material.SetColor( "_Color", c );
        }
    }
}
