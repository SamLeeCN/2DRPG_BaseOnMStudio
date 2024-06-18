using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    [Header("Event Listen")]
    public PlayAudioEventSO BGMEvent;
    public PlayAudioEventSO FXEvent;
    public FloatEventSO volumeChangeEvent;
    public VoidEventSO toggleSettingPanelEvent;
    [Header("Event Broadcast")]
    public FloatEventSO sycVolumeUIEvent;
    [Header("Source")] 
    public AudioSource BGMSource;
    public AudioSource FXSource;
    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        volumeChangeEvent.OnEventRaised += OnVolumeChangeEvent;
        toggleSettingPanelEvent.OnEventRaised += SycVolumeUI;
    }
    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeChangeEvent.OnEventRaised -= OnVolumeChangeEvent;
        toggleSettingPanelEvent.OnEventRaised -= SycVolumeUI;
    }

    private void SycVolumeUI()
    {
        float volumeAmount;
        mainMixer.GetFloat("MasterVolume",out volumeAmount);
        sycVolumeUIEvent.RaiseEvent(volumeAmount);
    }

    private void OnVolumeChangeEvent(float value)
    {
        //value varies from 0 to 1
        mainMixer.SetFloat("MasterVolume",value*100-80);//volume varies from -80 to 20
    }

    private void OnFXEvent(AudioClip audioClip)
    {
        FXSource.clip = audioClip;
        FXSource.Play();
    }

    private void OnBGMEvent(AudioClip audioClip)
    {
        BGMSource.clip = audioClip;
        BGMSource.Play();
    }

    void Start()
    {

    }

    void Update()
    {

    } 
}
