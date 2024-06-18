using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class SnailPatrolState : EnemyBaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.walkSpeed;
        currentEnemy.animator.SetBool("isHiding", false);
        currentEnemy.animator.SetBool("isWalking", true);
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.JudegeFindTarget())
        {
            currentEnemy.SwitchState(EnemyStateEnum.Skill);
        }
    }
    public override void PhysicalUpdate()
    {
        currentEnemy.Patrol();
    }
    public override void OnExit()
    {
        currentEnemy.isWaiting = false;
        currentEnemy.waitTimeTimer = currentEnemy.waitTime;
    }
}
