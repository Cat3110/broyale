using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct PlayerData : IComponentData {
        [GhostDefaultField]
        public int health;

        public int maxHealth;
        public int level;
        
        public int primarySkillId;
        public int attackSkillId;
        public int defenceSkillId;
        
        public int magic;
        public int power;

        public uint inventory;

        public int damageRadius;
}


