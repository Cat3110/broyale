
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
        public Color BodyColor;
        public Color PantsColor;

        public CurrentSkinData() { }

        public CurrentSkinData( string src )
        {
            string[] splt = src.Split( '|' );

            SkinId = splt[ 0 ];

            HeadIndex = uint.Parse( splt[ 1 ] );
            BodyIndex = uint.Parse( splt[ 2 ] );
            PantsIndex = uint.Parse( splt[ 3 ] );
        }

        public CurrentSkinData( CurrentSkinData skinData )
        {
            this.SkinId = skinData.SkinId;
            this.HeadIndex = skinData.HeadIndex;
            this.BodyIndex = skinData.BodyIndex;
            this.PantsIndex = skinData.PantsIndex;
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
            string ret = string.Format( "{0}|{1}|{2}|{3}", SkinId, HeadIndex, BodyIndex, PantsIndex );
            return ret;
        }
    }
}
