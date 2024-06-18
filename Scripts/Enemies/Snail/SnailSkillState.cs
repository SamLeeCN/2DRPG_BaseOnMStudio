using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SnailSkillState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.isTargeting = true;
        currentEnemy.animator.SetBool("isWalking", false);
        currentEnemy.animator.SetBool("isHiding", true);
        currentEnemy.animator.SetTrigger("hide");
        currentEnemy.character.isInvulnerable = true;
        currentEnemy.rb.velocity = Vector2.zero;
    }
    public override void LogicUpdate()
    {
        currentEnemy.character.invulnerableCounter = currentEnemy.character.invulnerableDuration;
        if (!currentEnemy.JudegeFindTarget())
        {
            currentEnemy.LosingTargetCounting();
        }
    }
    public override void PhysicalUpdate()
    {

    }
    public override void OnExit()
    {
        currentEnemy.animator.SetBool("isHiding", false);
        currentEnemy.isTargeting = false;
        currentEnemy.losingTargetTimeTimer = currentEnemy.losingTargetTime;
    }
}
