using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class ObjectHandler
    {
        public float GetHeight(InspectorObject obj)
        {
            float heightCache = 0;
            foreach (var inspectorSorter in obj.sorters)
            {
                if (inspectorSorter is InspectorElement element && element.shouldShow)
                {
                    element.guiHandler.GetHeight(element, new GUIContent(element.displayName),
                        LBEditorGUI.IsChildrenIncluded(element));
                    heightCache += element.guiHandler.heightCache;
                    heightCache += Constants.space;
                }
                else if (inspectorSorter is InspectorGroup group && group.shouldShow)
                {
                    group.layoutHandler.GetHeight(group);
                    heightCache += group.layoutHandler.heightCache;
                    heightCache += Constants.space;
                }
                else if (inspectorSorter is InspectorDecorator decorator)
                {
                    heightCache += decorator.decorateHandler.GetHeight();
                }
            }

            return heightCache;
        }

        public void OnGUILayout(InspectorObject obj)
        {
            OnGUI(GUILayoutUtility.GetRect(0, obj.guiHandler.GetHeight(obj)), obj);
        }

        public void OnGUI(Rect position, InspectorObject obj)
        {
            foreach (var inspectorSorter in obj.sorters)
            {
                if (inspectorSorter is InspectorElement element && element.shouldShow)
                {
                    position.height = element.guiHandler.heightCache;
                    element.guiHandler.OnGUI(position, element, new GUIContent(element.displayName),
                        LBEditorGUI.IsChildrenIncluded(element));
                    position.y += position.height;
                    position.y += Constants.space;
                }
                else if (inspectorSorter is InspectorGroup group && group.shouldShow)
                {
                    position.height = group.layoutHandler.heightCache;
                    group.layoutHandler.OnGUI(position, group);
                    position.y += position.height;
                    position.y += Constants.space;
                }
                else if (inspectorSorter is InspectorDecorator decorator)
                {
                    position.height = decorator.decorateHandler.GetHeight();
                    decorator.decorateHandler.OnGUI(position);
                    position.y += position.height;
                }
            }
        }
    }
}