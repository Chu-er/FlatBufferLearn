using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace LIBII
{
    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error,
    }
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class HelpBoxAttribute : ConditionAttribute
    {
        public string text;
        public MessageType type;
        public GUIContent label;

        /// <summary>
        /// Default show if there is no condition
        /// </summary>
        public HelpBoxAttribute(string text, MessageType type = MessageType.None)
        {
            this.text = text;
            this.type = type;
        }

        /// <param name="condition">field or property name for reflection</param>
        public HelpBoxAttribute(string condition, string text, MessageType type = MessageType.None)
        {
            this.condition = condition;

            this.text = text;
            this.type = type;
        }

        /// <param name="condition">field or property name for reflection</param>
        /// <param name="conditionValue">if the type of this condition is not boolean, use this parameter</param>
        public HelpBoxAttribute(string condition, object conditionValue, string text,
            MessageType type = MessageType.None)
        {
            this.condition = condition;
            this.conditionValue = conditionValue;

            this.text = text;
            this.type = type;
        }
    }
}