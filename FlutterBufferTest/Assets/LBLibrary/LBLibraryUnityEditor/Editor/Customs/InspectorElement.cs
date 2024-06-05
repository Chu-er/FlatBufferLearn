using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LIBII.EasyButtons.Editor;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class InspectorElement : InspectorSorter, IDisposable
    {
        public enum EType
        {
            SerializedProperty,
            Property,
            Method,
        }

        private InspectorElement _parent;
        private object _parentTarget;
        private InspectorObject _inspectorObject;
        private InspectorGroup _inspectorGroup;

        private EType _type;
        private string _elementPath;
        private int _depth;
        private string _name;
        private string _displayName;
        private bool _enabled = true;
        private object _preValue; //Property use
        private bool _hasSetPreValueOnce; //Property use
        private bool _isExpanded;
        private ShowIfAttribute _showIf;
        private HideIfAttribute _hideIf;
        private HelpBoxAttribute _helpBox;
        private ElementHandler _guiHandler = new ElementHandler();

        private SerializedProperty _serializedProperty;
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;
        private MethodInfo _methodInfo;
        private List<DynamicObject> _methodParameters;

        private List<InspectorGroup> _subGroups = new List<InspectorGroup>();
        private List<InspectorSorter> _subSorters = new List<InspectorSorter>();
        private Dictionary<string, InspectorElement> _children = new Dictionary<string, InspectorElement>();

        private Stack<IEnumerator<InspectorElement>> _iterators = new Stack<IEnumerator<InspectorElement>>();
        private List<string> _iteratorIdentifiers = new List<string>();

        internal bool isBelongToDynamicObject = false;

        internal InspectorElement dynamicElementParent;

        public string elementPath => _elementPath;

        //public int depth => _depth;

        public InspectorObject inspectorObject => _inspectorObject;

        public InspectorGroup inspectorGroup
        {
            get => _inspectorGroup;
            set => _inspectorGroup = value;
        }

        public string name => _name;

        public string displayName => _displayName;

        public bool enabled => _enabled;

        public EType type => _type;

        public SerializedProperty serializedProperty => _serializedProperty;

        public FieldInfo fieldInfo => _fieldInfo;

        public PropertyInfo propertyInfo => _propertyInfo;

        public MethodInfo methodInfo => _methodInfo;

        public List<DynamicObject> methodParameters => _methodParameters;

        public HelpBoxAttribute helpBox => _helpBox;

        public ElementHandler guiHandler => _guiHandler;

        public List<InspectorSorter> subSorters => _subSorters;

        public object parentTarget => _parentTarget;

        public bool isExpanded
        {
            get
            {
                if (_type == EType.SerializedProperty || _type == EType.Property) return _serializedProperty.isExpanded;
                return _isExpanded;
            }
            set
            {
                if (_type == EType.SerializedProperty || _type == EType.Property)
                    _serializedProperty.isExpanded = value;
                else _isExpanded = value;
            }
        }

        public GroupAttribute.ELayout layout =>
            inspectorGroup?.layout ?? _parent?.layout ?? GroupAttribute.ELayout.Vertical;

        public bool shouldShow
        {
            get
            {
                if (_propertyInfo == null && _fieldInfo != null &&
                    _fieldInfo.GetCustomAttribute<HideInInspector>() != null) return false;
                if (_propertyInfo != null && _propertyInfo.GetCustomAttribute<HideInInspector>() != null) return false;
                if (_methodInfo != null && _methodInfo.GetCustomAttribute<HideInInspector>() != null) return false;
                if (_showIf != null) return GetResult(_showIf);
                if (_hideIf != null) return !GetResult(_hideIf);
                return true;
            }
        }

        public InspectorElement(InspectorObject inspectorObject, InspectorElement parent, object parentTarget,
            SerializedProperty property, FieldInfo fieldInfo)
        {
            _parent = parent;
            _parentTarget = parentTarget;
            _inspectorObject = inspectorObject;

            order = fieldInfo.GetCustomAttribute<ShowInInspectorAttribute>()?.order ?? 0;
            order = fieldInfo.GetCustomAttribute<OrderAttribute>()?.order ?? order;

            _type = EType.SerializedProperty;
            if (parent == null)
            {
                _elementPath = fieldInfo.Name;
                _depth = 0;
            }
            else
            {
                _elementPath = parent._elementPath + "." + fieldInfo.Name;
                _depth = parent._depth + 1;
            }
            _name = fieldInfo.Name;
            var displayNameAttribute = fieldInfo.GetCustomAttribute<DisplayNameAttribute>();
            _displayName = displayNameAttribute != null ? displayNameAttribute.displayName : property.displayName;
            _enabled = fieldInfo.GetCustomAttribute<ReadonlyAttribute>() == null;
            _isExpanded = property.isExpanded;
            _showIf = fieldInfo.GetCustomAttribute<ShowIfAttribute>();
            _hideIf = fieldInfo.GetCustomAttribute<HideIfAttribute>();
            _helpBox = fieldInfo.GetCustomAttribute<HelpBoxAttribute>();
            if (_helpBox != null)
                _helpBox.label = new GUIContent(_helpBox.text,
                    LBEditorGUIUtility.GetHelpIcon((MessageType)_helpBox.type));
            _serializedProperty = property;
            _fieldInfo = fieldInfo;
            isBelongToDynamicObject = parent?.isBelongToDynamicObject ?? false;

            HandleChildFields(fieldInfo.FieldType, fieldInfo.GetValue(parentTarget));

            HandleChildProperties(fieldInfo.FieldType, fieldInfo.GetValue(parentTarget));

            HandleChildMethods(fieldInfo.FieldType, fieldInfo.GetValue(parentTarget));

            HandleChildArrayElements(this, property);
        }

        public InspectorElement(InspectorObject inspectorObject, InspectorElement parent, object parentTarget,
            SerializedProperty property, FieldInfo fieldInfo, PropertyInfo propertyInfo)
        {
            _parent = parent;
            _parentTarget = parentTarget;
            _inspectorObject = inspectorObject;

            order = propertyInfo.GetCustomAttribute<ShowInInspectorAttribute>().order;
            order = propertyInfo.GetCustomAttribute<OrderAttribute>()?.order ?? order;

            _type = EType.Property;
            if (parent == null)
            {
                _elementPath = propertyInfo.Name;
                _depth = 0;
            }
            else
            {
                _elementPath = parent._elementPath + "." + propertyInfo.Name;
                _depth = parent._depth + 1;
            }
            _name = propertyInfo.Name;
            var displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            _displayName = displayNameAttribute != null
                ? displayNameAttribute.displayName
                : propertyInfo.Name.GetMangledName();
            _enabled = propertyInfo.GetSetMethod() != null &&
                       propertyInfo.GetCustomAttribute<ReadonlyAttribute>() == null;
            _isExpanded = property.isExpanded;
            _showIf = propertyInfo.GetCustomAttribute<ShowIfAttribute>();
            _hideIf = propertyInfo.GetCustomAttribute<HideIfAttribute>();
            _helpBox = propertyInfo.GetCustomAttribute<HelpBoxAttribute>();
            if (_helpBox != null)
                _helpBox.label = new GUIContent(_helpBox.text,
                    LBEditorGUIUtility.GetHelpIcon((MessageType)_helpBox.type));
            _serializedProperty = property;
            _propertyInfo = propertyInfo;
            _fieldInfo = fieldInfo;
            isBelongToDynamicObject = parent?.isBelongToDynamicObject ?? false;

            HandleChildFields(propertyInfo.PropertyType, fieldInfo.GetValue(parentTarget));

            HandleChildProperties(propertyInfo.PropertyType, fieldInfo.GetValue(parentTarget));

            HandleChildMethods(propertyInfo.PropertyType, fieldInfo.GetValue(parentTarget));

            HandleChildArrayElements(this, property);
        }

        public InspectorElement(InspectorObject inspectorObject, InspectorElement parent, object parentTarget,
            MethodInfo methodInfo)
        {
            var buttonAttribute = methodInfo.GetCustomAttribute<ButtonAttribute>();
            _displayName = string.IsNullOrEmpty(buttonAttribute.label)
                ? methodInfo.Name.GetMangledName()
                : buttonAttribute.label;

            _parent = parent;
            _parentTarget = parentTarget;
            _inspectorObject = inspectorObject;

            order = buttonAttribute.order;
            order = methodInfo.GetCustomAttribute<OrderAttribute>()?.order ?? order;

            _type = EType.Method;
            if (parent == null)
            {
                _elementPath = LBEditorGUIUtility.GetMethodName(methodInfo);
                _depth = 0;
            }
            else
            {
                _elementPath = parent._elementPath + "." + LBEditorGUIUtility.GetMethodName(methodInfo);
                _depth = parent._depth + 1;
            }
            _enabled = methodInfo.GetCustomAttribute<ReadonlyAttribute>() == null;
            if (Application.isPlaying && buttonAttribute.executeType == ButtonAttribute.EExecuteType.Editor
                || !Application.isPlaying && buttonAttribute.executeType == ButtonAttribute.EExecuteType.Runtime)
            {
                _enabled = false;
            }
            _isExpanded = false;
            _showIf = methodInfo.GetCustomAttribute<ShowIfAttribute>();
            _hideIf = methodInfo.GetCustomAttribute<HideIfAttribute>();
            _helpBox = methodInfo.GetCustomAttribute<HelpBoxAttribute>();
            if (_helpBox != null)
                _helpBox.label = new GUIContent(_helpBox.text,
                    LBEditorGUIUtility.GetHelpIcon((MessageType)_helpBox.type));
            _methodInfo = methodInfo;
            if (methodInfo.GetParameters().Length > 0)
            {
                _methodParameters = new List<DynamicObject>();
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    var dynamicObject = new DynamicObject(parameterInfo);
                    foreach (var sorter in dynamicObject.editor.inspectorObject.sorters)
                    {
                        if (sorter is InspectorElement element)
                        {
                            element.isBelongToDynamicObject = true;
                            element.dynamicElementParent = this;
                        }
                    }
                    _methodParameters.Add(dynamicObject);
                }
            }
            isBelongToDynamicObject = parent?.isBelongToDynamicObject ?? false;
            if (parent != null && isBelongToDynamicObject) dynamicElementParent = parent.dynamicElementParent;
        }

        private InspectorElement(InspectorObject inspectorObject, InspectorElement parent, object arrayTarget,
            object arrayElementTarget, int arrayElementIndex, Type arrayElementType, SerializedProperty property)
        {
            _parent = parent;
            _parentTarget = arrayTarget;
            _inspectorObject = inspectorObject;

            _type = EType.SerializedProperty;
            _elementPath = parent._elementPath + "." + $"Array.data[{arrayElementIndex}]";
            _depth = parent._depth + 1;
            _name = property.name;
            _displayName = property.displayName;
            _enabled = parent._enabled;
            _isExpanded = property.isExpanded;
            _serializedProperty = property;

            HandleChildFields(arrayElementType, arrayElementTarget);

            HandleChildProperties(arrayElementType, arrayElementTarget);

            HandleChildMethods(arrayElementType, arrayElementTarget);

            HandleChildArrayElements(this, property);
        }

        ~InspectorElement()
        {
            Dispose();
        }

        public void Dispose()
        {
        }

        public InspectorElement FindElementRelative(string path)
        {
            path = $"{elementPath}.{path}";
            return _children.ContainsKey(path) ? _children[path] : default;
        }

        public InspectorElement FindArrayElementAtIndex(int index)
        {
            if (serializedProperty == null || !serializedProperty.IsNonStringArray()) return default;
            var path = $"{elementPath}.Array.data[{index}]";
            return _children.ContainsKey(path) ? _children[path] : default;
        }

        private void HandleChildFields(Type reflectType, object reflectTarget)
        {
            foreach (var childFieldInfo in reflectType.GetFields(LBEditorGUI.elementBindingFlags)
                         .Where
                         (x => (x.IsPublic && x.GetCustomAttribute<NonSerializedAttribute>() == null ||
                                !x.IsPublic && x.GetCustomAttribute<SerializeField>() != null)
                         )
                    )
            {
                string childElementPath = serializedProperty.propertyPath + "." + childFieldInfo.Name;
                var childProperty = _inspectorObject.editor.serializedObject.FindProperty(childElementPath);
                if (childProperty != null)
                {
                    var childElement = new InspectorElement(_inspectorObject, this, reflectTarget, childProperty,
                        childFieldInfo);

                    var decoratorAttributes = childFieldInfo.GetCustomAttributes<DecoratorAttribute>();
                    foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                                 x.orientation == DecoratorAttribute.EOrientation.Up))
                        _subSorters.Add(new InspectorDecorator(decoratorAttribute, childElement.order));

                    _children.Add(childElement.elementPath, childElement);

                    InspectorGroup.JoinSortersWithCheckGroup(childElement, ref _subGroups, ref _subSorters);
                }
            }
        }

        private void HandleChildProperties(Type reflectType, object reflectTarget)
        {
            foreach (var childPropertyInfo in reflectType.GetProperties(LBEditorGUI.elementBindingFlags)
                         .Where(x => LBEditorGUI.ValidProperties(x, _children.Values)))
            {
                string fieldName = childPropertyInfo.GetCustomAttribute<ShowInInspectorAttribute>().fieldName;
                var childFieldElement = _children.Values.First(x =>
                    x.serializedProperty != null && x.serializedProperty.name == fieldName);
                var childProperty = childFieldElement.serializedProperty;
                var childElement = new InspectorElement(_inspectorObject, this, reflectTarget, childProperty,
                    childFieldElement.fieldInfo, childPropertyInfo);

                var decoratorAttributes = childPropertyInfo.GetCustomAttributes<DecoratorAttribute>();
                foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                             x.orientation == DecoratorAttribute.EOrientation.Up))
                    _subSorters.Add(new InspectorDecorator(decoratorAttribute, childElement.order));

                _children.Add(childElement.elementPath, childElement);

                InspectorGroup.JoinSortersWithCheckGroup(childElement, ref _subGroups, ref _subSorters);
            }
        }

        private void HandleChildMethods(Type reflectType, object reflectTarget)
        {
            foreach (var childMethodInfo in reflectType.GetMethods(LBEditorGUI.elementBindingFlags)
                         .Where(x => x.GetCustomAttribute<ButtonAttribute>() != null))
            {
                var childElement = new InspectorElement(_inspectorObject, this, reflectTarget, childMethodInfo);

                var decoratorAttributes = childMethodInfo.GetCustomAttributes<DecoratorAttribute>();
                foreach (var decoratorAttribute in decoratorAttributes.Where(x =>
                             x.orientation == DecoratorAttribute.EOrientation.Up))
                    _subSorters.Add(new InspectorDecorator(decoratorAttribute, childElement.order));

                _children.Add(childElement.elementPath, childElement);

                InspectorGroup.JoinSortersWithCheckGroup(childElement, ref _subGroups, ref _subSorters);
            }
        }

        private void HandleChildArrayElements(InspectorElement element, SerializedProperty array)
        {
            if (!array.IsNonStringArray()) return;

            for (int i = 0; i < array.arraySize; i++)
            {
                element.AddArrayElementIfNotExist(array, i);
            }
        }

        /*private InspectorElement _preChild;

        public bool NextVisible(bool enterChildren, out InspectorElement child)
        {
            child = null;

            if (_iterators.Count == 0 && !_iteratorIdentifiers.Contains(_elementPath))
            {
                _iterators.Push(_children.Values.Where(x => x.shouldShow).GetEnumerator());
                _iteratorIdentifiers.Add(_elementPath);
            }
            else if (enterChildren && _preChild != null && !_iteratorIdentifiers.Contains(_preChild._elementPath))
            {
                _iterators.Push(_preChild._children.Values.Where(x => x.shouldShow).GetEnumerator());
                _iteratorIdentifiers.Add(_preChild._elementPath);
            }

            var iterator = _iterators.Peek();
            if (iterator.MoveNext())
            {
                _preChild = child = iterator.Current;
                return true;
            }
            else
            {
                iterator = _iterators.Pop();
                //Debug.Log("Remove iterator " + _iterators.Count);
                iterator.Dispose();
                iterator = null;
                if (_iterators.Count > 0)
                {
                    return NextVisible(enterChildren, out child);
                }

                _iteratorIdentifiers.Clear();
                return false;
            }
        }*/

        private string GetArrayElementElementPath(int index) => $"{_elementPath}.Array.data[{index}]";

        public InspectorElement GetArrayElement(int index)
        {
            InspectorElement inspectorElement = default;
            try
            {
                inspectorElement = _children[GetArrayElementElementPath(index)];
            }
            catch (Exception e)
            {
                Debug.LogError(elementPath + " " + e + ": " + index);
                throw;
            }
            return inspectorElement;
        }

        public void AddArrayElementIfNotExist(SerializedProperty array, int index)
        {
            var childSerializedProperty = array.GetArrayElementAtIndex(index);
            var childElementPath = GetArrayElementElementPath(index);
            if (childSerializedProperty != null && !_children.ContainsKey(childElementPath))
            {
                Type elementType = null;
                if (fieldInfo.FieldType.IsArray)
                {
                    elementType = fieldInfo.FieldType.GetElementType();
                }
                else if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    elementType = fieldInfo.FieldType.GetGenericArguments()[0];
                }
                var arrayTarget = (IList)_fieldInfo.GetValue(_parentTarget);
                var childElement = new InspectorElement(_inspectorObject, this, arrayTarget,
                    arrayTarget[index], index, elementType, childSerializedProperty);
                _children.Add(childElementPath, childElement);
            }
        }

        public void RemoveAnyArrayElement(SerializedProperty array)
        {
            var prefix = $"{(_parent != null ? $"{_parent._elementPath}." : "")}Array.data[";
            var abandonedKeys = new List<string>();
            foreach (var key in _children.Keys)
            {
                if (key.Contains(prefix)) abandonedKeys.Add(key);
            }
            foreach (var abandonedKey in abandonedKeys) _children.Remove(abandonedKey);
            for (int i = 0; i < array.arraySize; i++)
            {
                var childSerializedProperty = array.GetArrayElementAtIndex(i);
                if (childSerializedProperty != null)
                {
                    var childElementPath = GetArrayElementElementPath(i);
                    Type elementType = null;
                    if (fieldInfo.FieldType.IsArray)
                    {
                        elementType = fieldInfo.FieldType.GetElementType();
                    }
                    else if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        elementType = fieldInfo.FieldType.GetGenericArguments()[0];
                    }
                    var arrayTarget = (IList)_fieldInfo.GetValue(_parentTarget);
                    var childElement = new InspectorElement(_inspectorObject, this, arrayTarget,
                        arrayTarget[i], i, elementType, childSerializedProperty);
                    _children.Add(childElementPath, childElement);
                }
            }
        }

        public object[] GetParameterValues()
        {
            object[] paramValues = new object[_methodParameters.Count];
            for (int i = 0; i < _methodParameters.Count; i++)
            {
                var parameter = _methodParameters[i];
                paramValues[i] = parameter.GetValue();
            }

            return paramValues;
        }

        public bool GetResult(ConditionAttribute attribute)
        {
            bool result = true;
            if (string.IsNullOrEmpty(attribute.condition)) return result;

            var target = _parentTarget;
            var targetType = fieldInfo != null ? fieldInfo.DeclaringType :
                propertyInfo != null ? propertyInfo.DeclaringType :
                methodInfo != null ? methodInfo.DeclaringType : null;
            var reflectFlags = LBEditorGUI.elementBindingFlags | BindingFlags.Static;
            if (targetType != null && !string.IsNullOrEmpty(attribute.condition))
            {
                var conditionFieldInfo = targetType.GetField(attribute.condition, reflectFlags);
                if (conditionFieldInfo != null)
                {
                    if (conditionFieldInfo.FieldType == typeof(bool))
                    {
                        result = (bool)conditionFieldInfo.GetValue(target);
                    }
                    else
                    {
                        var value = Convert.ChangeType(conditionFieldInfo.GetValue(target),
                            conditionFieldInfo.FieldType);
                        if (value == null)
                        {
                            result = attribute.conditionValue == null;
                        }
                        else
                        {
                            result = value.Equals(attribute.conditionValue);
                        }
                    }
                }
                else
                {
                    var conditionPropertyInfo = targetType.GetProperty(attribute.condition, reflectFlags);
                    if (conditionPropertyInfo != null)
                    {
                        if (conditionPropertyInfo.PropertyType == typeof(bool))
                        {
                            result = (bool)conditionPropertyInfo.GetValue(target);
                        }
                        else
                        {
                            var value = Convert.ChangeType(conditionPropertyInfo.GetValue(target),
                                conditionPropertyInfo.PropertyType);
                            if (value == null)
                            {
                                result = attribute.conditionValue == null;
                            }
                            else
                            {
                                result = value.Equals(attribute.conditionValue);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}