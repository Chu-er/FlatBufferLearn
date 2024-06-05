using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    public class LBReorderableListWrapper
    {
        public static class Styles
        {
            public static readonly float headerHeight = 18f;
            public static readonly float headerIndent = Constants.FoldoutIndent;
            public static readonly float sizeWidth = 50f;
            public static readonly float addButtonWidth = 28f;
            public static readonly float elementHeaderIndent = 14f;
            public static readonly float minusButtonWidth = 26f;
            public static readonly float minusButtonHeight = 18f;

            public static readonly GUIStyle headerStyle = new GUIStyle(EditorStyles.foldoutHeader)
            {
                fixedHeight = headerHeight,
                fontStyle = FontStyle.Normal,
            };

            public static readonly GUIStyle addButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedWidth = addButtonWidth,
                fixedHeight = headerHeight,
            };

            public static readonly GUIStyle minusButtonStyle = new GUIStyle("IconButton")
            {
                fixedWidth = minusButtonWidth,
                fixedHeight = minusButtonHeight,
                alignment = TextAnchor.MiddleCenter,
            };
        }
        
        public ReorderableList.AddCallbackDelegate onAddCallback;
        public ReorderableList.RemoveCallbackDelegate onRemoveCallback;

        public float heightCache;

        private ReorderableList _rList;
        private int _deleteIndex = -1;
        private bool _needAddNewElement = false;

        public InspectorElement element;

        public ReorderableList RList => _rList;

        public void Initialize(InspectorElement element)
        {
            this.element = element;
            if (_rList == null)
            {
                _rList = new ReorderableList(element.serializedProperty.serializedObject, element.serializedProperty,
                    true,
                    true, false, false);
                SetCallbacks();
            }
            else
            {
                _rList.serializedProperty = element.serializedProperty;
            }
        }

        public void GetHeight()
        {
            heightCache = Styles.headerHeight + Constants.border.vertical +
                          (element.isExpanded ? GetContentHeight() : 0);
        }

        public float GetContentHeight()
        {
            float height = Constants.space;
            for (int i = 0; i < _rList.count; i++)
            {
                height += ElementHeight(i);
#if UNITY_2020_2_OR_NEWER
                height += Constants.space;
#endif
            }

            height += Constants.border.bottom;

            return height;
        }

        public void Draw(Rect position, Rect visibleArea)
        {
            float indent = EditorGUI.indentLevel * 15f;
            position.x += indent;
            position.width -= indent;
            DrawCustomHeader(position);
            //If expand draw elements
            if (element.isExpanded)
            {
                position.y += Styles.headerHeight + Constants.border.vertical;
                position.height -= Styles.headerHeight + Constants.border.vertical;
                visibleArea.y -= position.y;
                if (Event.current != null && Event.current.type == EventType.Repaint)
                    Constants.boxStyle.Draw(position, false, false, false, false);
#if UNITY_2019_4
                _rList.DoList(position);
#else
                _rList.DoList(position, visibleArea);
#endif
            }
        }

        public void DrawCustomHeader(Rect position)
        {
            var property = _rList.serializedProperty;

            //Draw custom header
            position.height = Styles.headerHeight;
            if (ReorderableList.defaultBehaviours == null)
            {
                var defaultBehaviours = new ReorderableList.Defaults();
                defaultBehaviours.DrawHeaderBackground(position);
            }
            else
            {
                ReorderableList.defaultBehaviours.DrawHeaderBackground(position);
            }

            //Cache some value
            var rect = new Rect(position);

            //Custom draw
            //Foldout header
            rect.x += Styles.headerIndent + Constants.border.left;
            rect.y += Constants.border.top;
            rect.width -= Styles.headerIndent + 2 * ReorderableList.Defaults.padding + Styles.sizeWidth +
                          Styles.addButtonWidth;

            if (!property.hasMultipleDifferentValues) EditorGUI.BeginProperty(rect, GUIContent.none, property);
            {
                bool prevEnabled = GUI.enabled;
                GUI.enabled = true;
                property.isExpanded =
                    EditorGUI.BeginFoldoutHeaderGroup(rect, property.isExpanded, element.displayName,
                        Styles.headerStyle) && _rList.count > 0;
                EditorGUI.EndFoldoutHeaderGroup();
                GUI.enabled = prevEnabled;
                
                /*if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        UObject[] objReferences = DragAndDrop.objectReferences;
                        foreach (var o in objReferences)
                        {
                            UObject validatedObject = LBEditorGUI.ValidateObjectFieldAssignment(new[] { o }, typeof(UObject), _rList.serializedProperty);
                            if (validatedObject != null)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                            }
                            else continue;

                            if (Event.current.type == EventType.DragPerform)
                            {
                                DoAddButton(_rList, validatedObject);
                            }
                        }
                        DragAndDrop.AcceptDrag();
                        Event.current.Use();
                    }
                }

                if (Event.current.type == EventType.DragExited)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    Event.current.Use();
                }*/
            }
            if (!property.hasMultipleDifferentValues) EditorGUI.EndProperty();

            //Size field
            rect.x += rect.width + ReorderableList.Defaults.padding;
            rect.width = Styles.sizeWidth;
            var preIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var sizeProperty = property.FindPropertyRelative("Array.size");
            var preSize = sizeProperty.intValue;
            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.PropertyField(rect, sizeProperty, GUIContent.none);
                EditorGUI.LabelField(rect, new GUIContent(""));
            }
            if (EditorGUI.EndChangeCheck() && sizeProperty.intValue != preSize)
            {
                _rList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_rList.serializedProperty.serializedObject.targetObject);
                if (sizeProperty.intValue > preSize)
                {
                    for (int i = preSize; i < sizeProperty.intValue; i++)
                    {
                        element.AddArrayElementIfNotExist(_rList.serializedProperty, i);
                    }
                }
                else
                {
                    element.RemoveAnyArrayElement(_rList.serializedProperty);
                }
            }
            EditorGUI.indentLevel = preIndentLevel;

            //Add button
            rect.x += rect.width + 3;
            rect.width = Styles.addButtonWidth;
            if (GUI.Button(rect, "✚", Styles.addButtonStyle))
            {
                _needAddNewElement = true;
                property.isExpanded = true;
            }
        }

        private void SetCallbacks()
        {
            _rList.showDefaultBackground = false;
            _rList.drawHeaderCallback = null;
            _rList.elementHeightCallback = ElementHeight;
            _rList.drawElementCallback = CustomDrawElement;
            _rList.drawElementBackgroundCallback = CustomDrawElementBackground;
            _rList.drawFooterCallback = CustomDrawFooter;

            _rList.headerHeight = 0;
            _rList.footerHeight = 0;
        }

        private float ElementHeight(int index)
        {
            //Invalid or not expanded do nothing
            if (_rList.count == 0 || index < 0 || index >= _rList.count) return 0;

            var child = element.GetArrayElement(index);
            var childProperty = child.serializedProperty;

            //Custom get height
            float height = 0;
            child.guiHandler.GetHeight(child, new GUIContent(childProperty.displayName),
                LBEditorGUI.IsChildrenIncluded(childProperty));
            height += child.guiHandler.heightCache;

#if UNITY_2019_4
            height += Constants.space;
#endif

            return height;
        }

        private void CustomDrawElement(Rect baseRect, int index, bool active, bool focused)
        {
            //Invalid or not expanded do nothing
            if (_rList.count == 0 || index < 0 || index >= _rList.count) return;

            var childProperty = _rList.serializedProperty.GetArrayElementAtIndex(index);
            var child = element.GetArrayElement(index);
            int indexCache = index;

            baseRect.y -= 1;

#if UNITY_2019_4
            baseRect.y += 2;
#endif

            //Custom draw
            Rect rect = new Rect(baseRect);
            float indent = LBEditorGUI.HasVisibleChildFields(childProperty) ? Styles.elementHeaderIndent : 0;
            rect.x += indent;
            rect.width -= Styles.minusButtonWidth + indent + Constants.border.right;
            rect.height -= Constants.space;
            child.guiHandler.OnGUI(rect, child, new GUIContent(childProperty.displayName),
                LBEditorGUI.IsChildrenIncluded(child.serializedProperty.Copy()));

            rect.x += rect.width + 3;
            rect.y = baseRect.y + (baseRect.height - Styles.minusButtonHeight) / 2f - 1;
            rect.width = Styles.minusButtonWidth;
            rect.height = Styles.minusButtonHeight;
            if (GUI.Button(rect, "✖", Styles.minusButtonStyle)) _deleteIndex = indexCache;
        }

        private void CustomDrawElementBackground(Rect rect, int index, bool active, bool focused)
        {
            //Invalid or not expanded do nothing
            if (_rList.count == 0 || index < 0 || index >= _rList.count) return;

            rect.x += Constants.border.left;
            rect.y -= Constants.space;
#if UNITY_2019_4
            rect.y += 2;
#endif
            rect.width -= Constants.border.horizontal;
            ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, active, focused, true);
        }

        private void CustomDrawFooter(Rect rect)
        {
            //do delete element in draw footer to avoid index error
            if (_deleteIndex >= 0)
            {
                _rList.index = _deleteIndex;
                element.RemoveAnyArrayElement(_rList.serializedProperty);
                ReorderableList.defaultBehaviours.DoRemoveButton(_rList);
                _rList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_rList.serializedProperty.serializedObject.targetObject);
                onRemoveCallback?.Invoke(_rList);
            }
            if (_needAddNewElement)
            {
                ReorderableList.defaultBehaviours.DoAddButton(_rList);
                _rList.serializedProperty.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_rList.serializedProperty.serializedObject.targetObject);
                element.AddArrayElementIfNotExist(_rList.serializedProperty, _rList.count - 1);
                onAddCallback?.Invoke(_rList);
            }

            _deleteIndex = -1;
            _needAddNewElement = false;
        }

        internal void DoAddButton(ReorderableList list, UObject value)
        {
            Reflection.DoAddButton.Invoke(this, new object[] { list, value });
        }
    }
}