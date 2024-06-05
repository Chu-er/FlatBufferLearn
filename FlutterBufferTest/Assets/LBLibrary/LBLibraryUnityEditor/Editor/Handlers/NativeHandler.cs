using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class NativeHandler
    {
        public float heightCache;

        public bool hasPropertyDrawer => propertyDrawer != null;

        public float DecorateDrawersGetHeight()
        {
            float height = 0;
            if (m_DecoratorDrawers != null && !isCurrentlyNested)
                foreach (DecoratorDrawer drawer in m_DecoratorDrawers)
                    height += drawer.GetHeight();
            return height;
        }

        public void DecorateDrawersOnGUI(Rect position)
        {
            float oldLabelWidth, oldFieldWidth;

            position.height = 0;
            if (m_DecoratorDrawers != null && !isCurrentlyNested)
            {
                foreach (DecoratorDrawer decorator in m_DecoratorDrawers)
                {
                    position.height = decorator.GetHeight();

                    oldLabelWidth = EditorGUIUtility.labelWidth;
                    oldFieldWidth = EditorGUIUtility.fieldWidth;
                    decorator.OnGUI(position);
                    EditorGUIUtility.labelWidth = oldLabelWidth;
                    EditorGUIUtility.fieldWidth = oldFieldWidth;

                    position.y += position.height;
                }
            }
        }

        public void GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            float height = 0;

            if (propertyDrawer != null)
            {
                var drawer = propertyDrawer;
#if UNITY_2021_2_OR_NEWER
                // Retrieve drawer BEFORE increasing nesting.
                var nestingContext = IncrementNestingContext();
                height += drawer.GetPropertyHeightSafe(property.Copy(),
                    label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
                var disposable = (IDisposable)nestingContext;
                disposable.Dispose();
#elif UNITY_2020_2_OR_NEWER
                try
                {
                    m_NestingLevel++;
                    height += drawer.GetPropertyHeightSafe(property.Copy(),
                        label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
                }
                finally
                {
                    m_NestingLevel--;
                }
#else
                height += propertyDrawer.GetPropertyHeightSafe(property.Copy(),
                    label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
#endif
            }
            else if (!includeChildren)
            {
                height += LBEditorGUI.GetSinglePropertyHeight(property, label);
            }

            heightCache = height;
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren,
            GroupAttribute.ELayout layout)
        {
            float oldLabelWidth, oldFieldWidth;

            if (propertyDrawer != null)
            {
                // Remember widths
                oldLabelWidth = EditorGUIUtility.labelWidth;
                oldFieldWidth = EditorGUIUtility.fieldWidth;
                // Draw with custom drawer - retrieve it BEFORE increasing nesting.
                PropertyDrawer drawer = propertyDrawer;

#if UNITY_2021_2_OR_NEWER
                var nestingContext = IncrementNestingContext();
                drawer.OnGUISafe(position, property.Copy(),
                    label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
                var disposable = (IDisposable)nestingContext;
                disposable.Dispose();
#elif UNITY_2020_2_OR_NEWER
                try
                {
                    m_NestingLevel++;
                    drawer.OnGUISafe(position, property.Copy(),
                        label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
                }
                finally
                {
                    m_NestingLevel--;
                }
#else
                // Draw with custom drawer
                propertyDrawer.OnGUISafe(position, property.Copy(),
                    label ?? LBEditorGUIUtility.TempContent(property.displayName, tooltip));
#endif

                // Restore widths
                EditorGUIUtility.labelWidth = oldLabelWidth;
                EditorGUIUtility.fieldWidth = oldFieldWidth;

                return false;
            }
            else
            {
                if (!includeChildren)
                {
                    if (layout == GroupAttribute.ELayout.Horizontal)
                    {
                        if (label != null)
                        {
                            var labelStyle = new GUIStyle("ControlLabel")
                            {
                                fixedWidth = position.width * 0.5f
                            };
                            EditorGUI.PrefixLabel(position, label, labelStyle);
                        }

                        if (property.propertyType != SerializedPropertyType.Boolean)
                        {
                            position.width *= 0.5f;
                            position.x += position.width;
                        }
                        else
                        {
                            position.x += position.width - Constants.toggleWidth;
                            position.width = Constants.toggleWidth;
                        }

                        bool onGUIResult = DefaultPropertyField(position, property, GUIContent.none);
                        return onGUIResult;
                    }
                    else
                    {
                        return DefaultPropertyField(position, property, label);
                    }
                }

                return false;
            }
        }

        //Reflection

        public object handler;

        public List<DecoratorDrawer> m_DecoratorDrawers =>
            (List<DecoratorDrawer>)Reflection.m_DecoratorDrawers.GetValue(handler);

        public string tooltip => (string)Reflection.tooltip.GetValue(handler);

        public int m_NestingLevel
        {
            get => (int)Reflection.m_NestingLevel.GetValue(handler);
            set => Reflection.m_NestingLevel.SetValue(handler, value);
        }

        public bool isCurrentlyNested => (bool)Reflection.isCurrentlyNested.GetValue(handler);

        public PropertyDrawer propertyDrawer => (PropertyDrawer)Reflection.propertyDrawer.GetValue(handler);

        public static object GetHandler(SerializedProperty property)
        {
            return Reflection.GetHandler.Invoke(null, new object[] { property });
        }

        public object IncrementNestingContext()
        {
            return Reflection.IncrementNestingContext.Invoke(handler, Constants.emptyParams);
        }

        public bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            return (bool)Reflection.DefaultPropertyField.Invoke(null, new object[] { position, property, label });
        }
    }
}