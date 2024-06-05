using System;
#if UNITY_EDITOR
using System.Runtime.CompilerServices;
#endif

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public enum EExecuteType
        {
            Runtime,
            Editor,
            Both
        }
        
        public int order;
        public string label;
        public EExecuteType executeType;

        public ButtonAttribute(
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            label = "";
            this.executeType = EExecuteType.Both;
            this.order = order;
        }

        public ButtonAttribute(
            string label,
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            this.label = label;
            this.executeType = EExecuteType.Both;
            this.order = order;
        }
        
        public ButtonAttribute(
            EExecuteType executeType,
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            this.label = "";
            this.executeType = executeType;
            this.order = order;
        }
        
        public ButtonAttribute(
            string label,
            EExecuteType executeType,
#if UNITY_EDITOR
            [CallerLineNumber]
#endif
            int order = default)
        {
            this.label = label;
            this.executeType = executeType;
            this.order = order;
        }
    }
}