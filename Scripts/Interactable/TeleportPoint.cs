using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class TeleportPoint : MonoBehaviour,IInteractable
{
    public SceneLoadEventSO sceneLoadEvent;
    
    public GameSceneSO sceneToGo;
    public Vector3 posToGo;
    public void TriggerAction()
    {
        sceneLoadEvent.RaiseSceneLoadEvent(sceneToGo,posToGo,true);
    }
}
