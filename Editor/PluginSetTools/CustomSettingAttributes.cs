using System;
using System.IO;
using UnityEngine;

/**
 * 标注导出设置类，设定导出目录
 */
[AttributeUsage(AttributeTargets.Class)]
public class ToLuaGenerateSettingAttribute : Attribute
{
    public string NameSpace;
    public string GeneratePath;
    public string LibName;
    public int Priority;
    public string FileHead;
    public string FileTail;
    
    public ToLuaGenerateSettingAttribute(string nameSpace, string path, string libName = ""
     , int priority = 0, string fileHead = null, string fileTail = null)
    {
        GeneratePath = path;
        LibName = libName;
        NameSpace = nameSpace;
        Priority = priority;
        FileHead = fileHead;
        FileTail = fileTail;
    }
}
    
/**
 * 导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
 * unity 有些类作为sealed class, 其实完全等价于静态类
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateStaticAttribute : Attribute
{
}

/**
 * 附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateDelegateAttribute : Attribute
{
}

/**
 * 在这里添加你要导出注册到lua的类型列表
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateClassAttribute : Attribute
{
}

/**
 * 动态类？
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateDynamicAttribute : Attribute
{
}

/**
 * 重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
 * 使用方法参见tolua例子14
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateOutListAttribute : Attribute
{
}

/**
 * 下面的类没有派生类，可以作为sealed class
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateSealedAttribute : Attribute
{
}

/**
 * 指定所有类的命名空间
 * 返回 IEnumerable<KeyValuePair<Type, string>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateNameSpaceAttribute : Attribute
{
}

/**
 * 指定所有类的WrapName
 * 返回 IEnumerable<KeyValuePair<Type, string>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateWrapNameAttribute : Attribute
{
}

/**
 * 指定所有类的LibName
 * 返回 IEnumerable<KeyValuePair<Type, string>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateLibNameAttribute : Attribute
{
}

/**
 * 指定所有类的基类
 * 返回 IEnumerable<KeyValuePair<Type, Type>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateBaseTypeAttribute : Attribute
{
}

/**
 * 列举所有类的黑名单
 * 返回 IEnumerable<KeyValuePair<Type, List<string>>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateBlackListAttribute : Attribute
{
}

/**
 * 列举所有类的ExtendType
 * 返回 IEnumerable<KeyValuePair<Type, List<Type>>>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateExtendTypeAttribute : Attribute
{
}

/**
 * 列举所有不导出的类
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateDropAttribute : Attribute
{
}


/**
 * 列举临时（导出当前设置时）的所有不导出的类
 * 返回 IEnumerable<Type>
 */
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ToLuaGenerateTempDropAttribute : Attribute
{
}
