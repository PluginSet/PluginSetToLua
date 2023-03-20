using PluginSet.Core;
using UnityEngine;

namespace PluginSet.Lua
{
    public class PluginToLuaConfig : ScriptableObject
    {
        public string StartScript = "Main.lua";

        public bool runLuaSource;

        public string[] LuaScriptSearchPaths;

        public string LuaBundleName = null;

        public bool LuaSocket = false;

        public bool LuaProfiler = false;
    }
}
