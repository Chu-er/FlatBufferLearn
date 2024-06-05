using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class VerticalAttribute : GroupAttribute
    {
        public VerticalAttribute(string path)
        {
            this.path = path;
            this.layout = ELayout.Vertical;
        }
    }
}