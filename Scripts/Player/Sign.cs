using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Sign : MonoBehaviour
{
    private IInteractable targetItem;
    bool isInteractable;
    [Header("KeyPrompt")]
    public GameObject keyPrompt;
    private Animator keyPromptAnimator;
    private SpriteRenderer keyPromptSpriteRenderer;
    public Transform playerTrans;
    private PlayerInputController playerInputController;

    private void Awake()
    {
        isInteractable = false;
        keyPromptAnimator=keyPrompt.GetComponent<Animator>();
        keyPromptSpriteRenderer=keyPrompt.GetComponent<SpriteRenderer>();
        keyPromptSpriteRenderer.enabled=false;
        playerInputController=new PlayerInputController();
        playerInputController.Enable();
    }
    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        playerInputController.GamePlay.Confirm.started += OnConfirm;
    }
    void Update()
    {
        keyPrompt.transform.localScale = playerTrans.localScale;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable")){
            targetItem=collision.GetComponent<IInteractable>();
            isInteractable = true;
            keyPromptAnimator.SetBool("isPrompting",true);
            keyPromptSpriteRenderer.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>()==targetItem)
        {
            isInteractable = false;
            keyPromptAnimator.SetBool("isPrompting", false);
            keyPromptSpriteRenderer.enabled = false;
        }
    }
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            var device = ((InputAction)obj).activeControl.device;
            switch (device.device)
            {
                case Keyboard:
                    keyPromptAnimator.SetBool("isKeyboard", true);
                    break;
                case Gamepad:
                    keyPromptAnimator.SetBool("isKeyboard", false);
                    break;
            }
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (!isInteractable)
        {
            return;
        }
        targetItem.TriggerAction();
    }
}
