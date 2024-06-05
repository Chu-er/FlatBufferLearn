using System.Collections;
using System.Collections.Generic;
using LIBII;
using UnityEngine;

public class ShowHideSample : MonoBehaviour
{
    public bool shouldShow;
    
    //shouldShow为true显示a这个Field，为false则隐藏
    [ShowIf("shouldShow")] public int a = 1;

    public bool shouldHide;

    //shouldShow为true隐藏b这个Field，为false则显示
    [HideIf("shouldHide")] public int b = 1;

    public enum ECondition
    {
        Show,
        Hide
    }

    public ECondition condition;

    //condition为ECondition.show显示c这个Field，为其它枚举值则隐藏
    [ShowIf("condition", ECondition.Show)] public int c = 1;
}