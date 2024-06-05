namespace LIBII.CustomEditor
{
    public class InspectorDecorator : InspectorSorter
    {
        public DecoratorAttribute.EOrientation orientation = DecoratorAttribute.EOrientation.Up;
        
        public DecoratorAttribute decorateHandler;

        public InspectorDecorator(DecoratorAttribute decorator, int order)
        {
            this.order = order;
            this.decorateHandler = decorator;
        }
    }
}