using System.Collections.Generic;
using LIBII;
using UnityEngine;

namespace LIBII.Runtime.Samples
{
    public class TitleSample : MonoBehaviour
    {
        [Title("Int")] public int a = 1;

        [Title("IntList")] public List<int> b = new List<int>();

        [Title("SerializedClass")] public SerializedClassSample c = new SerializedClassSample();
    }
}