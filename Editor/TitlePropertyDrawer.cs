#if UNITY_EDITOR
using VibePack.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VibePackEditor
{
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleAttributeDrawer : DecoratorDrawer
    {
        private static GUIStyle titleStyle;

        private static Color backgroundColor;
        private static Color rectColor;

#if UNITY_2022_2_OR_NEWER
        public override VisualElement CreatePropertyGUI()
        {
            VisualElement container = new VisualElement
            {
                style =
                {
                    backgroundColor = new StyleColor(backgroundColor),
                    display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex),
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    marginBottom = new StyleLength(4),
                }
            };

            titleStyle = new GUIStyle(EditorStyles.boldLabel);
            rectColor = titleStyle.normal.textColor;
            rectColor.a = 0.5f;
            VisualElement container2 = new VisualElement
            {
                style =
                {
                    width = new StyleLength(2),
                    // height = new StyleLength(10),
                    backgroundColor = new StyleColor(rectColor),
                    // flexGrow = new StyleFloat(1)
                }
            };

            if (attribute is not TitleAttribute titleAttribute)
                return base.CreatePropertyGUI();

            Color accentColor = Color.Lerp(titleAttribute.color, new Color(0, 0, 0, 1f), EditorGUIUtility.isProSkin ? 0.05f : 0.4f);
            TextElement text = new TextElement
            {
                text = titleAttribute.text,
                style =
                {
                    fontSize = 12,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold),
                    color = new StyleColor(accentColor),
                    
                    paddingBottom = new StyleLength(8),
                    paddingLeft = new StyleLength(6),
                    paddingRight = new StyleLength(6),
                    paddingTop = new StyleLength(8),
                }
            };

            container.Add(container2);
            container.Add(text);
            return container;
        }
#else
        private float CalculateHeight()
        {
            TitleAttribute titleAttribute = attribute as TitleAttribute;
            var height = titleStyle.CalcHeight(new GUIContent(titleAttribute.text), EditorGUIUtility.currentViewWidth);
            height += 4;

            if (titleAttribute.spacingTop)
                height += 12;

            return height;
        }

        public override float GetHeight()
        {
            if (titleStyle == null)
                Init();

            if (attribute is not TitleAttribute titleAttribute)
                return base.GetHeight();

            return CalculateHeight();
        }

        public override void OnGUI(Rect position)
        {
            if (titleStyle == null)
                Init();

            if (attribute is not TitleAttribute titleAttribute)
                return;

            position.height -= 4;
            if (titleAttribute.spacingTop)
            {
                position.y += 12;
                position.height -= 12;
            }

            var rect = new Rect(position);
            rect.width = 2;
            EditorGUI.DrawRect(rect, rectColor);

            var rect2 = new Rect(position);
            rect2.y += rect2.height;
            rect2.height = 1;
            EditorGUI.DrawRect(rect2, new Color(0, 0, 0, 0.15f));

            position.x += 2;
            position.width -= 2;
            EditorGUI.DrawRect(position, backgroundColor);

            titleStyle.normal.textColor = Color.Lerp(titleAttribute.color, new Color(0, 0, 0, 1f), EditorGUIUtility.isProSkin ? 0.05f : 0.4f);
            EditorGUI.LabelField(position, titleAttribute.text, titleStyle);
        }

        public static void OnGUILayout(string title)
        {
            if (titleStyle == null)
                Init();

            GUILayout.Space(8);
            EditorGUILayout.LabelField(title, titleStyle);
        }
#endif

        private static void Init()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.normal.textColor = EditorStyles.label.normal.textColor;
            // titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.font = EditorStyles.boldFont;
            titleStyle.stretchWidth = true;
            titleStyle.padding = new RectOffset(6, 4, 4, 4);

            backgroundColor = EditorGUIUtility.isProSkin ? new Color(30 / 255f, 30 / 255f, 30 / 255f) : new Color(1f, 1f, 1f);
            backgroundColor.a = 0.3f;

            rectColor = titleStyle.normal.textColor;
            rectColor.a = 0.5f;
        }
    }
}
#endif