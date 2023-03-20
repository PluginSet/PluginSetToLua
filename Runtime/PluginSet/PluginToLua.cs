using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuaInterface;
using PluginSet.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PluginSet.Lua
{
    [PluginRegister]
    public class PluginToLua : PluginBase, IStartPlugin
    {
        private class DelegateReference
        {
            public Action<PluginsEventContext> action;
            public List<string> references;

            public void Dispose()
            {
            }
        }

        public override string Name => "ToLua";

        public int StartOrder => PluginsStartOrder.VirtualMachine;

        public bool IsRunning { get; private set; }

        private LuaState _luaState = null;
        private LuaLooper _loop = null;
        private LuaFunction _levelLoaded = null;
        private LuaFunction _onAppPause = null;

        private bool _luaSocketOpend = false;

        private PluginToLuaConfig _toLuaConfig = null;

        private Dictionary<LuaFunction, DelegateReference> _luaDelegates
            = new Dictionary<LuaFunction, DelegateReference>();

        private void OpenLibs()
        {
            _luaSocketOpend = false;
            _luaState.OpenLibs(LuaDLL.luaopen_pb);
            _luaState.OpenLibs(LuaDLL.luaopen_struct);
            _luaState.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            _luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif

#if UNITY_EDITOR && LUA_DEBUGGER
            OpenZbsDebugger(UserDebugSetting.zbsDir);
#endif

            if (_toLuaConfig.LuaSocket)
            {
                OpenLuaSocket();
            }

            OpenCJson();
        }

#if UNITY_EDITOR && LUA_DEBUGGER
    private void OpenZbsDebugger(string zbsDir, string ip = "localhost", int port = 9966)
        {
            if (string.IsNullOrEmpty(zbsDir))
                return;

            if (!Directory.Exists(zbsDir))
            {
                Debugger.LogWarning($"Not Set zbsDir or zbsDir{zbsDir} is invalid!");
                return;
            }

            OpenLuaSocket();

            if (!string.IsNullOrEmpty(zbsDir))
            {
                _luaState.AddSearchPath(zbsDir);
            }
            
            _luaState.LuaDoString($"DebugServerIp = {ip};"
                                  + $"DebugServerPort='{port}';"
                                  + $"ZbsDir='{Global.GetFullPath(zbsDir)}';"
#if UNITY_EDITOR_64
                                  + "Is64=true;"
#endif
#if UNITY_EDITOR_WIN
                                  + "IsWin=true;"
#endif
                , "@PluginToLua.cs");
        }
#endif

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        private static int LuaOpen_Socket_Core(IntPtr L)
        {
            return LuaDLL.luaopen_socket_core(L);
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        private static int LuaOpen_Mime_Core(IntPtr L)
        {
            return LuaDLL.luaopen_mime_core(L);
        }


        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        private void OpenCJson()
        {
            _luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            _luaState.OpenLibs(LuaDLL.luaopen_cjson);
            _luaState.LuaSetField(-2, "cjson");

            _luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
            _luaState.LuaSetField(-2, "cjson.safe");
        }

        private void OpenLuaSocket()
        {
            if (_luaSocketOpend) return;
            _luaSocketOpend = true;

            _luaState.BeginPreLoad();
            _luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
            _luaState.RegFunction("mime.core", LuaOpen_Mime_Core);
            _luaState.EndPreLoad();
        }

        private void CallMain()
        {
            var main = _luaState.GetFunction("Main");
            if (main == null)
                return;

            main.Call();
            main.Dispose();
        }

        /// <summary>
        /// 通知lua游戏退出
        /// </summary>
        private void CallQuit(bool isAppQuit)
        {
            var quit = _luaState.GetFunction("OnApplicationQuit");
            if (quit == null)
                return;

            quit.Call<bool>(isAppQuit);
            quit.Dispose();
        }

        /// <summary>
        /// 通知lua游戏重启
        /// </summary>
        private void CallReadyRestart()
        {
            var reset = _luaState.GetFunction("OnApplicationReadyRestart");
            if (reset == null)
                return;

            reset.Call();
            reset.Dispose();
        }

        private void StartMain()
        {
            _luaState.DoFile(_toLuaConfig.StartScript);
            _levelLoaded = _luaState.GetFunction("OnLevelWasLoaded");
            _onAppPause = _luaState.GetFunction("OnApplicationPause");
            CallMain();
        }

        private void StartLooper()
        {
            _loop = gameObject.AddComponent<LuaLooper>();
            _loop.luaState = _luaState;
        }

        private void Bind()
        {
            LuaBinderUtils.Bind(_luaState);
            DelegateFactory.Init();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_levelLoaded != null)
            {
                _levelLoaded.Call<string, int>(scene.name, scene.buildIndex);
            }

            if (_luaState != null)
            {
                _luaState.RefreshDelegateMap();
            }
        }

        protected override void Init(PluginSetConfig config)
        {
            var cfg = config.Get<PluginToLuaConfig>();
            _toLuaConfig = cfg;

            Debugger.useLog = true;
        }

        public IEnumerator StartPlugin()
        {
#if UNITY_EDITOR && !LUA_BUNDLE_IN_EDITOR
            var runSourcePath = Application.dataPath;
#else
            var runSourcePath = _toLuaConfig.runLuaSource ? Application.persistentDataPath : null;
#endif

            var fileLoader = LuaFileLoader.GetInstance();
            var runSource = !string.IsNullOrEmpty(runSourcePath);
            if (!runSource)
            {
                yield return fileLoader.CheckLuaBundle(_toLuaConfig.LuaBundleName);
#if UNITY_EDITOR
                if (!fileLoader.IsLuaBundleExists)
                {
                    runSourcePath = Application.dataPath;
                    runSource = true;
                }
#endif
            }

            _luaState = new LuaState();
            OpenLibs();
            _luaState.LuaSetTop(0);
            Bind();

            if (runSource)
            {
                var luaScriptSearchPaths = _toLuaConfig.LuaScriptSearchPaths;
                if (luaScriptSearchPaths != null && luaScriptSearchPaths.Length > 0)
                {
                    foreach (var path in luaScriptSearchPaths)
                    {
                        _luaState.AddSearchPath(Path.GetFullPath(Path.Combine(runSourcePath, path)).Replace("\\", "/"));
                    }
                }
            }

            _luaState.Start();
#if UNITY_EDITOR && LUA_DEBUGGER
            _luaState.DoFile("debug.lua");
            _luaState.InitPackagePath();
#endif
            StartLooper();

            SetGlobals();

            base.SendNotification(PluginToLuaConstants.NOTIFY_ON_LUA_START, _luaState);

            IsRunning = true;
            StartMain();
        }

        private void SetGlobals()
        {
            var oldTop = _luaState.LuaGetTop();

            _luaState.Push(this);
            _luaState.LuaSetGlobal("_GPlugin");
            _luaState.LuaSetTop(oldTop);

#if DEBUG
            _luaState.Push(true);
#else
            _luaState.Push(false);
#endif
            _luaState.LuaSetGlobal("DEBUG");
            _luaState.LuaSetTop(oldTop);
        }

        public void DisposePlugin(bool isAppQuit = false)
        {
            IsRunning = false;
            if (_luaState == null)
                return;

            var fileLoader = LuaFileLoader.GetInstance();
            fileLoader.UnloadLuaBundle();

            foreach (var kv in _luaDelegates)
            {
                var refer = kv.Value;
                var action = refer.action;
                foreach (var eventName in refer.references)
                {
                    base.RemoveEventListener(eventName, action);
                }

                refer.Dispose();
                kv.Key.Dispose();
            }

            _luaDelegates.Clear();

            if (!isAppQuit)
                CallReadyRestart();
            
            CallQuit(isAppQuit);
            
            base.SendNotification(PluginToLuaConstants.NOTIFY_ON_LUA_STOP, _luaState);
            LuaState state = _luaState;
            _luaState = null;

            if (_levelLoaded != null)
            {
                _levelLoaded.Dispose();
                _levelLoaded = null;
            }

            if (_onAppPause != null)
            {
                _onAppPause.Dispose();
                _onAppPause = null;
            }

            if (_loop != null)
            {
                UnityEngine.Object.Destroy(_loop);
                _loop = null;
            }

            state.Dispose();
        }

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _toLuaConfig = null;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_onAppPause != null)
            {
                _onAppPause.Call<bool>(pauseStatus);
            }
        }

        public void AddEventListener(string eventName, LuaFunction callback)
        {
            if (callback == null)
                return;

            if (_luaDelegates.TryGetValue(callback, out var reference))
            {
                base.AddEventListener(eventName, reference.action);
                reference.references.Add(eventName);
            }
            else
            {
                var action = (Action<PluginsEventContext>) DelegateTraits<Action<PluginsEventContext>>.Create(callback);
                base.AddEventListener(eventName, action);
                _luaDelegates.Add(callback, new DelegateReference()
                {
                    action = action,
                    references = new List<string>(new[] {eventName})
                });
            }
        }

        public void RemoveEventListener(string eventName, LuaFunction callback)
        {
            if (callback == null)
                return;

            if (_luaDelegates.TryGetValue(callback, out var reference))
            {
                base.RemoveEventListener(eventName, reference.action);
                reference.references.Remove(eventName);
                if (reference.references.Count <= 0)
                {
                    reference.Dispose();
                    _luaDelegates.Remove(callback);
                }
            }
        }

        public IntPtr GetLuaStateIntPtr()
        {
            return _luaState.GetPtr();
        }
    }
}
