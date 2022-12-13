using System;
using System.Collections;
using System.IO;
using LuaInterface;
using PluginSet.Core;
using UnityEngine;

namespace PluginSet.Lua
{
    public class LuaFileLoader : LuaFileUtils
    {
        public static LuaFileLoader GetInstance()
        {
            if (LuaFileUtils.instance is LuaFileLoader loader)
                return loader;

            var luaFileLoader = new LuaFileLoader();
            LuaFileUtils.instance = luaFileLoader;
            return luaFileLoader;
        }

        private string _luaBundleName;

        public bool IsLuaBundleExists => !string.IsNullOrEmpty(_luaBundleName);
        
        public IEnumerator CheckLuaBundle(string luaBundleName)
        {
            UnloadLuaBundle();

            Debug.Log(">>>>>>>>>>>>> PluginToLua Environment.Is64BitOperatingSystem = " + Environment.Is64BitOperatingSystem);
            Debug.Log(">>>>>>>>>>>>> PluginToLua Environment.Is64BitProcess = " + Environment.Is64BitProcess);
            Debug.Log(">>>>>>>>>>>>> PluginToLua Environment.IntSize = " + IntPtr.Size);

            if (string.IsNullOrEmpty(luaBundleName))
                yield break;

            luaBundleName = luaBundleName.ToLower();
            var manager = ResourcesManager.Instance;
            if (manager.ExistsBundle(luaBundleName))
            {
                _luaBundleName = luaBundleName;
                yield break;
            }
            
            /**
             * 某些设备启动后，虽然系统为64位，但进程可能是32位，该情况下IntPtr.Size为4，只能运行32位luajit脚本
             */
            if (Environment.Is64BitProcess)
            {
                var name = luaBundleName + "64";
                if (manager.ExistsBundle(name))
                {
                    _luaBundleName = name;
                    yield break;
                }
            }
            else
            {
                var name = luaBundleName + "32";
                if (manager.ExistsBundle(name))
                {
                    _luaBundleName = name;
                    yield break;
                }
            }

            Debug.LogWarning("Cannot load lua bundle with name " + luaBundleName);
        }

        public void UnloadLuaBundle()
        {
            if (!string.IsNullOrEmpty(_luaBundleName))
                ResourcesManager.Instance.ReleaseBundle(_luaBundleName);
            _luaBundleName = null;
        }

        public override byte[] ReadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            
            byte[] buffer = null;
            
#if !UNITY_EDITOR || LUA_BUNDLE_IN_EDITOR
            if (string.IsNullOrEmpty(_luaBundleName))
            {
                buffer = ResourcesManager.Instance.LoadLuaBytes(fileName);
            }
            else
            {
                buffer = ReadBundleAsset(fileName);
            }
#endif

#if UNITY_EDITOR && !LUA_BUNDLE_IN_EDITOR
            if (buffer == null)
                buffer = ReadLuaFile(fileName);
#endif

            if (buffer == null)
            {
                buffer = ReadResourceFile(fileName);
            }

            return buffer;
        }

#if UNITY_EDITOR
        byte[] ReadLuaFile(string fileName)
        {
            var fullName = FindDataFile(fileName);
            if (string.IsNullOrEmpty(fullName))
                return null;

            return File.ReadAllBytes(fullName);
        }

        private string FindDataFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (Path.IsPathRooted(fileName))
            {
                if (!fileName.EndsWith(".lua"))
                {
                    fileName += ".lua";
                }

                return fileName;
            }

            if (fileName.EndsWith(".lua"))
            {
                fileName = fileName.Substring(0, fileName.Length - 4);
            }

            string fullPath = null;

            var manager = ResourcesManager.Instance;
            for (int i = 0; i < searchPaths.Count; i++)
            {
                fullPath = searchPaths[i].Replace("?", fileName);

                if (manager.IsValidAssetFile(fullPath))
                {
                    return fullPath;
                }

                fullPath = searchPaths[i].Replace("?", fileName) + ".txt";
                if (manager.IsValidAssetFile(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }
#endif

        byte[] ReadBundleAsset(string fileName)
        {
            if (!fileName.EndsWith(".lua"))
                fileName += ".lua";

            fileName += ".bytes";

            TextAsset asset = ResourcesManager.Instance.LoadAsset<TextAsset>(_luaBundleName, fileName);
            if (asset == null)
                return null;

            var buffer = asset.bytes;
            Resources.UnloadAsset(asset);
            return buffer;
        }

        byte[] ReadResourceFile(string fileName)
        {
            Debug.Log("ReadResourceFile >>>>>>>>>>>>> " + fileName);
            if (!fileName.EndsWith(".lua"))
            {
                fileName += ".lua";
            }

            byte[] buffer = null;
            string path = "Lua/" + fileName;
            TextAsset text = Resources.Load(path, typeof(TextAsset)) as TextAsset;

            if (text != null)
            {
                buffer = text.bytes;
                Resources.UnloadAsset(text);
            }

            return buffer;
        }
    }
}