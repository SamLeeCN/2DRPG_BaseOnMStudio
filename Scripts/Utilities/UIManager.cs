using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class UIManager : MonoBehaviour
{
    public PlayerInputController inputController;
    public GameObject mobileTouch;
    [Header("Player Status")]
    public PlayerStatusUI playerStatus;
    public CharacterEventSO healthEventSO;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public GameObject backToSavePointBtn;

    [Header("Setting Panel")]
    public GameObject settingPanel;
    public Slider volumeControlSlider;

    [Header("Event Listen")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;
    public VoidEventSO toggleSettingPanelEvent;
    public FloatEventSO sycVolumeUIEvent;
    private void Awake()
    {
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif
        inputController = new PlayerInputController();
        inputController.UI.Setting.started+=ToggleSettingPanel;
    }

    

    private void OnEnable()
    {
        inputController.Enable();
        healthEventSO.OnEventRaised += OnHealthEvent;
        sceneLoadEvent.OnSceneLoadEventRaised += OnSceneLoadEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        toggleSettingPanelEvent.OnEventRaised += OnToggleSettingPanelEvent;
        sycVolumeUIEvent.OnEventRaised += OnSycVolumeUIEvent;
    }
    private void OnDisable()
    {
        inputController.Disable();
        healthEventSO.OnEventRaised -= OnHealthEvent;
        sceneLoadEvent.OnSceneLoadEventRaised -= OnSceneLoadEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
        toggleSettingPanelEvent.OnEventRaised -= OnToggleSettingPanelEvent;
        sycVolumeUIEvent.OnEventRaised -= OnSycVolumeUIEvent;
    }

    private void OnSycVolumeUIEvent(float volumeAmount)
    {
        volumeControlSlider.value=(volumeAmount+80)/100;
    }

    private void ToggleSettingPanel(InputAction.CallbackContext context)
    {
        toggleSettingPanelEvent.RaiseEvent();
    }
    private void OnToggleSettingPanelEvent()
    {
        if (settingPanel.activeInHierarchy)
        {
            settingPanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            settingPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    #region game over panel

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backToSavePointBtn);
    }
    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);
    }
    private void OnBackToMenuEvent()
    {
        gameOverPanel.SetActive(false);
    }
    #endregion
    private void OnSceneLoadEvent(GameSceneSO sceneToGO, Vector3 posToGO, bool fadeScreen)
    {
        bool isMenuScene=sceneToGO.sceneType==SceneType.Menu;
        playerStatus.gameObject.SetActive(!isMenuScene);
    }

    private void OnHealthEvent(Character character)
    {
        playerStatus.SetCharacter(character);
        float persentageHealth = (float)character.currentHealth / character.maxHealth;
        playerStatus.OnHealthChange(persentageHealth);
    }
}
