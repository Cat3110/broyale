using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Common
{
    public class DontDestroyObject : MonoBehaviour
    {
        [SerializeField] private bool ImSingleton = true;
        [SerializeField] private string MyName;

        private static Dictionary<string,int> dicts = new Dictionary<string, int>();

        private void Awake()
        {
            GameObject.DontDestroyOnLoad( this.gameObject );

            if ( ImSingleton )
            {
                if ( dicts.ContainsKey( MyName ) )
                {
                    GameObject.DestroyObject( this.gameObject );
                    return;
                }

                dicts[ MyName ] = 1;
            }
        }
    }
}