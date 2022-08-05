using System;
using LuaInterface;
using PluginSet.Core;

namespace PluginSet.Lua
{
    public static class PluginsManagerExtension
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("PluginsManagerLua");

        private static void TryCallbackWithResult(LuaFunction callback, Result result)
        {
            if (callback != null && callback.IsAlive)
            {
                try
                {
                    callback.Call<Result>(result);
                }
                catch (Exception e)
                {
                    Logger.Error("Call login callback fail error:{0}, result:{1}", e, result);
                }
            }
        }
        
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
        
        public static void SetUserInfo(this PluginsManager pm, bool isNewUser, string userId, LuaTable table = null)
        {
            pm.SetUserInfo(isNewUser, userId, table?.ToDictTable<string, object>().ToDictionary());
        }
        
        public static void SetUserInfoWith(this PluginsManager pm, string pluginName, bool isNewUser, string userId, LuaTable table = null)
        {
            pm.SetUserInfoWith(pluginName, isNewUser, userId, table?.ToDictTable<string, object>().ToDictionary());
        }
        
        public static void CustomEvent(this PluginsManager pm, string customEventName, LuaTable table)
        {
            pm.CustomEvent(customEventName, table.ToDictTable<string, object>().ToDictionary());
        }

        public static void CustomEventWith(this PluginsManager pm, string pluginName
            , string customEventName, LuaTable table)
        {
            pm.CustomEventWith(pluginName, customEventName, table.ToDictTable<string, object>().ToDictionary());
        }
        
        public static void Login(this PluginsManager pm, LuaFunction callback)
        {
            pm.Login(delegate(Result result)
            {
                TryCallbackWithResult(callback, result);
            });
        }

        public static void LoginWith(this PluginsManager pm, string pluginName, LuaFunction callback)
        {
            pm.LoginWith(pluginName, delegate(Result result)
            {
                TryCallbackWithResult(callback, result);
            });
        }
        

        public static void Pay(this PluginsManager pm, string productId, LuaFunction callback, string jsonData = null)
        {
            pm.Pay(productId, delegate(Result result)
            {
                TryCallbackWithResult(callback, result);
            }, jsonData);
        }

        public static void PayWith(this PluginsManager pm, string pluginName, string productId, LuaFunction callback,
            string jsonData = null)
        {
            pm.PayWith(pluginName, productId, delegate(Result result)
            {
                TryCallbackWithResult(callback, result);
            }, jsonData);
        }
        
        public static void InitWithProducts(this PluginsManager pm, LuaTable products)
        {
            pm.InitWithProducts(products.ToDictTable<string, int>().ToDictionary());
        }
        
        public static void InitWithProductsWith(this PluginsManager pm, string pluginName, LuaTable products)
        {
            pm.InitWithProductsWith(pluginName, products.ToDictTable<string, int>().ToDictionary());
        }

        public static void ShareImage(this PluginsManager pm, string imagePath
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareImage(imagePath, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }

        public static void ShareImageWith(this PluginsManager pm, string pluginName, string imagePath
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareImageWith(pluginName, imagePath, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareImageUrl(this PluginsManager pm, string imageUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareImageUrl(imageUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareImageUrlWith(this PluginsManager pm, string pluginName, string imageUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareImageUrlWith(pluginName, imageUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareMiniProgram(this PluginsManager pm, string url, string path, string appId
            , LuaFunction success, LuaFunction fail = null,
            string title = null, string desc = null, string image = null)
        {
            pm.ShareMiniProgram(url, path, appId, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }

        public static void ShareMiniProgramWith(this PluginsManager pm, string pluginName, string url, string path, string appId
            , LuaFunction success, LuaFunction fail = null,
            string title = null, string desc = null, string image = null)
        {
            pm.ShareMiniProgramWith(pluginName, url, path, appId, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareMusic(this PluginsManager pm, string musicFile
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareMusic(musicFile, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }

        public static void ShareMusicWith(this PluginsManager pm, string pluginName, string musicFile
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareMusicWith(pluginName, musicFile, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareMusicUrl(this PluginsManager pm, string musicUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareMusicUrl(musicUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }

        public static void ShareMusicUrlWith(this PluginsManager pm, string pluginName, string musicUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareMusicUrlWith(pluginName, musicUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
        
        public static void ShareText(this PluginsManager pm, string text
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string image = null)
        {
            pm.ShareText(text, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, image);
        }

        public static void ShareTextWith(this PluginsManager pm, string pluginName, string text
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string image = null)
        {
            pm.ShareTextWith(pluginName, text, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, image);
        }

        public static void ShareVideo(this PluginsManager pm, string videoFile
            , LuaFunction success, LuaFunction fail = null
            , string extra = null, string title = null,
            string desc = null, string image = null)
        {
            pm.ShareVideo(videoFile, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, extra, title, desc, image);
        }

        public static void ShareVideoWith(this PluginsManager pm, string pluginName, string videoFile
            , LuaFunction success, LuaFunction fail = null,
            string extra = null, string title = null, string desc = null, string image = null)
        {
            pm.ShareVideoWith(pluginName, videoFile, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, extra, title, desc, image);
        }
        
        public static void ShareVideoUrl(this PluginsManager pm, string videoUrl
            , LuaFunction success, LuaFunction fail = null
            , string extra = null, string title = null,
            string desc = null, string image = null)
        {
            pm.ShareVideoUrl(videoUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, extra, title, desc, image);
        }

        public static void ShareVideoUrlWith(this PluginsManager pm, string pluginName, string videoUrl
            , LuaFunction success, LuaFunction fail = null,
            string extra = null, string title = null, string desc = null, string image = null)
        {
            pm.ShareVideoUrlWith(pluginName, videoUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, extra, title, desc, image);
        }

        public static void ShareWebPage(this PluginsManager pm, string webUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null,string image = null)
        {
            pm.ShareWebPage(webUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }

        public static void ShareWebPageWith(this PluginsManager pm, string pluginName, string webUrl
            , LuaFunction success, LuaFunction fail = null
            , string title = null, string desc = null, string image = null)
        {
            pm.ShareWebPageWith(pluginName, webUrl, delegate
            {
                TryCallbackAction(success);
            }, delegate
            {
                TryCallbackAction(fail);
            }, title, desc, image);
        }
    }
}