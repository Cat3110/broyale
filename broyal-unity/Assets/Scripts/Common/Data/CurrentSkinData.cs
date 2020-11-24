
using System;
using UnityEngine;

namespace Scripts.Common.Data
{
    [Serializable]
    public class CurrentSkinData
    {
        public string SkinId;

        public uint HeadIndex;
        public uint BodyIndex;
        public uint PantsIndex;

        public Color HeadColor;
        public Color Costume1Color;
        public Color Costume2Color;

        public CurrentSkinData() { }

        public CurrentSkinData( string src )
        {
            string[] splt = src.Split( '|' );

            SkinId = splt[ 0 ];

            HeadIndex = uint.Parse( splt[ 1 ] );
            BodyIndex = uint.Parse( splt[ 2 ] );
            PantsIndex = uint.Parse( splt[ 3 ] );

            if ( splt.Length > 6 )
            {
                ColorUtility.TryParseHtmlString( "#" + splt[ 4 ], out HeadColor );
                ColorUtility.TryParseHtmlString( "#" + splt[ 5 ], out Costume1Color );
                ColorUtility.TryParseHtmlString( "#" + splt[ 6 ], out Costume2Color );
            }
        }

        public CurrentSkinData( CurrentSkinData skinData )
        {
            this.SkinId = skinData.SkinId;

            this.HeadIndex = skinData.HeadIndex;
            this.BodyIndex = skinData.BodyIndex;
            this.PantsIndex = skinData.PantsIndex;

            this.HeadColor = skinData.HeadColor;
            this.Costume1Color = skinData.Costume1Color;
            this.Costume2Color = skinData.Costume2Color;
        }

        public CurrentSkinData( UserSkinData skinData )
        {
            this.SkinId = skinData.SkinId;

            this.HeadIndex = 0;
            this.BodyIndex = 0;
            this.PantsIndex = 0;
        }

        public override string ToString()
        {
            string ret = string.Format( "{0}|{1}|{2}|{3}|{4}|{5}|{6}", SkinId, HeadIndex, BodyIndex, PantsIndex,
                ColorUtility.ToHtmlStringRGB( HeadColor ),
                ColorUtility.ToHtmlStringRGB( Costume1Color ),
                ColorUtility.ToHtmlStringRGB( Costume2Color )
            );
            return ret;
        }
    }
}
