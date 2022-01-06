using System;
using System.Collections.Generic;
using LuaInterface;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;
using Object = UnityEngine.Object;

namespace PluginSet.Lua
{
    public static class ResourcesLuaWrapper
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("ResourcesLuaWrapper");
        
        public static string PatchesRootPath => ResourcesManager.PatchesRootPath;
        public static string PatchesSavePath => ResourcesManager.PatchesSavePath;

        public static string StreamingInternalUrlPrefix => ResourcesManager.StreamingInternalUrlPrefix;

        public static string PatchesSavePathUrlPrefix => ResourcesManager.PatchesSavePathUrlPrefix;
        
        public static string ResourceVersion => ResourcesManager.ResourceVersion;

        public static string RunningVersion => ResourcesManager.Instance.RunningVersion;
        
        public static string StreamingAssetsName => ResourcesManager.Instance.StreamingAssetsName;

        public static List<string> SearchPaths => ResourcesManager.Instance.SearchPaths;

        public static void AddSearchPath(string searchPath, bool front = false)
        {
            ResourcesManager.Instance.AddSearchPath(searchPath, front);
        }

        public static void RemoveSearchPath(string searchPath)
        {
            ResourcesManager.Instance.RemoveSearchPath(searchPath);
        }

        public static Object Load(string path)
        {
            return ResourcesManager.Instance.Load<Object>(path);
        }

        public static Object Load(string path, Type type)
        {
            return ResourcesManager.Instance.Load(path, type);
        }
        

        public static AsyncOperationHandle LoadAsync(string path)
        {
            return ResourcesManager.Instance.LoadAsync<Object>(path);
        }
        
        public static AsyncOperationHandle LoadAsync(string path, Type type)
        {
            return ResourcesManager.Instance.LoadAsync(path, type);
        }

        public static bool ExistsBundle(string bundleName)
        {
            return ResourcesManager.Instance.ExistsBundle(bundleName);
        }

        public static AssetBundle LoadBundle(string bundleName)
        {
            return ResourcesManager.Instance.LoadBundle(bundleName);
        }

        public static AsyncOperationHandle LoadBundleAsync(string bundleName)
        {
            return ResourcesManager.Instance.LoadBundleAsync(bundleName);
        }

        public static void ReleaseBundle(AssetBundle bundle)
        {
            ResourcesManager.Instance.ReleaseBundle(bundle);
        }

        public static void DontReleaseBundle(AssetBundle bundle)
        {
            ResourcesManager.Instance.DontReleaseBundle(bundle);
        }

        public static void ReleaseBundle(string bundleName)
        {
            ResourcesManager.Instance.ReleaseBundle(bundleName);
        }

        public static void DontReleaseBundle(string bundleName)
        {
            ResourcesManager.Instance.DontReleaseBundle(bundleName);
        }

        public static void ReleaseAll()
        {
            ResourcesManager.Instance.ReleaseAll();
        }

        public static Object LoadAsset(string bundleName, string assetName)
        {
            return ResourcesManager.Instance.LoadAsset<Object>(bundleName, assetName);
        }
        
        public static Object LoadAsset(string bundleName, string assetName, Type type)
        {
            return ResourcesManager.Instance.LoadAsset(bundleName, assetName, type);
        }

        public static AsyncOperationHandle<Object> LoadAssetAsync(string bundleName, string assetName)
        {
            return ResourcesManager.Instance.LoadAssetAsync<Object>(bundleName, assetName);
        }
        
        public static AsyncOperationHandle<Object> LoadAssetAsync(string bundleName, string assetName, Type type)
        {
            return ResourcesManager.Instance.LoadAssetAsync(bundleName, assetName, type);
        }

        public static Object[] LoadAllAssets(string bundleName)
        {
            return ResourcesManager.Instance.LoadAllAssets<Object>(bundleName);
        }
        
        public static Object[] LoadAllAssets(string bundleName, Type type)
        {
            return ResourcesManager.Instance.LoadAllAssets(bundleName, type);
        }

        public static AsyncOperationHandle<Object[]> LoadAllAssetsAsync(string bundleName)
        {
            return ResourcesManager.Instance.LoadAllAssetsAsync<Object>(bundleName);
        }
        
        public static AsyncOperationHandle<Object[]> LoadAllAssetsAsync(string bundleName, Type type)
        {
            return ResourcesManager.Instance.LoadAllAssetsAsync(bundleName, type);
        }
        
        /// <summary>
        /// 增加回调接口
        /// </summary>
        
        public static AsyncOperationHandle LoadAsync(string path, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAsync<Object>(path);
            async.OnGetResult += delegate(Object result)
            {
                TryCallback<Object>(callback, result);
            };
            return async;
        }
        
        public static AsyncOperationHandle LoadAsync(string path, Type type, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAsync(path, type);
            async.OnGetResult += delegate(Object result)
            {
                TryCallback<Object>(callback, result);
            };
            return async;
        }
        
        public static AsyncOperationHandle LoadBundleAsync(string bundleName, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadBundleAsync(bundleName);
            async.OnGetResult += delegate(AssetBundle bundle)
            {
                TryCallback<AssetBundle>(callback, bundle);
            };
            return async;
        }

        public static AsyncOperationHandle<Object> LoadAssetAsync(string bundleName, string assetName, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAssetAsync<Object>(bundleName, assetName);
            async.OnGetResult += delegate(Object result)
            {
                TryCallback<Object>(callback, result);
            };
            return async;
        }
        
        public static AsyncOperationHandle<Object> LoadAssetAsync(string bundleName, string assetName, Type type, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAssetAsync(bundleName, assetName, type);
            async.OnGetResult += delegate(Object result)
            {
                TryCallback<Object>(callback, result);
            };
            return async;
        }

        public static AsyncOperationHandle<Object[]> LoadAllAssetsAsync(string bundleName, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAllAssetsAsync<Object>(bundleName);
            async.OnGetResult += delegate(Object[] result)
            {
                TryCallback<Object[]>(callback, result);
            };
            return async;
        }
        
        public static AsyncOperationHandle<Object[]> LoadAllAssetsAsync(string bundleName, Type type, LuaFunction callback)
        {
            var async = ResourcesManager.Instance.LoadAllAssetsAsync(bundleName, type);
            async.OnGetResult += delegate(Object[] result)
            {
                TryCallback<Object[]>(callback, result);
            };
            return async;
        }
        
        private static void TryCallback<T>(LuaFunction callback, T result)
        {
            if (callback != null && callback.IsAlive)
            {
                try
                {
                    callback.Call<T>(result);
                }
                catch (Exception e)
                {
                    Logger.Error("Call callback fail error:{0}, result:{1}", e, result);
                }
            }
        }
    }
}