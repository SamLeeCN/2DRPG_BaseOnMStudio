using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.InputSystem;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    [Header("Event Listen")]
    public VoidEventSO afterSceneLoadEvent;
    void Awake()
    {
        confiner = transform.GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadEvent.OnEventRaised += OnAfterSceneLoadEvent;
    }
    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadEvent.OnEventRaised -= OnAfterSceneLoadEvent;
    }
    private void OnAfterSceneLoadEvent()
    {
        GetNewCameraBounds();
    }
    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    void Start()
    {
          
    }
    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        confiner.InvalidateCache();
        if(obj != null)
        {
            confiner.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        }
    }
    void Update()
    {

    } 
}
