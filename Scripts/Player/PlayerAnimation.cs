using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public PlayerCharacterController playerController;
    public PhysicalCheck physicalCheck;
    public Character character;
    void Start()
    {
        animator=GetComponent<Animator>();
        rb=GetComponent<Rigidbody2D>();
        playerController=GetComponent<PlayerCharacterController>();
        physicalCheck = GetComponent<PhysicalCheck>();
        character = GetComponent<Character>();
    }

    void Update()
    {
        SetAnimation();
    }
    /*public void SetAnimation()
    {
        if (!physicalCheck.isGround)
        {
            animator.SetBool("isJumping",true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }*/
    public void SetAnimation()
    {
        animator.SetFloat("absVelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velocityY",rb.velocity.y);
        animator.SetBool("isGround", physicalCheck.isGround);
        animator.SetBool("isCrouching", playerController.isCrouching);
        animator.SetBool("isInvulnerable", character.isInvulnerable);
        animator.SetBool("isAttacking", playerController.isAttacking);
        animator.SetBool("isOnWall", physicalCheck.isOnWall);
        animator.SetBool("isSliding", playerController.isSliding);
    }
    public void PlayHurt()
    {
        animator.SetTrigger("hurt");
    }
    public void PlayAttack()
    {
        animator.SetTrigger("attack");
    }
    public void PlayDie()
    {
        animator.SetBool("isDead", true);
    }
    public void Respawn()
    {
        animator.SetBool("isDead", false);
    }
}
