using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 

public class SceneManager : MonoBehaviour,ISaveable
{
    [Header("First Scene")]//the main menu
    public GameSceneSO mainMenuScene;
    public Vector3 firstPos;
    
    [Header("Event Listen")]
    public SceneLoadEventSO sceneLoadEvent;
    public ScreenFadeEventSO screenFadeEvent;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("Event Broadcast")]
    public VoidEventSO afterSceneLoadEvent;

    [Header("Teleport")]
    public GameSceneSO currentScene;
    public GameSceneSO sceneToGo;
    public Vector3 posToGo;
    public bool fadeScreen;

    [Header("Settings")]
    public float fadeDuration;

    [Header("Reference")]
    public Transform playerTrans;

    [Header("New Game")]
    public GameSceneSO initialScene;
    public Vector3 initialPos;
    private void Awake()
    {
        fadeScreen = true;
        
    }
    private void Start()
    {
        sceneLoadEvent.RaiseSceneLoadEvent(mainMenuScene, firstPos, false);
        //not in Awake() to ensure event broadcast after "sceneLoadEvent.OnSceneLoadEventRaised += OnSceneLoad;"
    }
    private void OnEnable()
    {
        sceneLoadEvent.OnSceneLoadEventRaised += OnSceneLoad;
        newGameEvent.OnEventRaised += OnNewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        ISaveable saveable = this;
        saveable.RegisterSaveData();

    }
    private void OnDisable()
    {
        sceneLoadEvent.OnSceneLoadEventRaised -= OnSceneLoad;
        newGameEvent.OnEventRaised -= OnNewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
        ISaveable saveable = this;
        saveable.UnregisterSaveData();
    }
    private void OnBackToMenuEvent()
    {
        sceneToGo = mainMenuScene;
        posToGo = firstPos;
        sceneLoadEvent.RaiseSceneLoadEvent(sceneToGo, posToGo, true);
    }

    private void OnSceneLoad(GameSceneSO sceneToGo, Vector3 posToGo, bool fadeScreen)
    {
        this.sceneToGo = sceneToGo;
        this.posToGo = posToGo;
        this.fadeScreen = fadeScreen;
        StartCoroutine(UnloadPreviousSceneEnumerator());
    }
    private IEnumerator UnloadPreviousSceneEnumerator()
    {
        if (fadeScreen)
        {
            screenFadeEvent.RaiseFadeOutEvent(fadeDuration);
        }
        if(currentScene != null)
        {
            yield return new WaitForSeconds(fadeDuration);
            yield return currentScene.sceneReference.UnLoadScene();
        }
        LoadNewScene(sceneToGo,posToGo);
    }
    private void LoadNewScene(GameSceneSO sceneToLoad,Vector3 posToGo)
    {
        sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive,true);
        //true means activate the scene after it is loaded
        playerTrans.position = posToGo;
        currentScene = sceneToLoad;
        if (fadeScreen)
        {
            screenFadeEvent.RaiseFadeInEvent(fadeDuration);
        }
        if (sceneToLoad.sceneType != SceneType.Menu)
        {
            afterSceneLoadEvent.RaiseEvent();//enable input controller
        }
    }
    private void OnNewGame()
    {
        sceneLoadEvent.RaiseSceneLoadEvent(initialScene, initialPos, true);
    }


    #region save&load
    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void SaveData(Data data)
    {
        data.SaveSceneToData(currentScene);
    }

    public void LoadData(Data data)
    {
        String playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            sceneToGo = data.GetSavedScene();
            posToGo = data.characterPosDict[playerID].ToVector3();
            sceneLoadEvent.RaiseSceneLoadEvent(sceneToGo, posToGo, true);
        }
    }

    #endregion
}
