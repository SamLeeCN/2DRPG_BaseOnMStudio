using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BoarChaseState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.isTargeting = true;
        currentEnemy.currentSpeed = currentEnemy.runSpeed;
        currentEnemy.animator.SetBool("isWalking", false);
        currentEnemy.animator.SetBool("isRunning", true);
    }
    public override void LogicUpdate()
    {
        if (!currentEnemy.JudegeFindTarget())
        {
            currentEnemy.LosingTargetCounting();
        }
    }
    public override void PhysicalUpdate()
    {
        currentEnemy.Chase();
    }
    public override void OnExit()
    {
        currentEnemy.isTargeting = false;
        currentEnemy.losingTargetTimeTimer=currentEnemy.losingTargetTime;
    }
}
