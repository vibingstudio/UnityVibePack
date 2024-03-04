/*using TMPro.EditorUtilities;
using VibePack.UI;
using UnityEditor;

namespace VibePackEditor
{
    [CustomEditor(typeof(TMP_Animated))]
    public class TMP_AnimatedEditor : TMP_EditorPanelUI
    {
        SerializedProperty gradient;
        SerializedProperty textSpeed;

        private void Awake()
        {
            gradient = serializedObject.FindProperty("gradient");
            textSpeed = serializedObject.FindProperty("textSpeed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(gradient);
            EditorGUILayout.PropertyField(textSpeed);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
*/