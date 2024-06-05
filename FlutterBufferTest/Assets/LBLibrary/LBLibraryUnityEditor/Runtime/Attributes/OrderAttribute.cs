using System;

namespace LIBII
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class OrderAttribute : Attribute
    {
        public int order;
        
        public OrderAttribute(int order)
        {
            this.order = order;
        }
    }
}