using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using LuaInterface;
using Debug = UnityEngine.Debug;

[AttributeUsage(AttributeTargets.Class)]
public class LuaBinderAttribute: Attribute
{
    
}

public interface ILuaBinder
{
    void Bind(LuaState L);
}

public static class LuaBinderUtils
{
    private static IEnumerable<Type> luaBinderTypes =>
        from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsDefined(typeof(LuaBinderAttribute), false)
            select type;
        

    public static void Bind(LuaState L)
    {
        var timer = new Stopwatch();
        timer.Start();
        
        var assemblis = 
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
#if !NET_STANDARD_2_0 && UNITY_EDITOR
            where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
#endif
            select assembly;

        foreach (var assembly in assemblis)
        {
            Type[] types;
                
            try
            {
                types = assembly.GetTypes();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                continue;
            }
                
            foreach (var type in types)
            {
                if (type.IsDefined(typeof(LuaBinderAttribute), false))
                {
                    var binder = Activator.CreateInstance(type) as ILuaBinder;
                    binder?.Bind(L);
                }
            }
        }
        
        timer.Stop();
        Debug.Log($"Lua bind all cost time: {timer.ElapsedMilliseconds}ms");
    }
}
