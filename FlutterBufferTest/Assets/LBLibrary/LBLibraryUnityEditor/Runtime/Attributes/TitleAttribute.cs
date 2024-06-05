using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class TitleAttribute : DecoratorAttribute
    {
        public string text;

        public TitleAttribute(string text)
        {
            this.text = text;
            this.orientation = EOrientation.Up;
        }

#if UNITY_EDITOR
        private static class Styles
        {
            public static readonly float textHeight = 20f;
            public static readonly float lineHeight = 2f;

            public static readonly RectOffset margin = new RectOffset(0, 0, 4, 2);

            public static readonly Color gray160 = new Color32(160, 160, 160, 255);

            public static readonly GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 14,
            };

            static Styles()
            {
                labelStyle.normal.textColor = gray160;
            }
        }

        public override float GetHeight()
        {
            return Styles.margin.vertical + (!string.IsNullOrEmpty(text) ? Styles.textHeight : 0f) + Styles.lineHeight;
        }


        public override bool OnGUI(Rect position)
        {
            Rect rect = new Rect(position);
            rect.y += Styles.margin.top;
            if (!string.IsNullOrEmpty(text))
            {
                rect.height = Styles.textHeight;
                GUI.Label(rect, text, Styles.labelStyle);
                rect.y += Styles.textHeight;
            }
            rect.height = Styles.lineHeight;
            EditorGUI.DrawRect(rect, Styles.gray160);
            return true;
        }
#endif
    }
}