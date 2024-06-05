using System;

namespace LIBII
{
    public class ConditionAttribute : Attribute
    {
        public string condition;
        public object conditionValue;
    }
}