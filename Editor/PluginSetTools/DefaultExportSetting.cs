using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;
using Object = UnityEngine.Object;

namespace PluginSet.Lua.Editor
{
    [ToLuaGenerateSetting("PluginSet.Lua.LuaWrap", "BaseWraps", "com.pluginset.tolua", int.MinValue)]
    public class DefaultExportSetting
    {
        //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
        //unity 有些类作为sealed class, 其实完全等价于静态类
        [ToLuaGenerateStatic]
        public static List<Type> staticClassTypes = new List<Type>
        {
            // plugin set core
            typeof(ResourcesLuaWrapper),
            typeof(CoroutineHelperWrapper),
            typeof(LoggerManager),
            typeof(PluginUtil),
            typeof(PluginConstants),
            typeof(PluginToLuaConstants),
        };


        //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
        [ToLuaGenerateDrop]
        [ToLuaGenerateDelegate]
        public static List<Type> customDelegateList = new List<Type>
        {
            typeof(Action),
            typeof(Action<AsyncOperationHandle>),
            typeof(Action<PluginsEventContext>),
            typeof(Func<IEnumerator>),
            typeof(Action<Result>),
            typeof(Action<bool>),
            typeof(Action<string>),
            typeof(Action<int>),
            typeof(Func<bool>),
            typeof(Action<Object>),
            typeof(Action<Object[]>),
        };


        //在这里添加你要导出注册到lua的类型列表
        [ToLuaGenerateDrop]
        [ToLuaGenerateClass]
        public static List<Type> customTypeList = new List<Type>
        {
            // unity base
            typeof(ScriptableObject),
            typeof(MonoBehaviour),
            typeof(Behaviour),
            typeof(Component),
            
            // lua
            typeof(LuaInjectionStation),
            typeof(InjectType),
            typeof(Debugger),
            typeof(LuaProfiler),
            typeof(LanguageType),
            
            typeof(PluginConstants),
            typeof(PluginToLuaConstants),

            // plugin set core
            typeof(AsyncOperationHandle),
            typeof(AsyncOperationHandle<Object>),
            typeof(AsyncOperationHandle<Object[]>),
            typeof(MultiAsyncOperations),
            typeof(SimpleDataSet),

            typeof(PluginsEventContext),
            typeof(PluginBase),
            typeof(PluginsManager),
            typeof(PluginUtil),

            typeof(CoroutineHelperWrapper),
            typeof(ResourcesLuaWrapper),
            typeof(PluginToLua),
            typeof(ClientProfiler),

            typeof(Devices),
            typeof(DevicePermission),
            typeof(Logger),
            typeof(LoggerManager),

            typeof(Result),
            typeof(ReportError),
            typeof(IPaymentProduct),
        };

        [ToLuaGenerateDynamic]
        public static List<Type> dynamicList = new List<Type>();

        [ToLuaGenerateBaseType]
        public static Dictionary<Type, Type> baseTypes = new Dictionary<Type, Type>
        {
            {typeof(AsyncOperationHandle), typeof(System.Object)},
            {typeof(PluginBase), typeof(System.Object)},
        };


        [ToLuaGenerateNameSpace]
        public static Dictionary<Type, string> nameSpaces = new Dictionary<Type, string>
        {
            {typeof(LuaInjectionStation), null},
            {typeof(InjectType), null},
            {typeof(LuaProfiler), null},
            {typeof(Debugger), null},

            {typeof(ResourcesLuaWrapper), null},
            {typeof(CoroutineHelperWrapper), null},
            {typeof(LoggerManager), null},
        };

        [ToLuaGenerateLibName]
        public static Dictionary<Type, string> libsName = new Dictionary<Type, string>
        {
            {typeof(ResourcesLuaWrapper), "ResourcesManager"},
            {typeof(CoroutineHelperWrapper), "CoroutineHelper"},
        };

        [ToLuaGenerateBlackList]
        public static Dictionary<Type, IEnumerable<string>> blackList = new Dictionary<Type, IEnumerable<string>>
        {
            {typeof(PluginToLua), new[] {"StartOrder", "StartPlugin", "DisposePlugin"}},
        };

        [ToLuaGenerateDrop]
        public static List<Type> dropList = new List<Type>
        {
            typeof(ResourcesManager),
            typeof(CoroutineHelper),
        };

        [ToLuaGenerateTempDrop]
        public static List<Type> tempDropList = new List<Type>
        {
            typeof(MonoBehaviour),
            typeof(Behaviour),
            typeof(Component),
        };

        [ToLuaGenerateExtendType]
        public static Dictionary<Type, IEnumerable<Type>> extendTypes = new Dictionary<Type, IEnumerable<Type>>
        {
            {typeof(PluginBase), new[] {typeof(PluginBaseExtension)}},
            {typeof(PluginsManager), new[] {typeof(PluginsManagerExtension)}},
        };
    }
}
