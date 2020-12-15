
using RemoteConfig;
using Scripts.Common.Data;
using Scripts.Common.UI;
using UnityEngine;

namespace Scripts.Common.Factories
{
    public interface ILobbyContentFactory
    {
        AbilityDraggingItem CreateDraggingAbility( SkillInfo skillInfo, Transform root );
        Sprite GetSpriteById( string id );

        GameObject GetPlayerPerson();
        GameObject GetPlayerPerson( string skinId );
        bool SetupPlayerPersonFor( CurrentSkinData skinData );
    }
}