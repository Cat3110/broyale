namespace RemoteConfig
{
    using UnityEngine;
    using FullSerializer;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using UniRx.Async;
    using Unity.Collections;
    using UnityEngine.Networking;

    public sealed class AppConfig
    { 
        public string LastDataFile => Path.Combine(Application.persistentDataPath, "lastdata.json");
        public const string DefaultConfigUrl = "https://script.google.com/macros/s/AKfycbyFwY0nil6QFV5-IKaDkF-KY2LRI92Rcttk-t1uR8uKMsNivKg/exec";
        
        public MainConfig Main { get; private set; } = new MainConfig();
        public LocalizationConfig Localization { get; private set; } = new LocalizationConfig();
        public CharactersConfig Characters { get; private set; } = new CharactersConfig();
        public SkillsConfig Skills { get; private set; } = new SkillsConfig();
        public ItemsConfig Items { get; private set; } = new ItemsConfig();
        
        public CommonConfig Common { get; private set; } = new CommonConfig();

        private fsSerializer fsSerializer = new fsSerializer();

        public static async UniTask<string> LoadByUrlAsync(string url = DefaultConfigUrl)
        { 
            await UniTask.SwitchToMainThread();
            
            using (var req = UnityWebRequest.Get(url))
            {
                var op = await req.SendWebRequest();
                if (string.IsNullOrEmpty(op.error))
                {
                    Debug.Log("[App] LoadByUrl Result:" + op.downloadHandler.text);
                    return op.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("[App] LoadByUrl Result:" + op.error);
                    return null;
                }
            }  
        }
        
        public bool Load(string data)
        {      
            var fsResult = fsJsonParser.Parse(data, out fsData fsData);
            if (fsResult.Succeeded)
            {               
                var sheets = fsData.AsDictionary;

                LoadTable(sheets, "Main", (MainConfig result) => 
                { 
                    Main = result;
                    
                    if(Main.RandomSeed > 0) UnityEngine.Random.InitState(Main.RandomSeed);
                    else UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
                });
                
                LoadTable(sheets, "Characters", (CharactersConfig result) => Characters = result);
                LoadTable(sheets, "Skills", (SkillsConfig result) => Skills = result);
                //LoadTable(sheets, "Localization", (LocalizationConfig result) => Localization = result);
                LoadTable(sheets, "Items", (ItemsConfig result) => Items = result);
                //LoadTable(sheets, "Common", (CommonConfig result) => Common = result);               
            }
            Debug.Log("[AppConfig] Load Result:" + fsResult.Succeeded);
            return fsResult.Succeeded;
        }

        private fsResult LoadTable<T>(Dictionary<string, fsData> sheets, string name, Action<T> onSucceeded ) where T : class, new()
        {
            var t = new T();
            var fsResult = fsSerializer.TryDeserialize(sheets[name], ref t);
            if (fsResult.Succeeded)
            {
                Debug.Log($"[AppConfig] LoadTable({name}) => Succeeded");
                onSucceeded?.Invoke(t);
            }
            else Debug.LogWarning($"[AppConfig] LoadTable({name}):{fsResult.FormattedMessages}");

            return fsResult;
        }

        public SkillInfo GetSkillByAttackType(int attackType) => Skills[attackType - 1];

        public SkillInfo GetSkillConfigById(string skillId) => Skills.Find(s => s.Id == skillId);
        public int GetSkillIndex(SkillInfo skillInfo) => Skills.FindIndex(s => s.Id == skillInfo.Id);
    }


    public sealed class MainConfig
    {
        public string ServerAddress;
        public ushort ServerPort;
        
        public int RandomSeed;
        public string LobbyAddress;

        public float MapAreaSize;
        public float MapAreaDecreaseTick;
        public float MapAreaDecreaseAmount;
    }

    public sealed class LocalizationConfig
    {

    }
    public sealed class CharactersConfig : List<CharacterInfo>
    {

    }
    public sealed class SkillsConfig : List<SkillInfo>
    {

    }
    
    public sealed class ItemsConfig : List<ItemInfo>
    {
        
    }
    
    public sealed class CommonConfig : List<fsData>
    {

    }

    public enum AimType{
        None = 0,
        Area = 1,
        Dot = 2,
        Direction = 3,
        Sector = 4,
        Trajectory = 5
    }
    
    public enum SkillType{
        Main = 0,
        Attack,
        Defence,
        Utils,
        Passive
    }
    
    [Serializable]
    public struct SkillInfo
    {
        public string Id { get; private set; }
        public float Range { get; private set; }
        public float Radius { get; set; }
        public float Cooldown{ get; private set; }
        
        public float PhysDMG { get; private set; }
        public float PhysDEffect { get; set; }
        public float MagDMG { get; private set; }
        public float MagDEffect { get; set; }

        public float SpeedEffect { get; set; }
        public float HPEffect { get; set; }
        
        public float ImpactTime { get; set; }
        public float Heal { get; set; }
        public float PhysArmor { get; set; }
        public float MagArmor { get; set; }
        public float Aiming { get; set; }
        
        public SkillType Type { get; set; }
        public AimType AimType { get; set; }
        public bool IsEnabled { get; set; }
    }
    
    [Serializable]
    public class CharacterInfo
    {
        public string Id;
        
        public float Speed;
        public int Health;
        public uint SkinType;
        
        public int HealthRegen;
        public int Power;
        public int Magic;	
    }
    
    public class ItemInfo
    {
        public string Id;
        
        public int Power;
        public int Magic;
        public int Health;
    }

    public class Session
    {
        public SkillInfo MainSkill { get; set; }
        public SkillInfo AttackSkill { get; set; }
        public SkillInfo DefenceSkill { get; set; }
        public SkillInfo UtilitySkill { get; set; }
        public SkillInfo PassiveSkill { get; set; }
        
        public CharacterInfo Character { get; set; }
        public NativeString64 UserId { get; set; }
    }
}