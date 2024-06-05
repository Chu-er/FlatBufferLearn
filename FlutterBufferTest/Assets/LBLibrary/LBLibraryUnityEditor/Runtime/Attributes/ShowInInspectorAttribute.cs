using System;
#if UNITY_EDITOR
using System.Runtime.CompilerServices;
#endif

namespace LIBII
{
    /// <summary>
    /// Use this attribute to add a default declaration-based order to a field or a property, 
    /// since c# reflection has no way of getting members in declarative order
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowInInspectorAttribute : Attribute
    {
        public int order = 0;
        public string fieldName;

        /// <summary>
        /// For serialized field
        /// </summary>
        public ShowInInspectorAttribute(
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            this.order = order;
        }

        /// <summary>
        /// For property which is related to serialized field, there must be a serialized field in use
        /// </summary>
        public ShowInInspectorAttribute(string fieldName,
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            this.order = order;
            this.fieldName = fieldName;
        }
    }
}