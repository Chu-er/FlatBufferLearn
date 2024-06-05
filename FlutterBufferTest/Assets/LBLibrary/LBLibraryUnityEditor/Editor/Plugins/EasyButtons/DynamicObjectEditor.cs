using LIBII.CustomEditor;
using UnityEditor;
using UnityEngine;

namespace LIBII.EasyButtons.Editor
{
    public class DynamicObjectEditor : UnityEditor.Editor
    {
        public InspectorObject inspectorObject;
        
        public bool onGUIResult;

        public float heightCache;

        private Rect _position;

        public void Initialize()
        {
            inspectorObject = new InspectorObject(this);
        }

        public void GetHeight()
        {
            heightCache = inspectorObject.guiHandler.GetHeight(inspectorObject);
        }

        public DynamicObjectEditor SetOnGUIParams(Rect position)
        {
            _position = position;
            return this;
        }

        public override void OnInspectorGUI()
        {
            inspectorObject.guiHandler.OnGUI(_position, inspectorObject);
        }

        public bool ApplyModifiedProperties()
        {
            if (!serializedObject.hasModifiedProperties) return false;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            return true;
        }
    }
}