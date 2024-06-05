using System.Collections.Generic;
using LIBII.Runtime.Samples;
using UnityEngine;

namespace LIBII.CustomEditor.Samples.Runtime
{
    public class CustomLBInspectorTest : MonoBehaviour
    {
        [ShowInInspector()] public int testInt = 0;
        [SerializeField, HideInInspector] private string testString = string.Empty;

        [ShowInInspector("testString")]
        public string TestString
        {
            get => testString;
            set
            {
                Debug.Log($"testString new value:{value}");
                testString = value;
            }
        }

        [ShowInInspector()]
        public List<SerializedClassSample> serializedClassSamples = new List<SerializedClassSample>();

        [ShowInInspector()] public List<GameObject> testGameObjects = new List<GameObject>();

        [Button()]
        public void TestMethod()
        {
            Debug.Log("call TestMethod");
        }
    }
}