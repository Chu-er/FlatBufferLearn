using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public static class Constants
    {
        public static readonly object[] emptyParams = { };

        public static readonly float space = 2f;
        public static readonly float foldoutHeight = 18f;
        public static readonly float foldoutIconWidth = 12f;
        public static readonly float indentPerLevel = 15f;
        public static readonly float toggleWidth = 17;

        public static readonly Vector2 smallIconSize = new Vector2(18f, 18f);

        public static readonly Rect defaultVisibleArea = new Rect(0, 0, float.MaxValue, float.MaxValue);

        public static readonly RectOffset border = new RectOffset(1, 1, 1, 1);

        public static readonly Color gray26 = new Color32(26, 26, 26, 255);
        public static readonly Color gray48 = new Color32(48, 48, 48, 255);
        public static readonly Color gray56 = new Color32(56, 56, 56, 255);
        public static readonly Color gray62 = new Color32(62, 62, 62, 255);
        public static readonly Color gray71 = new Color32(71, 71, 71, 255);
        public static readonly Color gray81 = new Color32(81, 81, 81, 255);
        public static readonly Color gray160 = new Color32(160, 160, 160, 255);

        public static readonly GUIStyle foldoutHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader)
        {
            fixedHeight = foldoutHeight - border.top,
            fontStyle = FontStyle.Normal,
        };

        public static readonly GUIStyle boxStyle = "RL Background";

        public static readonly GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox)
        {
            fontSize = 12,
        };

        public static readonly GUIStyle settingsScrollViewStyle = new GUIStyle(GUI.skin.scrollView)
        {
            padding = new RectOffset(10, 3, 0, 2),
        };

        public static float FoldoutIndent => EditorGUIUtility.hierarchyMode ? 14f : 0f;
    }
}