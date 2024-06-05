using System;
using UnityEngine;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public abstract class DecoratorAttribute : Attribute
    {
        public enum EOrientation
        {
            Up,
            Down
        }
        
        public EOrientation orientation = EOrientation.Up;
        
#if UNITY_EDITOR
        public abstract float GetHeight();

        public abstract bool OnGUI(Rect position);
#endif
    }
}