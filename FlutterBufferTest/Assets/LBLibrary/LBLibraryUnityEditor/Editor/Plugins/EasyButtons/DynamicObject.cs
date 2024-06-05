using System;
using System.Reflection;
using LIBII.EasyButtons.Editor.Utils;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace LIBII.EasyButtons.Editor
{
    public class DynamicObject : IDisposable
    {
        public ScriptableObject scriptableObject;
        public DynamicObjectEditor editor;
        public FieldInfo dynamicFieldInfo;
        public float heightCache;

        public DynamicObject(ParameterInfo paramInfo)
        {
            var generatedType = ScriptableObjectCache.GetClass(paramInfo.Name, paramInfo.ParameterType);
            scriptableObject = ScriptableObject.CreateInstance(generatedType);
            dynamicFieldInfo = generatedType.GetField(paramInfo.Name);
            editor = (DynamicObjectEditor)UnityEditor.Editor.CreateEditor(scriptableObject,
                typeof(DynamicObjectEditor));
            editor.Initialize();
            if (paramInfo.HasDefaultValue)
            {
                dynamicFieldInfo.SetValue(editor.target, paramInfo.DefaultValue);
                editor.serializedObject.Update();
            }
        }

        ~DynamicObject()
        {
            Dispose();
        }

        public object GetValue()
        {
            editor.ApplyModifiedProperties();
            return dynamicFieldInfo.GetValue(scriptableObject);
        }

        public void GetHeight()
        {
            editor.GetHeight();
            heightCache = editor.heightCache;
        }

        public bool OnGUI(Rect position)
        {
            editor.SetOnGUIParams(position).OnInspectorGUI();
            return editor.onGUIResult;
        }

        public void Dispose()
        {
            dynamicFieldInfo = null;
            UObject.DestroyImmediate(editor);
            editor = null;
            UObject.DestroyImmediate(scriptableObject);
            scriptableObject = null;
        }
    }
}