using System;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class ElementHandler
    {
        public float heightCache;
        public float helpBoxHeightCache;

        private NativeHandler _nativeHandler = new NativeHandler();
        private NativeHandler _propertyNativeHandler = new NativeHandler();
        private MethodHandler _methodHandler = new MethodHandler();

        private LBReorderableListWrapper _reorderableList = new LBReorderableListWrapper();

        public LBReorderableListWrapper ReorderableList => _reorderableList;

        public void GetHeight(InspectorElement element, GUIContent label, bool includeChildren)
        {
            float height = 0;

            if (element.helpBox != null && element.GetResult(element.helpBox))
            {
                helpBoxHeightCache =
                    Constants.helpBoxStyle.CalcHeight(element.helpBox.label, EditorGUIUtility.currentViewWidth);
                height += helpBoxHeightCache;
                height += Constants.space;
            }

            switch (element.type)
            {
                case InspectorElement.EType.SerializedProperty:
                case InspectorElement.EType.Property:
                    height += GetFieldOrPropertyHeight(element, label, includeChildren);
                    break;
                case InspectorElement.EType.Method:
                    _methodHandler.GetHeight(element);
                    height += _methodHandler.heightCache;
                    break;
            }

            heightCache = height;
        }

        private float GetFieldOrPropertyHeight(InspectorElement element, GUIContent label, bool includeChildren)
        {
            float height = 0;

            var property = element.serializedProperty;

            var nativeHandler = _nativeHandler;
            if (element.type == InspectorElement.EType.Property) nativeHandler = _propertyNativeHandler;
            nativeHandler.handler = NativeHandler.GetHandler(property);

            height += nativeHandler.DecorateDrawersGetHeight();

            if (nativeHandler.hasPropertyDrawer || !includeChildren)
            {
                //Debug.Log("Get height native" + " " + element.elementPath);
                nativeHandler.GetHeight(property, label, includeChildren);
                height += nativeHandler.heightCache;
            }
            else if (property.IsNonStringArray())
            {
                _reorderableList.Initialize(element);
                _reorderableList.GetHeight();
                height += _reorderableList.heightCache;
                //Debug.Log("Get height array" + " " + element.elementPath + " " +_reorderableList.GetHashCode());
            }
            else
            {
                // First property with custom label
                height += LBEditorGUI.GetSinglePropertyHeight(property, label);
                bool childrenAreExpanded = property.isExpanded && LBEditorGUI.HasVisibleChildFields(property);
                //Debug.Log("Get height multiple" + " " + element.elementPath + " " + childrenAreExpanded);

                // Loop through all child properties
                var tc = LBEditorGUIUtility.TempContent(property.displayName, nativeHandler.tooltip);
                if (childrenAreExpanded)
                {
                    foreach (var subSorters in element.subSorters)
                    {
                        if (subSorters is InspectorElement childElement && childElement.shouldShow)
                        {
                            var childProperty = childElement.serializedProperty;
                            bool childIncludeChildren = childProperty.IsNonStringArray() ||
                                                        childProperty.isExpanded &&
                                                        LBEditorGUI.HasVisibleChildFields(childProperty);
                            //Debug.Log("---- Get height child element" + " " + childProperty.propertyPath + " " + childrenAreExpanded + " " + childElement.guiHandler.GetHashCode());
                            childElement.guiHandler.GetHeight(childElement,
                                new GUIContent(childElement.displayName), childIncludeChildren);
                            height += childElement.guiHandler.heightCache;
                            height += Constants.space;
                        }
                        else if (subSorters is InspectorGroup subGroup && subGroup.shouldShow)
                        {
                            subGroup.layoutHandler.GetHeight(subGroup);
                            height += subGroup.layoutHandler.heightCache;
                            height += Constants.space;
                        }
                        else if (subSorters is InspectorDecorator decorator)
                        {
                            height += decorator.decorateHandler.GetHeight();
                        }
                    }
                }
            }

            return height;
        }

        public bool OnGUI(Rect position, InspectorElement element, GUIContent label, bool includeChildren)
        {
            bool onGUIResult = false;

            if (element.helpBox != null && element.GetResult(element.helpBox))
            {
                var helpBoxRect = new Rect(position) { height = helpBoxHeightCache };
                EditorGUI.LabelField(helpBoxRect, element.helpBox.label, Constants.helpBoxStyle);
                position.y += helpBoxHeightCache + Constants.space;
                position.height -= helpBoxHeightCache + Constants.space;
            }

            EditorGUI.BeginDisabledGroup(!element.enabled);
            {
                var visibleRect = Reflection.GUIClip.visibleRect;

                switch (element.type)
                {
                    case InspectorElement.EType.SerializedProperty:
                        onGUIResult = DrawFieldOrPropertyGUI(position, element, label, includeChildren, visibleRect);
                        break;
                    case InspectorElement.EType.Property:
                        var target = element.parentTarget;
                        var getMethod = element.propertyInfo.GetGetMethod();
                        var preFieldValue = element.fieldInfo.GetValue(target);
                        var prePropertyValue = getMethod == null ? null : element.propertyInfo.GetValue(target);

                        EditorGUI.BeginChangeCheck();
                        onGUIResult = DrawFieldOrPropertyGUI(position, element, label, includeChildren, visibleRect);
                        if (EditorGUI.EndChangeCheck())
                        {
                            element.inspectorObject.editor.serializedObject.ApplyModifiedProperties();
                            if (element.propertyInfo.GetSetMethod() != null)
                            {
                                var curFieldValue = element.fieldInfo.GetValue(target);
                                var curPropertyValue = getMethod == null ? null : element.propertyInfo.GetValue(target);
                                if ((getMethod != null && curPropertyValue == null) || (curFieldValue == null))
                                {
                                    if ((getMethod != null && prePropertyValue != null) || (preFieldValue != null))
                                    {
                                        element.fieldInfo.SetValue(target,
                                            getMethod != null ? prePropertyValue : preFieldValue);
                                        element.propertyInfo.SetValue(target, null);
                                    }
                                }
                                else if ((getMethod != null && !curPropertyValue.Equals(prePropertyValue)) ||
                                         (!curFieldValue.Equals(preFieldValue)))
                                {
                                    element.fieldInfo.SetValue(target,
                                        getMethod != null ? prePropertyValue : preFieldValue);
                                    element.propertyInfo.SetValue(target,
                                        getMethod != null ? curPropertyValue : curFieldValue);
                                }
                            }
                        }
                        else
                        {
                            if (getMethod != null && prePropertyValue != preFieldValue)
                                element.fieldInfo.SetValue(target, prePropertyValue);
                        }


                        break;
                    case InspectorElement.EType.Method:
                        onGUIResult = _methodHandler.OnGUI(position, element, label);
                        break;
                }
            }
            EditorGUI.EndDisabledGroup();

            return onGUIResult;
        }

        private bool DrawFieldOrPropertyGUI(Rect position, InspectorElement element, GUIContent label,
            bool includeChildren, Rect visibleArea)
        {
            var property = element.serializedProperty;

            var nativeHandler = _nativeHandler;
            if (element.type == InspectorElement.EType.Property) nativeHandler = _propertyNativeHandler;
            if (nativeHandler.handler == null) nativeHandler.handler = NativeHandler.GetHandler(property);

            var rect = new Rect(position);
            rect.height = nativeHandler.DecorateDrawersGetHeight();
            nativeHandler.DecorateDrawersOnGUI(rect);

            position.y += rect.height;
            position.height -= rect.height;
            if (nativeHandler.hasPropertyDrawer || !includeChildren)
            {
                //Debug.Log("OnGUI native" + " " + element.elementPath);
                var layout = element.layout;
                if (element.isBelongToDynamicObject && element.inspectorGroup == null)
                    layout = element.dynamicElementParent.layout;
                return nativeHandler.OnGUI(position, property, label, includeChildren, layout);
            }
            else if (property.IsNonStringArray())
            {
                // Calculate visibility rect specifically for reorderable list as when applied for the whole serialized object,
                // it causes collapsed out of sight array elements appear thus messing up scroll-bar experience
                var screenPos = GUIUtility.GUIToScreenPoint(position.position);

                screenPos.y = Mathf.Clamp(screenPos.y,
                    Reflection.GUIView.currentScreenPosition?.yMin ?? 0,
                    Reflection.GUIView.currentScreenPosition?.yMax ?? Screen.height);

                Rect listVisibility = new Rect(screenPos.x, screenPos.y,
                    Reflection.GUIView.currentScreenPosition?.width ?? Screen.width,
                    Reflection.GUIView.currentScreenPosition?.height ?? Screen.height);

                listVisibility = GUIUtility.ScreenToGUIRect(listVisibility);
                
                _reorderableList.Initialize(element);
                _reorderableList.Draw(position, listVisibility);
                //Debug.Log("OnGUI array" + " " + element.elementPath + " " + _reorderableList.GetHashCode());
                return !includeChildren && property.isExpanded;
            }
            else
            {
                // Remember state
                Vector2 oldIconSize = EditorGUIUtility.GetIconSize();
                bool wasEnabled = GUI.enabled;
                int origIndent = EditorGUI.indentLevel;

                int relIndent = origIndent - property.depth;

                SerializedProperty prop = property.Copy();

                position.height = LBEditorGUI.GetSinglePropertyHeight(prop, label);

                // First property with custom label
                EditorGUI.indentLevel = prop.depth + relIndent;
                bool childrenAreExpanded = nativeHandler.DefaultPropertyField(position, prop, label) &&
                                           LBEditorGUI.HasVisibleChildFields(prop);
                position.y += position.height + Constants.space;

                //Debug.Log("OnGUI multiple" + " " + element.elementPath + " " + childrenAreExpanded);

                // Loop through all child properties
                if (childrenAreExpanded)
                {
                    EditorGUI.indentLevel = prop.depth + 1 + relIndent;
                    foreach (var subSorter in element.subSorters)
                    {
                        if (subSorter is InspectorElement childElement && childElement.shouldShow)
                        {
                            var childProperty = childElement.serializedProperty;
                            bool childIncludeChildren = childProperty.IsNonStringArray() ||
                                                        childProperty.isExpanded &&
                                                        LBEditorGUI.HasVisibleChildFields(childProperty);
                            /*Debug.Log("---- OnGUI child" + " " + childProperty.propertyPath + " " +
                                      childIncludeChildren);*/
                            position.height = childElement.guiHandler.heightCache;
                            if (position.Overlaps(visibleArea))
                                childElement.guiHandler.OnGUI(position, childElement,
                                    new GUIContent(childElement.displayName), childIncludeChildren);
                            position.y += position.height;
                            position.y += Constants.space;
                        }
                        else if (subSorter is InspectorGroup subGroup && subGroup.shouldShow)
                        {
                            position.height = subGroup.layoutHandler.heightCache;
                            if (position.Overlaps(visibleArea))
                                subGroup.layoutHandler.OnGUI(position, subGroup);
                            position.y += position.height;
                            position.y += Constants.space;
                        }
                        else if (subSorter is InspectorDecorator decorator)
                        {
                            position.height = decorator.decorateHandler.GetHeight();
                            if (position.Overlaps(visibleArea))
                                decorator.decorateHandler.OnGUI(position);
                            position.y += position.height;
                        }
                    }
                }

                // Restore state
                GUI.enabled = wasEnabled;
                EditorGUIUtility.SetIconSize(oldIconSize);
                EditorGUI.indentLevel = origIndent;

                return false;
            }
        }
    }
}