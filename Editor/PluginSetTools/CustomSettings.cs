using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

using BindType = ToLuaMenu.BindType;
using System.Reflection;
using PluginSet.Core;

[InitializeOnLoad]
public static class CustomSettings
{
    public static string saveDir;
    public static string nameSpace;
    public static string fileHead;
    public static string fileTail;

    private static Type currentSetting;
    
    public static string injectionFilesPath => Path.Combine(Global.GetPackageFullPath("com.pluginset.tolua"), "Editor", "Injection");
    public static string toluaBaseType => Path.Combine(Global.GetPackageFullPath("com.pluginset.tolua"), "Runtime", "ToLua", "BaseType");
    
    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList;
    
    // 下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList;
    
    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes;
    
    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList;
    
    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList;
    
    public static BindType[] allCustomTypeList;

    public static List<Type> dynamicList;

    public static List<string> defaultMemeberFilter;

    public static List<Type> dropList;

    private static List<Type> baseDropList;
    
    private static List<Type> tempDropList;
    
    private static Dictionary<Type, string> nameSpaces;
    private static Dictionary<Type, string> wrapNames;
    private static Dictionary<Type, string> libNames;
    private static Dictionary<Type, List<string>> blackLists;
    private static Dictionary<Type, List<Type>> extendTypes;
    private static Dictionary<Type, Type> baseTypes;

    private static List<Type> genereteSettingTypes = new List<Type>();

    public static List<ToLuaGenerateSettingAttribute> GetAllGenerateSettings()
    {
        var result = new List<ToLuaGenerateSettingAttribute>();
        
        foreach (var type in Global.GetAllTypes<ToLuaGenerateSettingAttribute>())
        {
            foreach (ToLuaGenerateSettingAttribute attr in type.GetCustomAttributes(typeof(ToLuaGenerateSettingAttribute), false))
            {
                result.Add(attr);
            }
        }

        return result;
    }

    private static void InitConfigs(ToLuaGenerateSettingAttribute attribute)
    {
        defaultMemeberFilter = new List<string>(ToLuaExport.memberFilter);
        tempDropList = new List<Type>();
        
        nameSpace = attribute.NameSpace;
        if (string.IsNullOrEmpty(nameSpace))
            nameSpace = "LuaWrap";

        fileHead = attribute.FileHead ?? string.Empty;
        fileTail = attribute.FileTail ?? string.Empty;

        var path = attribute.GeneratePath;
        if (string.IsNullOrEmpty(path))
            path = "Generate";
        
        // 不再放在插件目录中
        // var lib = attribute.LibName;
        // if (string.IsNullOrEmpty(lib))
        // {
            saveDir = Path.Combine(Application.dataPath, path) + "/";
        // }
        // else
        // {
        //     saveDir = Path.Combine(Global.GetPackageFullPath(lib), "Runtime", path) + "/";
        // }

        int maxPriority = attribute.Priority;
        genereteSettingTypes.Clear();
        foreach (var type in Global.GetAllTypes<ToLuaGenerateSettingAttribute>())
        {
            bool isValid = false;
            foreach (ToLuaGenerateSettingAttribute attr in type.GetCustomAttributes(typeof(ToLuaGenerateSettingAttribute), false))
            {
                if (attr.Equals(attribute))
                {
                    currentSetting = type;
                    isValid = true;
                    break;
                }

                if (attr.Priority < maxPriority)
                {
                    isValid = true;
                    break;
                }
            }
            
            if (isValid)
                genereteSettingTypes.Add(type);
        }
    }

    public static void Init()
    {
        var settings = GetAllGenerateSettings();
        ToLuaGenerateSettingAttribute target = null;
        int maxPriority = int.MinValue;
        foreach (var setting in settings)
        {
            int priority = setting.Priority;
            if (priority >= maxPriority)
            {
                target = setting;
                maxPriority = priority;
            }
        }

        Init(target);
    }

