using System;
using System.Collections.Generic;
using LIBII;
using UnityEngine;

namespace LIBII.Runtime.Samples
{
    [Serializable]
    public class SerializedClassSample
    {
        public int innerIntField = 0;

        public Vector3[] vector3s = new Vector3[4];

        public List<SerializedClassSample2> innerSerializedClassListField = new List<SerializedClassSample2>();
    }

    [Serializable]
    public class SerializedClassSample2
    {
        public float innerFloatField;
    }
}