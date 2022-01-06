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
        private struct DebugSetting
        {
            public bool EnableLuaDebug;
            public string zbsDir;
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

        public static string zbsDir
        {
            get
            {
                return _debugSetting.zbsDir;
            }

            private set
            {
                _debugSetting.zbsDir = value;
                SaveToUserSetting(_debugSetting);
            }
        }

        private static DebugSetting _debugSetting;

        static UserDebugSetting()
        {
            _debugSetting = ReadFromUserSetting();
            CheckSetting();
        }
        
        public static void CheckSetting(bool force = false)
        {
            if (!EnableLuaDebug)
                return;

            if (force || string.IsNullOrEmpty(zbsDir) || !Directory.Exists(zbsDir))
            {
                ChooseDebuggerFolder();
            }
        }

        private static void ChooseDebuggerFolder()
        {
            var title = "选择EmmyDebugger目录";
            var folder = zbsDir;
            if (string.IsNullOrEmpty(folder))
                folder = Path.GetFullPath(".");
            
            string path = EditorUtility.OpenFolderPanel(title, folder, string.Empty);
            if (string.IsNullOrEmpty(path))
                path = zbsDir;
            
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                zbsDir = Path.GetFullPath(path);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "未正确设置EmmyDebugger目录，lua调试不可用", "确定");
            }
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