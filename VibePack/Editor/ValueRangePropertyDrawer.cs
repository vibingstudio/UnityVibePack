using VibePack.Utility;
using UnityEditor;
using UnityEngine;

namespace VibePackEditor
{
    [CustomPropertyDrawer(typeof(ValueRange<>))]
    public class ValueRangePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("min");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minProperty = property.FindPropertyRelative("min");
            var maxProperty = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(position, label, property);

            position.width *= 2f / 3f;
            EditorGUI.PropertyField(position, minProperty, label, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width;
            position.height = EditorGUI.GetPropertyHeight(maxProperty);
            position.x += 4;
            position.width /= 3;
            EditorGUI.PropertyField(position, maxProperty, GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}