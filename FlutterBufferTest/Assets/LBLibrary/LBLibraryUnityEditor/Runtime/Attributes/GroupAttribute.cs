using System;

namespace LIBII
{
    public class GroupAttribute : Attribute
    {
        public enum ELayout
        {
            Horizontal,
            Vertical
        }

        public ELayout layout;
        public string path;
    }
}