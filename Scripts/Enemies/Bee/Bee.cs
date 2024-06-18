using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class Bee : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        patrolState=new BeePatrolState();
        chaseState=new BeeChaseState();
        if (!mannulDetection)
        {
            targetDetectionDistance = 4f;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (target)
        {
            targetPosition = new Vector2(target.position.x, target.position.y + target.GetComponent<Collider2D>().bounds.size.y / 2f);
        }
        weaponPosition = new Vector2(transform.position.x, transform.position.y - coll.bounds.size.y / 2f);
    }
    public override void Move()
    {
        if (isFoughtBack || isWaiting)//being controlled or being blocked
        {
            if (!isFoughtBack)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            return;
        }
        rb.velocity = new Vector2(moveDir.x * currentSpeed * Time.deltaTime, moveDir.y * currentSpeed * Time.deltaTime);
    }

    public override void Patrol()
    {
        if (Mathf.Abs(transform.position.x - destination.x) < 0.1 && Mathf.Abs(transform.position.y - destination.y) < 0.1)
        {
            if (isFoughtBack)
            {
                return;
            }
            rb.velocity = new Vector2(0f, 0f);
            isWaiting = true;
            Wait();
        }
        Move();
    }
    
    public override bool JudegeFindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, targetDetectionDistance, targetLayer);
        if (hit)
        {
            target = hit.transform;
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        //Detection
        Gizmos.DrawWireSphere(transform.position, targetDetectionDistance);
        //Patrol Area
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint, patrolDistance);
        //Weapon Position
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(weaponPosition, 0.2f);
        //Target Position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
    }
    public override void Wait()
    {
        if (isWaiting)
        {
            waitTimeTimer -= Time.deltaTime;
            if (waitTimeTimer <= 0)
            {
                destination=GetNewPoint();
                waitTimeTimer = waitTime;
                isWaiting = false;
            }
        }
    }
    public override void Chase()
    {
        
        if(Vector2.Distance(weaponPosition,targetPosition) < regularAttack.attackRange)
        {
            if (!isFoughtBack)
            {

                rb.velocity = Vector2.zero;
            }
            return;
        }
        if (isAttacking || isControlled || isFoughtBack)
        {
            return;
        }
        animator.SetBool("isRunning", true);
        rb.velocity = moveDir * currentSpeed * Time.deltaTime;
    }
}
