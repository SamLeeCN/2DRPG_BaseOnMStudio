using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class MainMenu : MonoBehaviour
{
    public GameObject btnNewGame;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(btnNewGame);
    }
    public void Exit()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
