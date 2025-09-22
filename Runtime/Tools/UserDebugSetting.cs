

using System;
#if UNITY_EDITOR
using System.IO;
using PluginSet.Core;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Lua
{
    [InitializeOnLoad]
    public static class UserDebugSetting
    {
        private const string ToLuaDebugSettingKey = "tolua_debug_setting";
        
        [Serializable]
        private struct DebugSetting
        {
            public bool EnableLuaDebug;
            public bool ReadBundleInEditor;
        }

        public static bool EnableLuaDebug
        {
            get
            {
                return _debugSetting.EnableLuaDebug;
            }
            
            set
            {
                _debugSetting.EnableLuaDebug = value;
                SaveToUserSetting(_debugSetting);
            }
            
        }

        public static bool ReadBundleInEditor
        {
            get
            {
                return _debugSetting.ReadBundleInEditor;
            }

            set
            {
                _debugSetting.ReadBundleInEditor = value;
                SaveToUserSetting(_debugSetting);
            }
        }

        private static DebugSetting _debugSetting;

        static UserDebugSetting()
        {
            _debugSetting = ReadFromUserSetting();
        }

        private static DebugSetting ReadFromUserSetting()
        {
            var debugSettingJson = EditorUserSettings.GetConfigValue(ToLuaDebugSettingKey);
            if (string.IsNullOrEmpty(debugSettingJson))
                return new DebugSetting();

            return JsonUtility.FromJson<DebugSetting>(debugSettingJson);
        }

        private static void SaveToUserSetting(DebugSetting setting)
        {
            var jsonStr = JsonUtility.ToJson(setting);
            EditorUserSettings.SetConfigValue(ToLuaDebugSettingKey, jsonStr);
        }
    }
}
#endif