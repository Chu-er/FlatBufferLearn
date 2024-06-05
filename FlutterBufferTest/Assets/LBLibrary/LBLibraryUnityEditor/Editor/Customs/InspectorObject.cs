using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace LIBII.CustomEditor
{
    public class InspectorObject : IDisposable
    {
        public readonly Editor editor;

        public List<InspectorGroup> groups = new List<InspectorGroup>();
        public List<InspectorSorter> sorters = new List<InspectorSorter>();
        public Dictionary<string, InspectorElement> children = new Dictionary<string, InspectorElement>();

        public ObjectHandler guiHandler = new ObjectHandler();

        public InspectorObject(Editor editor)
        {
            this.editor = editor;

            Initialize();
        }

        public void Dispose()
        {
        }

        ~InspectorObject()
        {
            Dispose();
        }

        public InspectorElement FindElement(string path)
        {
            return children[path];
        }

        private void Initialize()
        {
            var reflectType = editor.target.GetType();

            HandleFields(reflectType);

            HandleProperties(reflectType);

            HandleMethods(reflectType);

            foreach (var group in groups) group.Sort();

            if (sorters.Count > 1) sorters = sorters.OrderBy(x => x.order).ToList();
        }

        private void HandleFields(Type reflectType)
        {
            var fieldInfos = new List<FieldInfo>();
            var searchTypes = new List<Type>() { reflectType };
            var baseType = reflectType.BaseType;
            while (baseType != null && baseType.FullName != "UnityEngine.MonoBehaviour" &&
                   baseType.FullName != "UnityEngine.ScriptableObject")
            {
                searchTypes.Add(baseType);
                baseType = baseType.BaseType;
            }
            if (searchTypes.Count > 0)
            {
                for (int i = searchTypes.Count - 1; i >= 0; i--)
                {
                    var searchType = searchTypes[i];
                    var serializedFieldInfos = searchType.GetSerializedFieldInfos();
                    if (serializedFieldInfos != null)
                    {
                        foreach (var serializedFieldInfo in serializedFieldInfos)
                        {
                            if (!fieldInfos.Exists(x => x.Name == serializedFieldInfo.Name))
                            {
                                fieldInfos.Add(serializedFieldInfo);
                            }
                        }
                    }
                }
            }

            foreach (var fieldInfo in fieldInfos)
            {
                string elementPath = fieldInfo.Name;
                var property = editor.serializedObject.FindProperty(elementPath);
                if (property != null)
                {
                    var element = new InspectorElement(this, null, editor.target, property, fieldInfo);

                    var decoratorAttributes = fieldInfo.GetCustomAttributes<DecoratorAttribute>();
                    foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                                 x.orientation == DecoratorAttribute.EOrientation.Up))
                        sorters.Add(new InspectorDecorator(decoratorAttribute, element.order));

                    children.Add(element.elementPath, element);

                    InspectorGroup.JoinSortersWithCheckGroup(element, ref groups, ref sorters);
                }
            }
        }

        private void HandleProperties(Type reflectType)
        {
            foreach (var propertyInfo in reflectType.GetProperties(LBEditorGUI.elementBindingFlags)
                         .Where(x => LBEditorGUI.ValidProperties(x, children.Values)))
            {
                var fieldName = propertyInfo.GetCustomAttribute<ShowInInspectorAttribute>().fieldName;
                var fieldElement = children.Values.First(x =>
                    x.serializedProperty != null && x.serializedProperty.name == fieldName);
                var element = new InspectorElement(this, null, editor.target, fieldElement.serializedProperty,
                    fieldElement.fieldInfo, propertyInfo);

                var decoratorAttributes = propertyInfo.GetCustomAttributes<DecoratorAttribute>();
                foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                             x.orientation == DecoratorAttribute.EOrientation.Up))
                {
                    sorters.Add(new InspectorDecorator(decoratorAttribute, element.order));
                }

                children.Add(element.elementPath, element);
                InspectorGroup.JoinSortersWithCheckGroup(element, ref groups, ref sorters);
            }
        }

        private void HandleMethods(Type reflectType)
        {
            foreach (var methodInfo in reflectType.GetMethods(LBEditorGUI.elementBindingFlags)
                         .Where(x => x.GetCustomAttribute<ButtonAttribute>() != null))
            {
                var element = new InspectorElement(this, null, editor.target, methodInfo);

                var decoratorAttributes = methodInfo.GetCustomAttributes<DecoratorAttribute>();
                foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                             x.orientation == DecoratorAttribute.EOrientation.Up))
                {
                    sorters.Add(new InspectorDecorator(decoratorAttribute, element.order));
                }

                children.Add(element.elementPath, element);
                InspectorGroup.JoinSortersWithCheckGroup(element, ref groups, ref sorters);
            }
        }
    }
}