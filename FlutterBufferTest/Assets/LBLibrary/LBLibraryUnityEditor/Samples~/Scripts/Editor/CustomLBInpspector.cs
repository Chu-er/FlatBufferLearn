using System.Reflection;
using LIBII.CustomEditor.Samples.Runtime;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor.Samples.Editor
{
    [UnityEditor.CustomEditor(typeof(CustomLBInspectorTest))]
    public class CustomLBInspector : LBInspector
    {
        public InspectorElement testInt;
        public InspectorElement testString;
        public InspectorElement serializedClassSamples;
        public InspectorElement testMethod;
        public InspectorElement testGameObjects;

        protected override void OnEnable()
        {
            base.OnEnable();

            testInt = InspectorObject.FindElement("testInt");
            testString = InspectorObject.FindElement("TestString");
            serializedClassSamples = InspectorObject.FindElement("serializedClassSamples");
            testMethod = InspectorObject.FindElement(LBEditorGUIUtility.GetMethodName(target.GetType()
                .GetMethod("TestMethod", BindingFlags.Public | BindingFlags.Instance)));
            serializedClassSamples.FindArrayElementAtIndex(0)
                .FindElementRelative("innerSerializedClassListField").guiHandler.ReorderableList.onAddCallback += list => 
            {
                Debug.Log("innerSerializedClassListField 添加了东西");
            };
            testGameObjects = InspectorObject.FindElement("testGameObjects");
            testGameObjects.guiHandler.ReorderableList.onAddCallback += list => 
            {
                Debug.Log("innerSerializedClassListField 添加了东西");
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            /*DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            return;*/

            LBEditorGUILayout.ElementField(testInt);
            LBEditorGUILayout.ElementField(testString);
            LBEditorGUILayout.ElementField(serializedClassSamples);
            LBEditorGUILayout.ElementField(testMethod);
            LBEditorGUILayout.ElementField(testGameObjects);

            serializedObject.ApplyModifiedProperties();
        }
    }
}