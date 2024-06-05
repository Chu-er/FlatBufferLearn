using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class LBInspector : Editor
    {
        private InspectorObject _inspectorObject;

        public InspectorObject InspectorObject => _inspectorObject;

        protected virtual void OnEnable()
        {
            _inspectorObject = new InspectorObject(this);
        }

        protected virtual void OnDisable()
        {
            _inspectorObject.Dispose();
            _inspectorObject = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            {
                var scriptProperty = serializedObject.FindProperty("m_Script");
                EditorGUILayout.PropertyField(scriptProperty);
            }
            EditorGUI.EndDisabledGroup();

            /*DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            return;*/

            _inspectorObject.guiHandler.OnGUILayout(_inspectorObject);

            serializedObject.ApplyModifiedProperties();
        }
    }
}