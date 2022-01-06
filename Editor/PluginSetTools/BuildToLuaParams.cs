using System.Linq;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Lua.Editor
{
    [BuildTools]
    [BuildChannelsParams("ToLua", -100, "ToLua环境相当配置")]
    public class BuildToLuaParams : ScriptableObject
    {
        [Tooltip("lua脚本搜索目录")]
        [FolderDrag(null, ".")]
        public string[] LuaScriptPaths;

        [Tooltip("打包时是否将lua打包为jit")]
        public bool BuildLuaToJit;

        [Tooltip("直接运行lua源文件")]
        public bool runLuaSource;

        [Tooltip("lua启动脚本")]
        public string StartScript = "Main.lua";

        [Tooltip("lua所在的bundle名称（所有脚本在同一个assetBundle中）")]
        public string LuaBundleName = null;

        [Tooltip("是否启用luaSocket")]
        public bool LuaSocket = false;

        [Tooltip("如果选中，则在导出EmmyLuaAPI时会拷贝所有基础Lua脚本到LuaAPI目录")]
        public bool CopyLuaFilesToLuaApi = false;

        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            context.AddLinkAssembly("PluginSet.Tolua");
            context.AddLinkAssembly("Assembly-CSharp");
            if (UserDebugSetting.EnableLuaDebug && !context.ProductMode)
                context.Symbols.Add("LUA_DEBUGGER");
            
            var buildParams = context.BuildChannels.Get<BuildToLuaParams>("ToLua");
            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.Get<PluginToLuaConfig>("ToLua");
            config.runLuaSource = buildParams.runLuaSource;
            config.StartScript = buildParams.StartScript;
            config.LuaBundleName = buildParams.LuaBundleName;
            config.LuaSocket = buildParams.LuaSocket;
            config.LuaScriptSearchPaths = buildParams.LuaScriptPaths.Select(path => path.Substring(7)).ToArray();
        }

        [iOSXCodeProjectModify(int.MaxValue)]
        public static void OnIOSXCodeProjectModify(BuildProcessorContext context, PBXProjectManager projectManager)
        {
            // 目前TOLUA静态库不支持bitcode
            projectManager.Project.SetBuildProperty(projectManager.UnityFramework, "ENABLE_BITCODE", "NO");
        }
    }
}
