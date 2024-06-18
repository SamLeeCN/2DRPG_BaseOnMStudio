using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName ="Event/ScreenFadeEvent")]
public class ScreenFadeEventSO : ScriptableObject
{
    public UnityAction<Color,float> OnEventRaised;
    //true means isFadeIn
    public void RaiseFadeInEvent(float duration)
    {
        OnEventRaised?.Invoke(Color.clear, duration);
    }
    public void RaiseFadeOutEvent(float duration)
    {
        OnEventRaised?.Invoke(Color.black,duration);
    }
}
