using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class FadeScreen : MonoBehaviour
{
    public ScreenFadeEventSO screenFadeEvent;
    public Image screenFadeImage;
    private void OnEnable()
    {
        screenFadeEvent.OnEventRaised += OnScreenFadeEvent;
    }
    private void OnDisable()
    {
        screenFadeEvent.OnEventRaised -= OnScreenFadeEvent;
    }
    private void OnScreenFadeEvent(Color color, float duration)
    {
        screenFadeImage.DOBlendableColor(color, duration);
    }
    private void Awake()
    {
        screenFadeImage = GetComponent<Image>();
    }

}
