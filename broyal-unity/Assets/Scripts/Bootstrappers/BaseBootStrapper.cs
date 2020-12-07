

using RemoteConfig;

namespace Bootstrappers
{
    using UnityEngine;
    using Utils;
    using System.Collections.Generic;
    using FullSerializer;

    class MainContainer
    {
        public static readonly IContainer Container = new Container(); 
    }
    public class BaseBootStrapper : MonoBehaviour
    {
        public MainConfig config;
        public TextAsset collidersFile;
        public static IContainer Container => MainContainer.Container;
        
        public static AppConfig OnConfigLoaded(string jsonConfig)
        {
            var appConfig = new AppConfig();
                
            if (!string.IsNullOrEmpty(jsonConfig))
            {
                appConfig.Load(jsonConfig);
            }
            
            Container.Register(appConfig);
            
            return appConfig;
        }

        public virtual void Start()
        {
            fsSerializer fsSerializer = new fsSerializer();
            var result = fsJsonParser.Parse(collidersFile.text, out fsData fsData);
            if( result.Failed ) Debug.LogError($"Unable to parse colliders info {result.FormattedMessages}");
            else
            {
                var colliders = new List<ColliderData>();
                fsSerializer.TryDeserialize( fsData, ref colliders);
                
                Container.Register(colliders);
            }
        }
    }
}