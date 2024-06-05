using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class BoxAttribute : GroupAttribute
    {
        public bool collapsible;
        public bool isFoldoutByDefault;

        public BoxAttribute(string path, ELayout layout = ELayout.Vertical)
        {
            this.path = path;
            this.layout = layout;
            this.collapsible = true;
            this.isFoldoutByDefault = true;
        }

        public BoxAttribute(string path, bool collapsible, bool isFoldoutByDefault = true)
        {
            this.path = path;
            this.layout = ELayout.Vertical;
            this.collapsible = collapsible;
            this.isFoldoutByDefault = isFoldoutByDefault;
        }

        public BoxAttribute(string path, bool collapsible, bool isFoldoutByDefault, ELayout layout)
        {
            this.path = path;
            this.layout = layout;
            this.collapsible = collapsible;
            this.isFoldoutByDefault = isFoldoutByDefault;
        }
    }
}