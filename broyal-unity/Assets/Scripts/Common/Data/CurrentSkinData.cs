
using System;

namespace Scripts.Common.Data
{
    [Serializable]
    public class CurrentSkinData
    {
        public string SkinId;

        public uint HeadIndex;
        public uint BodyIndex;
        public uint PantsIndex;

        public CurrentSkinData() { }
        public CurrentSkinData( UserSkinData skinData )
        {
            this.SkinId = skinData.SkinId;

            this.HeadIndex = 0;
            this.BodyIndex = 0;
            this.PantsIndex = 0;
        }
    }
}