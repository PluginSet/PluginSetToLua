using System.IO;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Lua.Editor
{
    public static class MenuToLua
    {
#if !LUA_BUNDLE_IN_EDITOR
        [MenuItem("PluginSet/Lua/Enable Read Bundle In Editor", false, 1000)]
        public static void EnableLuaBundleInEditor()
        {
            Global.AddPlatformSymbol(BuildTargetGroup.iOS, "LUA_BUNDLE_IN_EDITOR");
            Global.AddPlatformSymbol(BuildTargetGroup.Android, "LUA_BUNDLE_IN_EDITOR");
            Global.AddPlatformSymbol(BuildTargetGroup.Standalone, "LUA_BUNDLE_IN_EDITOR");
        }
#else
        [MenuItem("PluginSet/Lua/Disable Read Bundle In Editor", false, 1000)]
        public static void EnableLuaBundleInEditor()
        {
            Global.RemovePlatformSymbol(BuildTargetGroup.iOS, "LUA_BUNDLE_IN_EDITOR");
            Global.RemovePlatformSymbol(BuildTargetGroup.Android, "LUA_BUNDLE_IN_EDITOR");
            Global.RemovePlatformSymbol(BuildTargetGroup.Standalone, "LUA_BUNDLE_IN_EDITOR");
        }
#endif
        
#if !LUA_DEBUGGER
        [MenuItem("PluginSet/Lua/Enable Debugger", false, 1000)]
        public static void EnableLuaDebugger()
        {
            UserDebugSetting.EnableLuaDebug = true;
            Global.AddPlatformSymbol(BuildTargetGroup.iOS, "LUA_DEBUGGER");
            Global.AddPlatformSymbol(BuildTargetGroup.Android, "LUA_DEBUGGER");
            Global.AddPlatformSymbol(BuildTargetGroup.Standalone, "LUA_DEBUGGER");
        }
#else
        [MenuItem("PluginSet/Lua/Disable Debugger", false, 1000)]
        public static void DisableLuaDebugger()
        {
            UserDebugSetting.EnableLuaDebug = false;
            Global.RemovePlatformSymbol(BuildTargetGroup.iOS, "LUA_DEBUGGER");
            Global.RemovePlatformSymbol(BuildTargetGroup.Android, "LUA_DEBUGGER");
            Global.RemovePlatformSymbol(BuildTargetGroup.Standalone, "LUA_DEBUGGER");
        }
        
        [MenuItem("PluginSet/Lua/Set Lua Debugger", false, 1000)]
        public static void SetLuaDebugger()
        {
            UserDebugSetting.CheckSetting(true);
        }
#endif

        [MenuItem("PluginSet/Lua/Generator", false, 2000)]
        public static void ShowGenerator()
        {
            ToLuaGeneratorWindow.ShowWindow();
        }

        [MenuItem("PluginSet/Lua/Gen All", false, 2000)]
        public static void GenerateAll()
        {
            var settings = CustomSettings.GetAllGenerateSettings();
            foreach (var target in settings)
            {
                CustomSettings.Init(target);
                ToLuaMenu.GenLuaAll();
            }
        }

        [MenuItem("PluginSet/Lua/Clear wrap files", false, 2000)]
        public static void ClearWrapFiles()
        {
            CustomSettings.Init();
            ToLuaMenu.ClearLuaWraps();
        }

        [MenuItem("PluginSet/Lua/AttachProfiler", false, 2000)]
        public static void AttachProfiler()
        {
            if (!Application.isPlaying)
                return;

            EditorProfiler.ExecuteAttach();
        }

        [MenuItem("PluginSet/Lua/DetachProfiler", false, 2000)]
        public static void DetachProfiler()
        {
            if (!Application.isPlaying)
                return;

            EditorProfiler.ExecuteDetach();
        }

//        [MenuItem("PluginSet/Lua/CopyLuaFiles", false, 2000)]
        public static void CopyLuaFiles()
        {
            BuildToLuaTools.CopyLuaFiles(Path.Combine(Application.dataPath, "Resources/Lua"), new[]
            {
                "PluginSetToLua/Resources/Lua"
            });
        }

        [MenuItem("PluginSet/Lua/Gen API for EmmyLua", false, 2000)]
        public static void ExportEmmyLua()
        {
            var exportPath = EditorUtility.SaveFolderPanel("选择导出目录", Path.Combine(Application.dataPath, ".."), "LuaAPI");
            if (string.IsNullOrEmpty(exportPath))
                return;

            EditorApplication.delayCall += delegate
            {
                CustomSettings.Init();
                ToLuaEmmyAPIGenerator.ExportEmmyLua(exportPath);

                var config = EditorSetting.CurrentBuildChannel;
                var toLua = config.Get<BuildToLuaParams>();
                if (toLua.CopyLuaFilesToLuaApi)
                    Global.CallCustomOrderMethods<OnCopyLuaFilesAttribute>(exportPath);

                AssetDatabase.Refresh();
            };
        }

        [MenuItem("PluginSet/Lua/Gen EmmyLua To Default Folder", false, 2000)]
        public static void ExportEmmyLuaToDefault()
        {
            var exportPath = Path.Combine(Application.dataPath, "LuaAPI");
            CustomSettings.Init();
            ToLuaEmmyAPIGenerator.ExportEmmyLua(exportPath);

            var config = EditorSetting.CurrentBuildChannel;
            var toLua = config.Get<BuildToLuaParams>();
            if (toLua.CopyLuaFilesToLuaApi)
                Global.CallCustomOrderMethods<OnCopyLuaFilesAttribute>(exportPath);

            AssetDatabase.Refresh();
        }
    }
}
