using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BeeChaseState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.isTargeting = true;
        currentEnemy.currentSpeed = currentEnemy.runSpeed;
        currentEnemy.animator.SetBool("isRunning", true);
        currentEnemy.animator.SetBool("isWalking", false);
        
    }

    public override void LogicUpdate()
    {
        currentEnemy.moveDir = (currentEnemy.targetPosition- currentEnemy.weaponPosition).normalized;
        if (currentEnemy.moveDir.x > 0)
        {
            currentEnemy.faceDir = 1;
        }
        else
        {
            currentEnemy.faceDir = -1;
        }
        if (!currentEnemy.JudegeFindTarget())
        {
            currentEnemy.LosingTargetCounting();
        }
    }

    public override void PhysicalUpdate()
    {
        currentEnemy.Chase();
        currentEnemy.Attack();
    }

    public override void OnExit()
    {
        currentEnemy.isTargeting = false;
        currentEnemy.losingTargetTimeTimer = currentEnemy.losingTargetTime;
    }

    
}
