namespace Utils
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IAssetManager
    {
        void LoadAssetByNameAsync(string name, Action<GameObject> callback);
        void LoadAssetByIdAsync(uint assetRefId, Action<GameObject> callback);
    }
    public class AssetsManager : IAssetManager
    {
        private MainConfig _config;
        private ILogger _logger;

        private Dictionary<int, GameObject> assetsCache;
   
        public AssetsManager(MainConfig config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }
   
        public void LoadAssetByNameAsync(string name, Action<GameObject> callback)
        {
            var nameId = _config.GetNameId(name);
            LoadAssetByIdAsync(nameId.Id, callback);
        }
   
        public void LoadAssetByIdAsync(uint assetRefId, Action<GameObject> callback)
        {
            _logger.Log($"{nameof(AssetsManager)} LoadAssetById{assetRefId}");

            var loadOperation = _config.AssetRefMembers[assetRefId].InstantiateAsync();
            loadOperation.Completed += (status) =>
            {
                _logger.Log($"{nameof(AssetsManager)} InstantiateAsync => {assetRefId} {status.Result}");
                callback(status.Result);
            };
        }
    }
}



