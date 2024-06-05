using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Assertions;

namespace LIBII.EasyButtons.Editor.Utils
{
    internal static class ScriptableObjectCache
    {
        private const string AssemblyName = "EasyButtons.DynamicAssembly";

        private static readonly AssemblyBuilder _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
            new AssemblyName(AssemblyName)
        {
            CultureInfo = CultureInfo.InvariantCulture,
            Flags = AssemblyNameFlags.None,
            ProcessorArchitecture = ProcessorArchitecture.MSIL,
            VersionCompatibility = AssemblyVersionCompatibility.SameDomain
        }, AssemblyBuilderAccess.Run);

        private static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName, true);

        private static readonly Dictionary<string, Type> _classDict = new Dictionary<string, Type>();

        public static Type GetClass(string fieldName, Type fieldType)
        {
            string className = GetClassName(fieldName, fieldType);

            if (_classDict.TryGetValue(className, out Type classType))
                return classType;

            if ( ! fieldType.IsUnitySerializable())
            {
                fieldType = typeof(NonSerializedError);
            }

            classType = CreateClass(className, fieldName, fieldType);
            _classDict[className] = classType;
            return classType;
        }

        private static Type CreateClass(string className, string fieldName, Type fieldType)
        {
            TypeBuilder typeBuilder = _moduleBuilder.DefineType(
                $"{AssemblyName}.{className}",
                TypeAttributes.NotPublic,
                typeof(ScriptableObject));

            typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);
            Type type = typeBuilder.CreateType();
            return type;
        }

        private static string GetClassName(string fieldName, Type fieldType)
        {
            string fullTypeName = fieldType.FullName;

            Assert.IsNotNull(fullTypeName);

            string classSafeTypeName = fullTypeName
                .Replace('.', '_')
                .Replace('`', '_');

            return $"{classSafeTypeName}_{fieldName}".CapitalizeFirstChar();
        }
        
        #region Dictionary Extends
        
        /*private static (Type, string) GetClass(string className, string fieldName, Type fieldType,
            params CustomAttributeBuilder[] attributeBuilders)
        {
            if (_classDict.TryGetValue(className, out Type classType))
                return (classType, fieldName);

            if (!fieldType.IsUnitySerializable())
            {
                throw new Exception("This type is not serializable " + fieldType.FullName);
            }

            classType = CreateClass(className, fieldName, fieldType, attributeBuilders);
            _classDict[className] = classType;
            return (classType, fieldName);
        }

        private static Type CreateClass(string className, string fieldName, Type fieldType,
            params CustomAttributeBuilder[] attributeBuilders)
        {
            var typeBuilder = _moduleBuilder.DefineType(
                $"{AssemblyName}.{className}",
                TypeAttributes.NotPublic,
                typeof(ScriptableObject));

            var fieldBuilder = typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);
            foreach (var attributeBuilder in attributeBuilders)
                if (attributeBuilder != null)
                    fieldBuilder.SetCustomAttribute(attributeBuilder);

            Type type = typeBuilder.CreateType();
            return type;
        }
        
        public static (Type, string) GetDictionaryFieldClass(this FieldInfo fieldInfo,
            params CustomAttributeBuilder[] attributeBuilders)
        {
            string fieldName = fieldInfo.Name;
            Type[] genericParams = fieldInfo.FieldType.GetGenericArguments();
            Type listFieldType = typeof(List<>);
            Type elementType = 
            return GetClass(fieldInfo.GetClassName(fieldName, fieldType), fieldName, fieldType, attributeBuilders);
        }
        
        private static string GetClassName(this FieldInfo field, string fieldName, Type fieldType)
        {
            return $"Field_{GetClassName(fieldName, fieldType)}";
        }*/
        
        #endregion
    }
}