    public static void Init(ToLuaGenerateSettingAttribute target)
    {
        if (target == null)
            throw new Exception("Cannot generate tolua without target");
        
        InitConfigs(target);
        
        baseDropList = new List<Type>(ToLuaMenu.baseType);
        baseDropList.AddRange(ToLuaMenu.dropType);
        dropList = IterFieldAndProperties<ToLuaGenerateDropAttribute, Type>(currentSetting).ToList();
        dropList.AddRange(IterFieldAndProperty<ToLuaGenerateTempDropAttribute, Type>(currentSetting).ToList());
        dropList.AddRange(baseDropList);
        
        staticClassTypes = IterFieldAndProperties<ToLuaGenerateStaticAttribute, Type>().Distinct()
            .Where(val => !dropList.Contains(val)).ToList();
        
        customDelegateList = IterFieldAndProperties<ToLuaGenerateDelegateAttribute, Type>().Distinct()
            .Where(val => !dropList.Contains(val)).Select(_DT).ToArray();
        
        dynamicList = IterFieldAndProperties<ToLuaGenerateDynamicAttribute, Type>().Distinct()
            .Where(val => !dropList.Contains(val)).ToList();
        
        outList = IterFieldAndProperties<ToLuaGenerateOutListAttribute, Type>().Distinct()
            .Where(val => !dropList.Contains(val)).ToList();
        
        sealedList = IterFieldAndProperties<ToLuaGenerateSealedAttribute, Type>().Distinct()
            .Where(val => !dropList.Contains(val)).ToList();

        nameSpaces = SelectDict<ToLuaGenerateNameSpaceAttribute, Type, string>();
        wrapNames = SelectDict<ToLuaGenerateWrapNameAttribute, Type, string>();
        libNames = SelectDict<ToLuaGenerateLibNameAttribute, Type, string>();
        baseTypes = SelectDict<ToLuaGenerateBaseTypeAttribute, Type, Type>();

        blackLists = MergeDict<ToLuaGenerateBlackListAttribute, Type, string>();
        extendTypes = MergeDict<ToLuaGenerateExtendTypeAttribute, Type, Type>();
        
        customTypeList = IterFieldAndProperties<ToLuaGenerateClassAttribute, Type>()
            .Where(val => !dropList.Contains(val)).Select(_GT).ToArray();
            
        allCustomTypeList = IterFieldAndProperties<ToLuaGenerateClassAttribute, Type>()
            .Where(val => !baseDropList.Contains(val)).Select(_GT).ToArray();
    }

    private static Dictionary<T1, T2> SelectDict<TA, T1, T2>() where TA: Attribute
    {
        var dict = new Dictionary<T1, T2>();
        foreach (var kv in IterFieldAndProperties<TA, KeyValuePair<T1, T2>>())
        {
            if (!dict.ContainsKey(kv.Key))
                dict.Add(kv.Key, kv.Value);
        }

        return dict;
    }
    
    private static Dictionary<T1, List<T2>> MergeDict<TA, T1, T2>() where TA: Attribute
    {
        var dict = new Dictionary<T1, List<T2>>();
        foreach (var kv in IterFieldAndProperties<TA, KeyValuePair<T1, IEnumerable<T2>>>())
        {
            if (dict.TryGetValue(kv.Key, out var list))
            {
                list.AddRange(kv.Value);
            }
            else
            {
                dict.Add(kv.Key, new List<T2>(kv.Value));
            }
        }

        return dict;
    }


    private static IEnumerable<R> IterFieldAndProperty<T, R>(Type type) where T : Attribute
    {
        return (
            from property in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where property.IsDefined(typeof(T), false)
            from iter in (IEnumerable<R>) property.GetValue(null)
            select iter
        ).Concat(
            from field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where field.IsDefined(typeof(T), false)
            from iter in (IEnumerable<R>) field.GetValue(null)
            select iter
        );
    }

    private static IEnumerable<R> IterFieldAndProperties<T, R>(params Type[] exludes) where T : Attribute
    {
        return (
            from type in genereteSettingTypes
            where !exludes.Contains(type)
            from property in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where property.IsDefined(typeof(T), false)
            from iter in (IEnumerable<R>) property.GetValue(null)
            select iter
        ).Concat(
            from type in genereteSettingTypes
            where !exludes.Contains(type)
            from field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            where field.IsDefined(typeof(T), false)
            from iter in (IEnumerable<R>) field.GetValue(null)
            select iter
        );
    }

    public static BindType _GT(Type t)
    {
        var bt = new BindType(t);
        if (nameSpaces.TryGetValue(t, out var ns))
        {
            bt.SetNameSpace(ns);
        }
        
        if (wrapNames.TryGetValue(t, out var wn))
        {
            bt.SetWrapName(wn);
        }

        if (libNames.TryGetValue(t, out var ln))
        {
            bt.SetLibName(ln);
        }
        
        if (baseTypes.TryGetValue(t, out var b))
        {
            bt.SetBaseType(b);
        }

        if (blackLists.TryGetValue(t, out var bl))
        {
            foreach (var name in bl.Distinct())
            {
                bt.AddBlackList(name);
            }
        }

        if (extendTypes.TryGetValue(t, out var et))
        {
            foreach (var type in et)
            {
                bt.AddExtendType(type);
            }
        }
        
        return bt;
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }    
}
