using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ReadonlyAttribute : Attribute
    {
    }
}