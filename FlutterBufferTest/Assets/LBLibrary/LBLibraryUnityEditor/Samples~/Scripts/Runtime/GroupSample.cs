using System.Collections;
using System.Collections.Generic;
using LIBII;
using LIBII.Runtime.Samples;
using UnityEngine;

public class GroupSample : MonoBehaviour
{
    [Title("Box"), Box("VerticalBox", false)]
    public int i1;

    [Box("VerticalBox", false)]
    public int i2;

    [Box("HorizontalBox", GroupAttribute.ELayout.Horizontal)]
    public int i16;

    [Box("HorizontalBox", GroupAttribute.ELayout.Horizontal)]
    public int i17;

    [Box("Box")] public int i3;
    [Box("Box/SubBox")] public int i4;

    [Title("Horizontal"), Horizontal("Horizontal")] public int i5;
    [Horizontal("Horizontal")] public int i6;

    [Title("Vertical"), Vertical("Vertical")] public int i9;
    [Vertical("Vertical")] public int i10;

    [Title("Mix"), Box("MixBox")] public int i11;
    [Horizontal("MixBox/Horizontal")] public int i12;
    [Horizontal("MixBox/Horizontal")] public int i13;
    [Vertical("MixBox/Vertical")] public int i14;
    [Vertical("MixBox/Vertical")] public int i15;

    [Title("Serializable Class"), Box("BoxVerticalS")]
    public SerializedClassSample sample7 = new SerializedClassSample();
    
    [Box("BoxVerticalS")]
    public SerializedClassSample sample8 = new SerializedClassSample();
    
    [Box("BoxHorizontalS", GroupAttribute.ELayout.Horizontal)]
    public SerializedClassSample sample1 = new SerializedClassSample();
    
    [Box("BoxHorizontalS", GroupAttribute.ELayout.Horizontal)]
    public SerializedClassSample sample2 = new SerializedClassSample();

    [Horizontal("HorizontalS")]
    public SerializedClassSample sample3 = new SerializedClassSample();
    
    [Horizontal("HorizontalS")]
    public SerializedClassSample sample4 = new SerializedClassSample();
    
    [Vertical("VerticalS")]
    public SerializedClassSample sample5 = new SerializedClassSample();
    
    [Vertical("VerticalS")]
    public SerializedClassSample sample6 = new SerializedClassSample();
}