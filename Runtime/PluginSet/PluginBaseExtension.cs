using System;
using LuaInterface;
using PluginSet.Core;

namespace PluginSet.Lua
{
    public static class PluginBaseExtension
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("PluginBaseLua");
        
        private static void TryCallbackAction(LuaFunction callback)
        {
            if (callback != null && callback.IsAlive)
            {
                try
                {
                    callback.Call();
                }
                catch (Exception e)
                {
                    Logger.Error("Call callback fail error:{0}", e);
                }
            }
        }
        
        public static bool SendNotification(this PluginBase plugin, string method, object data
            , LuaFunction confirm, LuaFunction cancel = null)
        {
            return plugin.SendNotification(method, data, delegate
            {
                TryCallbackAction(confirm);
            }, delegate
            {
                TryCallbackAction(cancel);
            });
        }

        public static bool SendNotificationDict(this PluginBase plugin, string method, LuaTable data
            , LuaFunction confirm = null, LuaFunction cancel = null)
        {
            return SendNotification(plugin, method, data.ToDictTable<string, object>().ToDictionary(), confirm, cancel);
        }
        
        public static bool NotifyAnyOne(this PluginBase plugin, string method, object data
            , LuaFunction confirm,  LuaFunction cancel = null)
        {
            return plugin.NotifyAnyOne(method, data, delegate
            {
                TryCallbackAction(confirm);
            }, delegate
            {
                TryCallbackAction(cancel);
            });
        }
        

        public static bool NotifyAnyOneDict(this PluginBase plugin, string method, LuaTable data
            , LuaFunction confirm = null, LuaFunction cancel = null)
        {
            return NotifyAnyOne(plugin, method, data.ToDictTable<string, object>().ToDictionary(), confirm, cancel);
        }
    }
}