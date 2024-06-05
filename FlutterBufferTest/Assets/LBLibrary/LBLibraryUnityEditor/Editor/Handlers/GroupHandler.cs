using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class GroupHandler
    {
        public float heightCache;

        public void GetHeight(InspectorGroup group)
        {
            float height = 0;

            if (group.style == InspectorGroup.EStyle.Box && ((BoxAttribute)group.attribute).collapsible)
            {
                height += Constants.foldoutHeight + Constants.border.vertical;
            }

            if (group.isFoldout && group.shouldShow)
            {
                if (group.style == InspectorGroup.EStyle.Box) height += Constants.space;

                if (group.layout == GroupAttribute.ELayout.Vertical)
                {
                    foreach (var sorter in group.sorters)
                    {
                        if (sorter is InspectorElement element && element.shouldShow)
                        {
                            height += Constants.space;
                            element.guiHandler.GetHeight(element, new GUIContent(element.displayName),
                                LBEditorGUI.IsChildrenIncluded(element));
                            height += element.guiHandler.heightCache;
                        }
                        else if (sorter is InspectorGroup subGroup && subGroup.shouldShow)
                        {
                            height += Constants.space;
                            subGroup.layoutHandler.GetHeight(subGroup);
                            height += subGroup.layoutHandler.heightCache;
                        }
                    }

                    if (group.style == InspectorGroup.EStyle.Layout) height -= Constants.space;
                }
                else if (group.layout == GroupAttribute.ELayout.Horizontal)
                {
                    var maxHeight = 0f;
                    foreach (var sorter in group.sorters)
                    {
                        if (sorter is InspectorElement element && element.shouldShow)
                        {
                            element.guiHandler.GetHeight(element, new GUIContent(element.displayName),
                                LBEditorGUI.IsChildrenIncluded(element));
                            maxHeight = Mathf.Max(maxHeight, element.guiHandler.heightCache);
                        }
                        else if (sorter is InspectorGroup subGroup && subGroup.shouldShow)
                        {
                            subGroup.layoutHandler.GetHeight(subGroup);
                            maxHeight = Mathf.Max(maxHeight, subGroup.layoutHandler.heightCache);
                        }
                    }

                    height += maxHeight;
                    if (group.style == InspectorGroup.EStyle.Box) height += Constants.space;
                }

                if (group.style == InspectorGroup.EStyle.Box) height += Constants.border.bottom;
            }

            heightCache = height;
        }

        public void OnGUI(Rect position, InspectorGroup group)
        {
            float indent = EditorGUI.indentLevel * Constants.indentPerLevel;

            if (group.style == InspectorGroup.EStyle.Box && ((BoxAttribute)group.attribute).collapsible)
            {
                var headerRect = new Rect(position) { height = Constants.foldoutHeight };
                headerRect.x += indent;
                headerRect.width -= indent;
                ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);
                headerRect.x += Constants.FoldoutIndent + Constants.border.left;
                headerRect.y += Constants.border.top;
                headerRect.width -= Constants.FoldoutIndent + ReorderableList.Defaults.padding;
                group.isFoldout = EditorGUI.BeginFoldoutHeaderGroup(headerRect, group.isFoldout,
                    new GUIContent(group.name), Constants.foldoutHeaderStyle);
                EditorGUI.EndFoldoutHeaderGroup();
            }

            if (group.isFoldout && group.shouldShow)
            {
                if (group.style == InspectorGroup.EStyle.Box)
                {
                    var headerHeight = ((BoxAttribute)group.attribute).collapsible
                        ? Constants.foldoutHeight + Constants.border.vertical
                        : 0;
                    position.y += headerHeight;
                    var listRect = new Rect(position) { height = position.height - headerHeight };
                    listRect.x += indent;
                    listRect.width -= indent;
                    if (Event.current != null && Event.current.type == EventType.Repaint)
                        Constants.boxStyle.Draw(listRect, false, false, false, false);

                    position.x += 3;
                    position.width -= 2 * 3;
                    position.y += Constants.space;
                }

                var visibleSorters = group.visibleSorters;

                if (group.layout == GroupAttribute.ELayout.Vertical)
                {
                    if (group.style != InspectorGroup.EStyle.Box)
                    {
                        position.x += indent;
                        position.width -= indent;
                    }

                    for (int i = 0; i < visibleSorters.Count; i++)
                    {
                        var sorter = visibleSorters[i];
                        if (sorter is InspectorElement element)
                        {
                            if (LBEditorGUI.HasVisibleChildFields(element) &&
                                group.style == InspectorGroup.EStyle.Box)
                            {
                                position.x += Constants.foldoutIconWidth;
                                position.width -= Constants.foldoutIconWidth;
                            }

                            position.height = element.guiHandler.heightCache;
                            element.guiHandler.OnGUI(position, element, new GUIContent(element.displayName),
                                LBEditorGUI.IsChildrenIncluded(element));

                            if (LBEditorGUI.HasVisibleChildFields(element) &&
                                group.style == InspectorGroup.EStyle.Box)
                            {
                                position.x -= Constants.foldoutIconWidth;
                                position.width += Constants.foldoutIconWidth;
                            }
                            position.y += position.height;
                            position.y += Constants.space;
                        }
                        else if (sorter is InspectorGroup subGroup)
                        {
                            position.height = subGroup.layoutHandler.heightCache;
                            subGroup.layoutHandler.OnGUI(position, subGroup);
                            position.y += position.height;
                            position.y += Constants.space;
                        }
                    }
                }
                else if (group.layout == GroupAttribute.ELayout.Horizontal)
                {
                    if (group.style != InspectorGroup.EStyle.Box) position.x += indent;

                    position.width -= indent;
                    int visibleCount = visibleSorters.Count;
                    float totalWidth = position.width - Constants.space * (visibleCount - 1);
                    position.width = totalWidth / visibleCount;
                    float boxMultipleWidth = (totalWidth - Constants.foldoutIconWidth * visibleCount) /
                                             group.sorters.Count;
                    float multipleWidth = (totalWidth - Constants.foldoutIconWidth * (visibleCount - 1)) /
                                          visibleCount;

                    for (int i = 0; i < visibleSorters.Count; i++)
                    {
                        var sorter = visibleSorters[i];
                        if (sorter is InspectorElement element)
                        {
                            if (LBEditorGUI.HasVisibleChildFields(element))
                            {
                                if (group.style == InspectorGroup.EStyle.Box)
                                {
                                    position.x += Constants.foldoutIconWidth;
                                    position.width = boxMultipleWidth;
                                }
                                else
                                {
                                    if (group.sorters[0] != sorter) position.x += Constants.foldoutIconWidth;
                                    position.width = multipleWidth;
                                }
                            }

                            position.height = element.guiHandler.heightCache;
                            element.guiHandler.OnGUI(position, element, new GUIContent(element.displayName),
                                LBEditorGUI.IsChildrenIncluded(element));
                            position.x += position.width;
                        }
                        else if (sorter is InspectorGroup subGroup)
                        {
                            position.height = subGroup.layoutHandler.heightCache;
                            subGroup.layoutHandler.OnGUI(position, subGroup);
                            position.x += position.width;
                        }

                        position.x += Constants.space;
                    }
                }
            }
        }
    }
}