using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Chest : MonoBehaviour,IInteractable
{
    [Header("Reference")]
    public SpriteRenderer spriteRenderer;
    public Sprite chestCloseSprite;
    public Sprite chestOpenSprite;

    [Header("Properties")]
    public bool isDone;

    [Header("Audio")]
    public AudioDefination audioDefination;

    private void Awake()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();   
        audioDefination=GetComponent<AudioDefination>();
    }
    private void OnEnable()
    {
        spriteRenderer.sprite=isDone ? chestOpenSprite : chestCloseSprite;
        gameObject.tag = isDone ? "Untagged" : "Interactable";
    }
    public void TriggerAction()
    {
        OpenChest();
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = chestOpenSprite;
        audioDefination.Play();
        isDone = true;
        gameObject.tag = "Untagged";
    }
    
}
