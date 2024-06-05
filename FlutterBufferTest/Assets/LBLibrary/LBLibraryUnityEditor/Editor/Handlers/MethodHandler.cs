using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    public class MethodHandler
    {
        private class Styles
        {
            public static GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        }

        public float heightCache;

        private (bool, bool) DoFoldoutBoxWithButton(Rect position, bool foldout, GUIContent label,
            Action<Rect> doContent)
        {
            float buttonWidth = 80f;

            Rect rect = new Rect(position.x, position.y, position.width, Constants.foldoutHeight);
            ReorderableList.defaultBehaviours.DrawHeaderBackground(rect);
            rect.x += Constants.FoldoutIndent + Constants.border.left;
            rect.y += Constants.border.top;
            rect.width = 0;
            var preEnabled = GUI.enabled;
            GUI.enabled = true;
            bool isFoldout =
                EditorGUI.BeginFoldoutHeaderGroup(rect, foldout, GUIContent.none, Constants.foldoutHeaderStyle);
            EditorGUI.EndFoldoutHeaderGroup();
            GUI.enabled = preEnabled;
            rect.x += ReorderableList.Defaults.padding;
            rect.width = position.width - 4f * ReorderableList.Defaults.padding;
            Styles.btnStyle.fixedHeight = rect.height;
            bool isButtonClick = GUI.Button(rect, label, Styles.btnStyle);
            if (isFoldout)
            {
                var headerHeight = Constants.foldoutHeight + Constants.border.vertical;
                position.y += headerHeight;
                var listRect = new Rect(position) { height = position.height - headerHeight };
                if (Event.current != null && Event.current.type == EventType.Repaint)
                {
                    Constants.boxStyle.Draw(listRect, false, false, false, false);
                }
                int preIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                doContent?.Invoke(listRect);
                EditorGUI.indentLevel = preIndentLevel;
            }

            return (isFoldout, isButtonClick);
        }

        public void GetHeight(InspectorElement element)
        {
            float height = 0;

            if (element.inspectorObject.editor.targets.Length > 1)
            {
                height = EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var parameters = element.methodParameters;

                if (parameters == null || !element.isExpanded)
                {
                    height += Constants.foldoutHeight + Constants.border.vertical;
                }
                else
                {
                    height += Constants.foldoutHeight + +Constants.border.vertical;
                    height += Constants.space;
                    foreach (var parameter in parameters)
                    {
                        parameter.GetHeight();
                        height += parameter.heightCache;
                    }
                    height += Constants.border.bottom;
                }
            }

            heightCache = height;
        }

        public bool OnGUI(Rect position, InspectorElement element, GUIContent label)
        {
            if (element.inspectorObject.editor.targets.Length > 1)
            {
                EditorGUI.HelpBox(position, "Method does not support multi-select editing",
                    UnityEditor.MessageType.None);
                return false;
            }

            var methodInfo = element.methodInfo;
            var parameters = element.methodParameters;
            var obj = element.parentTarget;

            if (label == null) label = new GUIContent(element.displayName);

            if (parameters == null)
            {
                Styles.btnStyle.fixedHeight = position.height;
                if (GUI.Button(position, label, Styles.btnStyle)) methodInfo.Invoke(obj, Constants.emptyParams);
                return false;
            }
            else
            {
                bool isButtonClick = false;
                (element.isExpanded, isButtonClick) = DoFoldoutBoxWithButton(position, element.isExpanded, label,
                    rect =>
                    {
                        rect.x += 3;
                        rect.y += Constants.space;
                        rect.width -= 2 * 3;
                        rect.height = 0;
                        foreach (var parameter in parameters)
                        {
                            rect.y += rect.height;
                            rect.height = parameter.heightCache;
                            parameter.OnGUI(rect);
                        }
                    });
                if (isButtonClick)
                {
                    methodInfo.Invoke(obj, element.GetParameterValues());
                }

                return element.isExpanded;
            }
        }
    }
}