using System.Collections.Generic;
using LIBII;
using UnityEngine;

namespace LIBII.Runtime.Samples
{
    public class PropertySample : MonoBehaviour
    {
        [ShowInInspector, Delayed] public float floatField;

        [ShowInInspector("floatField")]
        public float FloatProperty
        {
            get => floatField;
            set
            {
                Debug.Log("Delayed set float field value" + value);
                floatField = value;
            }
        }

        [ShowInInspector, SerializeField] private int readonlyIntField;

        [ShowInInspector("readonlyIntField")]
        public int ReadonlyIntProperty
        {
            get => readonlyIntField;
            private set => readonlyIntField = value;
        }

        [ShowInInspector] public List<int> listIntField = new List<int>();

        [ShowInInspector("listIntField")]
        public List<int> ListIntProperty
        {
            get => listIntField;
            set => listIntField = value;
        }
        
        [ShowInInspector] public SerializedClassSample serializedField = new SerializedClassSample();

        [ShowInInspector("serializedField")]
        public SerializedClassSample SerializedProperty
        {
            get => serializedField;
            set => serializedField = value;
        }
    }
}