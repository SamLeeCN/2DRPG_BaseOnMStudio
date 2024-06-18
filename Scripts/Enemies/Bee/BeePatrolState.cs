using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class BeePatrolState : EnemyBaseState
{
    
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.walkSpeed;
        currentEnemy.animator.SetBool("isRunning", false);
        currentEnemy.animator.SetBool("isWalking", true);
        currentEnemy.destination=currentEnemy.GetNewPoint();
    }
    public override void LogicUpdate()
    {
        if (currentEnemy.JudegeFindTarget())
        {
            currentEnemy.SwitchState(EnemyStateEnum.Chase);
        }
        currentEnemy.moveDir = (currentEnemy.destination - (Vector2)currentEnemy.transform.position).normalized;
        if (currentEnemy.moveDir.x > 0)
        {
            currentEnemy.faceDir = 1;
        }
        else
        {
            currentEnemy.faceDir = -1;
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
