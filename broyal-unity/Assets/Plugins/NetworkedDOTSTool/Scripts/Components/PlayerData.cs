using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData {
        [GhostDefaultField]
        public int health;
        public int level;
        public int primarySkillId;
}
