using System;
using System.Collections;
using LuaInterface;
using PluginSet.Core;
using UnityEngine;

namespace PluginSet.Lua
{
    public static class CoroutineHelperWrapper
    {
        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return CoroutineHelper.Instance.StartCoroutine(routine);
        }
        
        public static Coroutine StartCoroutine(IEnumerator routine, LuaFunction callback)
        {
            return CoroutineHelper.Instance.StartCoroutine(RoutineCallback(routine, callback));
        }
        
        public static Coroutine StartCoroutine(IEnumerator routine, Action callback)
        {
            return CoroutineHelper.Instance.StartCoroutine(routine, callback);
        }
        
        public static Coroutine StartCoroutine(params IEnumerator[] routines)
        {
            return CoroutineHelper.Instance.StartCoroutine(ContactRoutine(routines));
        }
        

        public static IEnumerator RoutineCallback(IEnumerator routine, LuaFunction callback)
        {
            yield return routine;
            if (callback == null || !callback.IsAlive)
                yield break;

            try
            {
                callback.Call();
            }
            catch (Exception e)
            {
                Debug.LogError("Error in CoroutineHelperWrapper: " + e);
            }
        }

        public static IEnumerator ContactRoutine(params IEnumerator[] routines)
        {
            return CoroutineHelper.ContactRoutine(routines);
        }
    }
}