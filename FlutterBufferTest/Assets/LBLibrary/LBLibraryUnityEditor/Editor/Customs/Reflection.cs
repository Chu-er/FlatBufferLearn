using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    [InitializeOnLoad]
    public static class Reflection
    {
        static Reflection()
        {
            var runtimeAssembly = Assembly.Load("UnityEngine");
            var editorAssembly = Assembly.Load("UnityEditor");

            var ScriptAttributeUtility = editorAssembly.GetType("UnityEditor.ScriptAttributeUtility");
            GetHandler =
                ScriptAttributeUtility.GetMethod("GetHandler", BindingFlags.NonPublic | BindingFlags.Static);

            var PropertyHandler = editorAssembly.GetType("UnityEditor.PropertyHandler");
            m_DecoratorDrawers = PropertyHandler.GetField("m_DecoratorDrawers",
                BindingFlags.NonPublic | BindingFlags.Instance);
            isCurrentlyNested = PropertyHandler.GetProperty("isCurrentlyNested",
                BindingFlags.NonPublic | BindingFlags.Instance);
            propertyDrawer = PropertyHandler.GetProperty("propertyDrawer",
                BindingFlags.NonPublic | BindingFlags.Instance);
            m_NestingLevel = PropertyHandler.GetField("m_NestingLevel",
                BindingFlags.NonPublic | BindingFlags.Instance);
            IncrementNestingContext = PropertyHandler.GetMethod("IncrementNestingContext",
                BindingFlags.Public | BindingFlags.Instance);
            tooltip = PropertyHandler.GetField("tooltip",
                BindingFlags.Public | BindingFlags.Instance);

            GetPropertyHeightSafe = typeof(PropertyDrawer).GetMethod("GetPropertyHeightSafe",
                BindingFlags.NonPublic | BindingFlags.Instance);
            OnGUISafe = typeof(PropertyDrawer).GetMethod("OnGUISafe",
                BindingFlags.NonPublic | BindingFlags.Instance);

            DefaultPropertyField =
                typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static);
            ObjectFieldValidatorOptions =
                typeof(EditorGUI).GetNestedType("ObjectFieldValidatorOptions", BindingFlags.NonPublic);
            ValidateObjectFieldAssignment =
                typeof(EditorGUI).GetMethod("ValidateObjectFieldAssignment",
                    BindingFlags.NonPublic | BindingFlags.Static, null,
                    new Type[]
                    {
                        typeof(Object[]), typeof(Type), typeof(SerializedProperty), ObjectFieldValidatorOptions
                    }, null);

            var GUIViewType = editorAssembly.GetType("UnityEditor.GUIView");
            GUIView.guiViewCurrent = GUIViewType.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            GUIView.guiViewCurrentScreenPosition =
                GUIViewType.GetProperty("screenPosition", BindingFlags.Public | BindingFlags.Instance);

            var GUIClipType = runtimeAssembly.GetType("UnityEngine.GUIClip");
            GUIClip.guiClipVisibleRect =
                GUIClipType.GetProperty("visibleRect", BindingFlags.NonPublic | BindingFlags.Static);
            DoAddButton =
                typeof(ReorderableList.Defaults).GetMethod("DoAddButton", BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new Type[] { typeof(ReorderableList), typeof(Object) }, null);
        }

        public static MethodInfo GetHandler;

        public static FieldInfo m_DecoratorDrawers;
        public static PropertyInfo isCurrentlyNested;
        public static PropertyInfo propertyDrawer;
        public static FieldInfo m_NestingLevel;
        public static MethodInfo IncrementNestingContext;
        public static FieldInfo tooltip;

        public static MethodInfo GetPropertyHeightSafe;
        public static MethodInfo OnGUISafe;

        public static MethodInfo DefaultPropertyField;
        public static Type ObjectFieldValidatorOptions;
        public static MethodInfo ValidateObjectFieldAssignment;

        public static MethodInfo DoAddButton;

        public class GUIView
        {
            public static PropertyInfo guiViewCurrent;
            public static PropertyInfo guiViewCurrentScreenPosition;

            public static object current => guiViewCurrent.GetValue(null);

            public static Rect? currentScreenPosition
            {
                get
                {
                    if (current == null) return null;
                    return (Rect)guiViewCurrentScreenPosition.GetValue(current);
                }
            }
        }

        public class GUIClip
        {
            public static PropertyInfo guiClipVisibleRect;

            public static Rect visibleRect => (Rect)guiClipVisibleRect.GetValue(null);
        }
    }

    public static class ReflectionExtension
    {
        public static float GetPropertyHeightSafe(this PropertyDrawer drawer, SerializedProperty property,
            GUIContent label)
        {
            return (float)Reflection.GetPropertyHeightSafe.Invoke(drawer, new object[] { property, label });
        }

        public static void OnGUISafe(this PropertyDrawer drawer, Rect position, SerializedProperty property,
            GUIContent label)
        {
            Reflection.OnGUISafe.Invoke(drawer, new object[] { position, property, label });
        }
    }
}