using System;
using System.Collections.Generic;
using System.Diagnostics;
using LIBII;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LIBII.Runtime.Samples
{
    public class NestedSample : MonoBehaviour
    {
        [ShowInInspector()] public bool hideIntField = false;
        [ShowInInspector()] public bool hideFloatField = false;
        [ShowInInspector()] public bool showProperty = true;

        [ShowInInspector(),
         Readonly,
         HideIf("hideIntField"),
         Box("Field and Property")]
        public int testIntField;
        
        [ShowInInspector(),
         Readonly,
         HideIf("hideFloatField"),
         Box("Field and Property")]
        public float testFloatField;

        [ShowInInspector("testIntField"),
         ShowIf("showProperty"),
         Box("Field and Property")]
        public int TestIntProperty
        {
            get => testIntField;
            set => testIntField = value;
        }

        [ShowInInspector] public List<int> intList = new List<int>();

        [Button, Horizontal("Method")]
        public void Test1()
        {
            Debug.Log("Test1");
        }

        [Button, Horizontal("Method")]
        public void Test2(string a, float b)
        {
            Debug.Log("Test2 " + a + " " + b);
        }

        [ShowInInspector()] public Class1 class1;
    }

    [Serializable]
    public class BaseClass
    {
        [ShowInInspector()] public bool hideIntField = false;
        [ShowInInspector()] public bool hideFloatField = false;
        [ShowInInspector()] public bool showProperty = true;

        [ShowInInspector(),
         Readonly,
         HideIf("hideIntField"),
         Box("Field and Property")]
        public int testIntField;
        
        [ShowInInspector(),
         Readonly,
         HideIf("hideFloatField"),
         Box("Field and Property")]
        public float testFloatField;

        [ShowInInspector("testIntField"),
         ShowIf("showProperty"),
         Box("Field and Property")]
        public int TestIntProperty
        {
            get => testIntField;
            set => testIntField = value;
        }

        [ShowInInspector] public List<int> intList = new List<int>();

        [Button, Vertical("Method")]
        public void Test1()
        {
            Debug.Log("Test1");
        }

        [Button, Vertical("Method")]
        public void Test2(string a, float b)
        {
            Debug.Log("Test2 " + a + " " + b);
        }
    }

    [Serializable]
    public class Class1 : BaseClass
    {
        public Class2 class2 = new Class2();
    }

    [Serializable]
    public class Class2 : BaseClass
    {
        public Class3 class3 = new Class3();
    }

    [Serializable]
    public class Class3 : BaseClass
    {
    }
}