using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SavePoint : MonoBehaviour,IInteractable
{

    [Header("Reference")]
    public TextMeshPro text;
    public GameObject lightObj; 
    [Header("Properties")]
    public bool isDone;
    [Header("Event Broadcast")]
    public VoidEventSO saveDataEvent; 

    private void OnEnable()
    {
        text.color=isDone?new Color32(255,255,255,255):new Color32 (124,124,124,255);
        lightObj.SetActive(isDone?true:false);
    }
    public void TriggerAction()
    {
        text.color=new Color32 (255,255,255,255);
        lightObj.SetActive (true);
        isDone = true;
        gameObject.tag = "Untagged";
        saveDataEvent.RaiseEvent();
    }
}
