using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    public static class LBEditorGUI
    {
        public const float kSingleLineHeight = 18f;
        public const float kStructHeaderLineHeight = 18;
        public const float kVerticalSpacingMultiField = 2;

        public static BindingFlags elementBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static bool ValidProperties(PropertyInfo x, IEnumerable<InspectorElement> fieldSearchRange)
        {
            return x.GetCustomAttribute<ShowInInspectorAttribute>() != null &&
                   fieldSearchRange.Count(y => y.serializedProperty != null && y.serializedProperty.name ==
                       x.GetCustomAttribute<ShowInInspectorAttribute>().fieldName) > 0;
        }

        public static bool IsChildrenIncluded(InspectorElement ele)
        {
            switch (ele.type)
            {
                case InspectorElement.EType.SerializedProperty:
                case InspectorElement.EType.Property:
                    return IsChildrenIncluded(ele.serializedProperty);
                case InspectorElement.EType.Method:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsChildrenIncluded(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector4:
                    return true;
                default:
                    return false;
            }
        }

        public static bool HasVisibleChildFields(InspectorElement element, bool isUIElements = false)
        {
            switch (element.type)
            {
                case InspectorElement.EType.SerializedProperty:
                case InspectorElement.EType.Property:
                    return HasVisibleChildFields(element.serializedProperty);
                case InspectorElement.EType.Method:
                    return false;
                default:
                    return false;
            }
        }

        public static bool HasVisibleChildFields(SerializedProperty property, bool isUIElements = false)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
#if UNITY_2021_1_OR_NEWER
                case SerializedPropertyType.Hash128:
#endif
                    return false;
            }

            if (property.IsNonStringArray()) return false;

            return property.hasVisibleChildren;
        }

        public static bool LabelHasContent(GUIContent label)
        {
            if (label == null)
            {
                return true;
            }

            // @TODO: find out why checking for GUIContent.none doesn't work
            return label.text != string.Empty || label.image != null;
        }

        // Get the height needed for a ::ref::PropertyField control, not including its children and not taking custom PropertyDrawers into account.
        public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null)
                return kSingleLineHeight;
            return GetPropertyHeight(property.propertyType, label);
        }

        // Get the height needed for a simple builtin control type.
        public static float GetPropertyHeight(SerializedPropertyType type, GUIContent label)
        {
            if (type == SerializedPropertyType.Vector3 || type == SerializedPropertyType.Vector2 ||
                type == SerializedPropertyType.Vector4 ||
                type == SerializedPropertyType.Vector3Int || type == SerializedPropertyType.Vector2Int)
            {
                return (!LabelHasContent(label) || EditorGUIUtility.wideMode
                           ? 0f
                           : kStructHeaderLineHeight + kVerticalSpacingMultiField) +
                       kSingleLineHeight;
            }

            if (type == SerializedPropertyType.Rect || type == SerializedPropertyType.RectInt)
            {
                return (!LabelHasContent(label) || EditorGUIUtility.wideMode
                           ? 0f
                           : kStructHeaderLineHeight + kVerticalSpacingMultiField) +
                       kSingleLineHeight * 2 + kVerticalSpacingMultiField;
            }

            // Bounds field has label on its own line even in wide mode because the words "center" and "extends"
            // would otherwise eat too much of the label space.
            if (type == SerializedPropertyType.Bounds || type == SerializedPropertyType.BoundsInt)
            {
                return (!LabelHasContent(label) ? 0f : kStructHeaderLineHeight + kVerticalSpacingMultiField) +
                       kSingleLineHeight * 2 + kVerticalSpacingMultiField;
            }

            return kSingleLineHeight;
        }

        public static void ElementField(InspectorElement element, Rect position)
        {
            ElementField(element, position, new GUIContent(element.displayName), IsChildrenIncluded(element));
        }

        public static void ElementField(InspectorElement element, Rect position, GUIContent label)
        {
            ElementField(element, position, label, IsChildrenIncluded(element));
        }

        public static void ElementField(InspectorElement element, Rect position, bool includeChildren)
        {
            ElementField(element, position, new GUIContent(element.displayName), includeChildren);
        }

        public static void ElementField(InspectorElement element, Rect position, GUIContent label, bool includeChildren)
        {
            position.height = element.guiHandler.heightCache;
            element.guiHandler.OnGUI(position, element, label, includeChildren);
        }
        
        public static Object ValidateObjectFieldAssignment(Object[] references, Type objType,
            SerializedProperty property)
        {
            return (Object)Reflection.ValidateObjectFieldAssignment.Invoke(null,
                new object[]
                    { references, objType, property, Reflection.ObjectFieldValidatorOptions.GetEnumValues().GetValue(0)});
        }
    }

    public static class LBEditorGUILayout
    {
        public static void ElementField(InspectorElement element, params GUILayoutOption[] options)
        {
            var includeChildren = LBEditorGUI.IsChildrenIncluded(element);
            var label = new GUIContent(element.displayName);
            ElementField(element, label, includeChildren, options);
        }

        public static void ElementField(InspectorElement element, GUIContent label, params GUILayoutOption[] options)
        {
            var includeChildren = LBEditorGUI.IsChildrenIncluded(element);
            ElementField(element, label, includeChildren, options);
        }

        public static void ElementField(InspectorElement element, bool includeChildren,
            params GUILayoutOption[] options)
        {
            var label = new GUIContent(element.displayName);
            ElementField(element, label, includeChildren, options);
        }

        public static void ElementField(InspectorElement element, GUIContent label, bool includeChildren,
            params GUILayoutOption[] options)
        {
            element.guiHandler.GetHeight(element, label, includeChildren);
            var height = element.guiHandler.heightCache;
            LBEditorGUI.ElementField(element, GUILayoutUtility.GetRect(0, height + 1, options));
        }
    }
}