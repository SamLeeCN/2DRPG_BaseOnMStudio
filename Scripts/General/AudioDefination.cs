using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class AudioDefination : MonoBehaviour
{
    public bool playOnEnable;
    public AudioClip audioClip;
    public PlayAudioEventSO audioEvent;
    private void OnEnable()
    {
        if (playOnEnable)
        {
            audioEvent.RaiseEvent(audioClip);
        }
    }
    public void Play()
    {
        audioEvent.RaiseEvent(audioClip);
    }
}
