using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Lua.Editor
{
    public class ToLuaGeneratorWindow: UnityEditor.EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<ToLuaGeneratorWindow>();
            window.titleContent = new GUIContent("ToLua Generator", "Generate or clear ToLua wrapper files");
            window.Show();
        }

        
        private List<ToLuaGenerateSettingAttribute> generateSettingAttributes;

        private string[] settingTexts;
        private int selectedIndex = 0;
        
        private void OnEnable()
        {
            // 使用当前类初始化  
            generateSettingAttributes = CustomSettings.GetAllGenerateSettings();
            generateSettingAttributes.Sort((a, b) => a.Priority - b.Priority);
            selectedIndex = generateSettingAttributes.Count - 1;
            
            UpdateSettingTexts();
        }

        private void UpdateSettingTexts()
        {
            List<string> settingTextList = new List<string>();
            for (int i = 0; i < generateSettingAttributes.Count; i++)
            {
                var attr = generateSettingAttributes[i];
                
                string lib = attr.LibName;
                if (string.IsNullOrEmpty(lib))
                    lib = "Assembly-CSharp";

                string nameSpace = attr.NameSpace;
                if (string.IsNullOrEmpty(nameSpace))
                    nameSpace = "NULL";
                
                string path = attr.GeneratePath;
                if (string.IsNullOrEmpty(path))
                    path = "LuaWrap";

                var text = $"{lib}::{nameSpace} {path} -> {attr.Priority}";
//                var content = new GUIContent(text, text);
                settingTextList.Add(text);
            }

            settingTexts = settingTextList.ToArray();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            
            EditorGUI.BeginChangeCheck();

            int selected = GUILayout.SelectionGrid(selectedIndex, settingTexts, 1);
            if (EditorGUI.EndChangeCheck())
            {
                selectedIndex = selected;
                UpdateSettingTexts();
            }

            if (GUILayout.Button("Generate all"))
            {
                if (selectedIndex < 0)
                {
                    Debug.LogWarning("Please choose a generate target first");
                }
                else
                {
                    Debug.Log("Generate :: " + settingTexts[selectedIndex]);
                    CustomSettings.Init(generateSettingAttributes[selectedIndex]);
                    ToLuaMenu.GenLuaAll();
                }
            }

            if (GUILayout.Button("Clear Wrappers"))
            {
                if (selectedIndex < 0)
                {
                    Debug.LogWarning("Please choose a generate target first");
                }
                else
                {
                    Debug.Log("Clear wrappers :: " + settingTexts[selectedIndex]);
                    CustomSettings.Init(generateSettingAttributes[selectedIndex]);
                    ToLuaMenu.ClearLuaWraps();
                }
            }
            
            GUILayout.EndVertical();
        }
    }
}