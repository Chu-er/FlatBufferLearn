using UnityEngine;
using UnityEngine.UIElements;

namespace LIBII.Runtime.Samples
{
    public class HelpBoxSample : MonoBehaviour
    {
        [HelpBox("This a bool field for deciding whether the help boxes of IntProperty and Method can show",
            MessageType.Info)]
        public bool showPropertyAndMethodHelpBox;

        [HelpBox("This a field")] public int intField;

        [HelpBox("showPropertyAndMethodHelpBox", "This a property", MessageType.Warning)]
        [ShowInInspector("intField")]
        public int IntProperty
        {
            get => intField;
            set => intField = value;
        }

        [HelpBox("showPropertyAndMethodHelpBox", "This a method", MessageType.Error)]
        [Button]
        public void Method()
        {
        }
    }
}