using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class HorizontalAttribute : GroupAttribute
    {
        public HorizontalAttribute(string path)
        {
            this.path = path;
            this.layout = ELayout.Horizontal;
        }
    }
}