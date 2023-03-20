using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Lua.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OnCopyLuaFilesAttribute : OrderCallBack
    {
        public OnCopyLuaFilesAttribute()
            :base(0)
        {
        }
    }
    
    [BuildTools]
    public static class BuildToLuaTools
    {
        private delegate void CopyFileDelegate(string src, string dst);
        
//        private static string ToolPath => Path.Combine(Application.dataPath, "LuaEncoder");
        private static string ToolPath => Path.Combine(Global.GetPackageFullPath("com.pluginset.tolua"), "Editor", "LuaEncoder~");

        private static readonly string TempPath = Path.Combine(Application.dataPath, "Temp");

        [OnBuildBundles(int.MinValue)]
        [OnBuildPatches(int.MinValue)]
        public static void PrepareScriptPathsOnBuildBundles(BuildProcessorContext context)
        {
            var luaScriptPaths = context.TryGet<List<string>>("toluaScriptPaths", null);
            if (luaScriptPaths != null)
                return;
            
            var buildParams = context.BuildChannels.Get<BuildToLuaParams>();
            if (string.IsNullOrEmpty(buildParams.LuaBundleName))
                return;

            var luaPaths = context.CollectAssetBundleFilePaths(buildParams.LuaBundleName);
            context.Set<List<string>>("toluaScriptPaths", luaPaths);
        }

        [OnBuildBundles]
        [OnBuildPatches]
        public static void OnBuildBundles(BuildProcessorContext context)
        {
            var buildParams = context.BuildChannels.Get<BuildToLuaParams>();
            if (string.IsNullOrEmpty(buildParams.LuaBundleName))
                return;
            
            var luaScriptPaths = context.TryGet<List<string>>("toluaScriptPaths", null);
            if (luaScriptPaths == null || luaScriptPaths.Count <= 0)
                return;

            List<string> buildPaths = BuildGlobal.CollectBuildPaths(context, luaScriptPaths);
            if (buildPaths == null || buildPaths.Count <= 0)
                return;

            if (context.ForEditor)
            {
                BuildLuaFilesCopy(context, buildParams.LuaBundleName, buildPaths.ToArray());
            }
            else if (context.BuildTarget != BuildTarget.WebGL && buildParams.BuildLuaToJit)
            {
                BuildLuaFilesWithJit(context, buildParams.LuaBundleName, buildPaths.ToArray());
            }
            else
            {
                BuildLuaFilesNoJit(context, buildParams.LuaBundleName, buildPaths.ToArray());
            }
            AssetDatabase.Refresh();
        }

        private static void CheckAndRemoveLuaFiles(string[] searchPaths, ref List<string> files)
        {
            files.RemoveAll(delegate(string file)
            {
                var fileName = file.ToLower();
                foreach (var path in searchPaths)
                {
                    if (fileName.Contains($"/assets/{path.ToLower()}/"))
                        return true;
                }

                return false;
            });
        }
        
        public static void CopyLuaFiles(string to, string[] searchPaths)
        {
            if (Directory.Exists(to))
                Directory.Delete(to, true);
            Directory.CreateDirectory(to);
            
            CopyFiles(to, searchPaths, File.Copy, ".txt");
        }
        
        private static void BuildLuaFilesCopy(BuildProcessorContext context, string bundleName, string[] searchPaths)
        {
            EncodeLuaFiles(context, bundleName, Path.Combine(TempPath, "luatemp"), searchPaths, File.Copy);
        }
        
        private static void BuildLuaFilesNoJit(BuildProcessorContext context, string bundleName, string[] searchPaths)
        {
            EncodeLuaFiles(context, bundleName, Path.Combine(TempPath, "luatemp"), searchPaths, LuacEncodeFile);
        }
        
        private static void BuildLuaFilesWithJit(BuildProcessorContext context, string bundleName, string[] searchPaths)
        {
            EncodeLuaFiles(context, bundleName + "32"
                , Path.Combine(TempPath, "luatemp32")
                , searchPaths, LuaJitEncode32File);
            EncodeLuaFiles(context, bundleName + "64"
                , Path.Combine(TempPath, "luatemp64")
                , searchPaths, LuaJitEncode64File);
        }

        private static void EncodeLuaFiles(BuildProcessorContext context, string bundleName, string tempPath, string[] searchPaths, CopyFileDelegate encodeFunc)
        {
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
            Directory.CreateDirectory(tempPath);
            
            CopyFiles(tempPath, searchPaths, (file, newFileName) =>
            {
                encodeFunc(file, newFileName);
                //将编辑好的lua文件添加的bundle目录,等待打包bundle.
                context.AddBuildBundle(bundleName, newFileName.ToLower());
            });
            
            context.TemplatePaths.Add(tempPath);
        }

        private static void CopyFiles(string dstPath, string[] searchPaths, CopyFileDelegate copyFunc, string ext = ".bytes")
        {
            var srcPath = Path.Combine(Application.dataPath, "..");
            foreach (var path in searchPaths)
            {
                string luaPath = Path.Combine(srcPath, path);
                foreach (var file in Directory.GetFiles(luaPath, "*.lua", SearchOption.AllDirectories))
                {
                    var newFileName = file.Replace(luaPath,  dstPath) + ext;
                    if (File.Exists(newFileName))
                        continue;

                    var dir = Path.GetDirectoryName(newFileName);
                    if (dir != null && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    
                    copyFunc(file, newFileName);
                }
            }
        }

        private static void LuaJitEncode32File(string src, string dst)
        {
            LuaJitEncodeFile(src, dst, false);
        }
        
        private static void LuaJitEncode64File(string src, string dst)
        {
            LuaJitEncodeFile(src, dst, true);
        }

        private static void LuaJitEncodeFile(string src, string dst, bool isBit64)
        {
            string luajitPath = string.Empty;
            string luajitFile = string.Empty;
            bool isWin = true;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                luajitPath = Path.Combine(ToolPath, "luajit_win", isBit64 ? "luajit64" : "luajit32");
                luajitFile = Path.Combine(luajitPath, "luajit.exe");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                isWin = false;
                luajitPath = Path.Combine(ToolPath, "luajit_mac", isBit64 ? "luajit64" : "luajit32");
                luajitFile = Path.Combine(luajitPath, "luajit");
            }
            else
            {
                UnityEngine.Debug.LogError("Cannot support this editor platform");
            }

            if (!File.Exists(luajitFile))
            {
                UnityEngine.Debug.LogError("Cannot find luac tool file!");
                File.Copy(src, dst);
                return;
            }

            dst = $"\"{dst.Replace("\\", "/")}\"";
            src = $"\"{src.Replace("\\", "/")}\"";
            Global.ExecuteCommand(luajitFile, luajitPath, isWin, "-b", "-g", src, dst);
        }

        private static void LuacEncodeFile(string src, string dst)
        {
            string luacPath = string.Empty;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // TODO
                luacPath = "luac.exe";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                 luacPath = Path.Combine(ToolPath, "luac_mac", "luac");
            }
            else
            {
                UnityEngine.Debug.LogError("Cannot support this editor platform");
            }

            if (!File.Exists(luacPath))
            {
                UnityEngine.Debug.LogError("Cannot find luac tool file!");
                File.Copy(src, dst);
                return;
            }

            dst = dst.Replace("\\", "/");
            src = src.Replace("\\", "/");
            Global.ExecuteCommand(luacPath, "-o", dst, src);
        }

        [OnCopyLuaFiles]
        public static void CopyLuaFiles(string targetPath)
        {
            var path = Global.GetPackageFullPath("com.pluginset.tolua");
            if (string.IsNullOrEmpty(path))
                return;
            if (!Directory.Exists(path))
                return;

            var luaPath = Path.Combine(path, "Resources", "Lua");
            Global.CopyFiles(luaPath, targetPath, "*.lua");
            Global.CopyFiles(luaPath, targetPath, "*.lua.txt");
        }
    }
}