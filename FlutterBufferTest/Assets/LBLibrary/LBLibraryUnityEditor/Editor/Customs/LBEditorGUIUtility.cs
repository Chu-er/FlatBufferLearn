using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class LBEditorGUIUtility : GUIUtility
    {
        private static readonly GUIContent s_Text = new GUIContent();
        private static Texture2D s_InfoIcon;
        private static Texture2D s_WarningIcon;
        private static Texture2D s_ErrorIcon;

        public static Texture2D infoIcon => s_InfoIcon;

        public static Texture2D warningIcon => s_WarningIcon;

        public static Texture2D errorIcon => s_ErrorIcon;

        static LBEditorGUIUtility()
        {
            var infoIcon = typeof(EditorGUIUtility)
                .GetProperty("infoIcon", BindingFlags.NonPublic | BindingFlags.Static);
            s_InfoIcon = (Texture2D)infoIcon.GetValue(null);
            
            
            var warningIcon = typeof(EditorGUIUtility)
                .GetProperty("warningIcon", BindingFlags.NonPublic | BindingFlags.Static);
            s_WarningIcon = (Texture2D)warningIcon.GetValue(null);
            
            var errorIcon = typeof(EditorGUIUtility)
                .GetProperty("errorIcon", BindingFlags.NonPublic | BindingFlags.Static);
            s_ErrorIcon = (Texture2D)errorIcon.GetValue(null);
        }

        internal static GUIContent TempContent(string text, string tip)
        {
            s_Text.image = null;
            s_Text.text = text;
            s_Text.tooltip = tip;
            return s_Text;
        }

        public static Texture2D GetHelpIcon(MessageType type)
        {
            switch (type)
            {
                case MessageType.Info:
                    return infoIcon;
                case MessageType.Warning:
                    return warningIcon;
                case MessageType.Error:
                    return errorIcon;
                default:
                    return null;
            }
        }
        
        public static string GetMethodName(MethodInfo methodInfo)
        {
            string name = methodInfo.Name;
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                name += "_" + parameterInfo.ParameterType;
            }
            return name;
        }
    }
}