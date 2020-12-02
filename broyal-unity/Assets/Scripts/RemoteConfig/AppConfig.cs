using System.IO;
using System.Threading.Tasks;
using UniRx.Async;
using Unity.Collections;
using UnityEngine.Networking;

namespace RemoteConfig
{
    using UnityEngine;
    using FullSerializer;
    using System.Collections.Generic;
    using System;

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
    }


    public sealed class MainConfig
    {
        public string ServerAddress;
        public ushort ServerPort;
        
        public int RandomSeed;
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

    [Serializable]
    public struct SkillInfo
    {
        public string Id { get; private set; }
        public int Damage { get; private set; }
        public int Range { get; private set; }
        public float Cooldown{ get; private set; }
        
        public float PhysDMG { get; private set; }
        public float MagDMG { get; private set; }
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
        public int SkillId { get; set; }
        public CharacterInfo Character { get; set; }
        public NativeString64 UserId { get; set; }
    }
}