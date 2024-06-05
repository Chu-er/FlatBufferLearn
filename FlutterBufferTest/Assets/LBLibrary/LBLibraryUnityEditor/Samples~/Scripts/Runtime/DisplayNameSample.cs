using System.Collections;
using System.Collections.Generic;
using LIBII;
using UnityEngine;

public class DisplayNameSample : MonoBehaviour
{
    [ShowInInspector, DisplayName("这是一个整型字段")]public int intField;

    [ShowInInspector("intField"), DisplayName("这是上面那个整型字段的关联属性")]
    public int IntProperty
    {
        get => intField;
        set => intField = value;
    }
}
