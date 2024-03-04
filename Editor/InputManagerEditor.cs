#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEngine.InputSystem;
using VibePack.Input;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace VibePackEditor
{
    [CustomEditor(typeof(InputManager))]
    public class InputManagerEditor : Editor
    {
        SerializedProperty actionAsset;

        private void Awake() => actionAsset = serializedObject.FindProperty("actionAsset");

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            InputManager script = (InputManager)target;

            if (GUILayout.Button("Generate Class & Enums"))
            { 
                if(actionAsset.objectReferenceValue == null)
                {
                    Color color = Color.yellow;
                    string message = "==> Input Action Asset Not Set.";
                    Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), message));
                    return;
                }

                string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(script));
                CreateEventEnum(Path.GetFullPath(Path.Combine(path, "../InputEvent.cs")));
                CreateMapEnum(Path.GetFullPath(Path.Combine(path, "../InputMap.cs")));
                CompilationPipeline.RequestScriptCompilation();
            }
        }

        private void CreateMapEnum(string path)
        {
            string script = "namespace VibePack.Input\n" +
                   "{\n" +
                   "\tpublic enum InputMap\n" +
                   "\t{\n";

            List<string> usedNames = new List<string>();
            foreach (var map in (actionAsset.objectReferenceValue as InputActionAsset).actionMaps)
            {
                string name = map.name;
                if (usedNames.Contains(name))
                    continue;

                usedNames.Add(name);
                script += "\t\t" + name + ",\n";
            }

            script += "\t}\n" +
                "}";

            if (File.Exists(path))
            {
                File.WriteAllText(path, script);
                return;
            }

            File.Create(path);
            CompilationPipeline.compilationFinished += (obj) => File.WriteAllText(path, script);
        }

        private void CreateEventEnum(string path)
        {
            string script = "namespace VibePack.Input\n" +
                    "{\n" +
                    "\tpublic enum InputEvent\n" +
                    "\t{\n";

            List<string> usedNames = new List<string>();
            foreach (var action in (actionAsset.objectReferenceValue as InputActionAsset))
            {
                string name = action.name;
                if (usedNames.Contains(name))
                    continue;

                usedNames.Add(name);
                script += "\t\t" + name + ",\n";
            }

            script += "\t}\n" +
                "}";

            if (File.Exists(path))
            {
                File.WriteAllText(path, script);
                return;
            }

            File.Create(path);
            CompilationPipeline.compilationFinished += (obj) => File.WriteAllText(path, script);
        }
    }
}
#endif