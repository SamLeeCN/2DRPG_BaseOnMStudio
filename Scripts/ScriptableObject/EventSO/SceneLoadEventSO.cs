using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    

    public UnityAction<GameSceneSO, Vector3, bool> OnSceneLoadEventRaised;
    public void RaiseSceneLoadEvent(GameSceneSO sceneToGo, Vector3 posToGo, bool fadeScreen)
    {
        OnSceneLoadEventRaised?.Invoke(sceneToGo, posToGo, fadeScreen);
    }
}
