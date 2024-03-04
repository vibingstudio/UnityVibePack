#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace VibePack
{
    public class SpriteRuler : MonoBehaviour
    {
        [SerializeField] Vector2 spriteSize;
        SpriteRenderer render;

        public Vector2 GetSpriteSize()
        {
            if (render == null)
                render = GetComponent<SpriteRenderer>();

            spriteSize = render.bounds.size;
            return spriteSize;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteRuler))]
    public class SpriteRulerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpriteRuler spriteRuler = (SpriteRuler)target;
            if (GUILayout.Button("Get Sprite Size"))
            {
                Vector2 spriteSize = spriteRuler.GetSpriteSize();
                EditorGUILayout.LabelField("Sprite Size", spriteSize.ToString());
                Repaint();
            }
        }
    }
#endif
}
