using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string displayName;

        public DisplayNameAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }
}