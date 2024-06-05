using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LIBII;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace LIBII.Runtime.Samples
{
    public class ListAndArraySample : MonoBehaviour
    {
        public Rect[] vector2s = new Rect[4];
        public float[] floats = new float[20];
        public List<SerializedClassSample> serializedObjList = new List<SerializedClassSample>();
    }
}