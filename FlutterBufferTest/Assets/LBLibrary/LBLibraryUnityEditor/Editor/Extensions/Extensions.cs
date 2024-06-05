using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    internal static class Extensions
    {
        public static bool IsArrayOrList(this Type listType)
        {
            if (listType.IsArray)
            {
                return true;
            }
            else if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return true;
            }

            return false;
        }

        public static bool IsNonStringArray(this SerializedProperty property)
        {
            if (property == null) return false;
            // Strings should not be represented with ReorderableList, they will use custom drawer therefore we don't treat them as other arrays
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }

        public static IEnumerable<FieldInfo> GetSerializedFieldInfos(this Type type)
        {
            if (!type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsSubclassOf(typeof(ScriptableObject))) return null;

            return type.GetFields(LBEditorGUI.elementBindingFlags).Where(x =>
                x.IsPublic && x.GetCustomAttribute<NonSerializedAttribute>() == null ||
                !x.IsPublic && x.GetCustomAttribute<SerializeField>() != null);
        }

        public static string GetMangledName(this string name)
        {
            var strb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var curChar = name[i];
                if (curChar >= 'A' && curChar <= 'Z' && i > 0)
                {
                    var preChar = name[i - 1];
                    if ((preChar >= 'A' && preChar <= 'Z') || (preChar >= 'a' && preChar <= 'z')) strb.Append(" ");
                }
                strb.Append(curChar);
            }
            return strb.ToString();
        }
    }
}