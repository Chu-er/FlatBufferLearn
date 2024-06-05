using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class HideIfAttribute : ConditionAttribute
    {
        /// <param name="condition">field or property name for reflection</param>
        public HideIfAttribute(string condition)
        {
            this.condition = condition;
        }

        /// <param name="condition">field or property name for reflection</param>
        /// <param name="conditionValue">if the type of this condition is not boolean, use this parameter</param>
        public HideIfAttribute(string condition, object conditionValue)
        {
            this.condition = condition;
            this.conditionValue = conditionValue;
        }
    }
}