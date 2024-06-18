using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Video;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(PhysicalCheck))]
[RequireComponent(typeof(Character))]
public class Enemy : MonoBehaviour
{
    [Header("Properties")]
    public float currentSpeed;
    public float runSpeed;
    public float walkSpeed;
    public float jumpForce;
    public int size;
    public Vector2 moveDir;


    [Header("Reference")]
    public Rigidbody2D rb;
    public Collider2D coll;
    public Animator animator;
    public PhysicalCheck physicalCheck;
    public Attack touchAttack;
    public Attack regularAttack;
    public Character character;


    [Header("Status")]
    public bool isControlled;
    public bool isFoughtBack;
    public int faceDir;
    public bool isTargeting;
    public bool isAttacking;

    [Header("Timer")]
    public bool isWaiting;
    public float waitTime;
    public float waitTimeTimer;
    public float losingTargetTime;
    public float losingTargetTimeTimer;
    public float regularAttackRateTimer;

    [Header("State")]
    public EnemyBaseState currentState;
    public EnemyBaseState patrolState;
    public EnemyBaseState chaseState;
    public EnemyBaseState skillState;

    [Header("Detection")]
    public bool mannulDetection;
    public Vector2 targetDetectionOffset;
    public Vector2 targetDetectionSize;
    public float targetDetectionDistance;
    public LayerMask targetLayer;

    [Header("Patrol")]
    public Vector2 spawnPoint;
    public float patrolDistance;
    public Vector2 destination;

    [Header("Chase")]
    public Transform target;
    public Vector2 targetPosition;
    public Vector2 weaponPosition;

    protected virtual void Awake()
    {
        #region Initialize Status
        isControlled = false;
        isFoughtBack = false;
        faceDir = 1;
        isWaiting = false;
        isTargeting = false;
        #endregion

        #region Initialize Timer
        waitTimeTimer = waitTime;
        losingTargetTimeTimer = losingTargetTime;
        regularAttackRateTimer = regularAttack?regularAttack.attackRate:0;
        #endregion

        #region Initialize Reference
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        physicalCheck = GetComponent<PhysicalCheck>();
        character = GetComponent<Character>();
        #endregion

        #region Initialize Properties
        currentSpeed = walkSpeed;
        spawnPoint=transform.position;
        #endregion

        #region Initialize Animator
        animator.SetBool("isDead", false);
        #endregion

        #region Pre-set Values
        if (!mannulDetection)
        {
            targetDetectionOffset = coll.offset;
            targetDetectionSize = coll.bounds.size;
            targetDetectionDistance = 3 * coll.bounds.size.x;
        }
        #endregion
    }

    protected virtual void OnEnable()
    {
        #region Initialize State
        currentState = patrolState;
        patrolState.OnEnter(this);
        #endregion
    }
    private void OnDisable()
    {
        currentState.OnExit();
    }
    protected virtual void Update()
    {
        currentState.LogicUpdate();
    }
    void FixedUpdate()
    {
        transform.localScale = new Vector3(faceDir * size, transform.localScale.y, transform.localScale.z);
        currentState.PhysicalUpdate();
    }
    #region Detection
    public virtual bool JudegeFindTarget()
    {
        RaycastHit2D hit = Physics2D.BoxCast
            ((Vector2)transform.position + targetDetectionOffset, targetDetectionSize,
            0, new Vector2(faceDir, 0), targetDetectionDistance, targetLayer);
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
    #endregion

    #region Regular Motion of States
    #region Patrol
    public virtual void Patrol()
    {
        if (((physicalCheck.isTouchingLeftWall || (!physicalCheck.isLeftGround && physicalCheck.isGround)) && faceDir < 0) ||
            ((physicalCheck.isTouchingRightWall || (!physicalCheck.isRightGround && physicalCheck.isGround)) && faceDir > 0))
        {
            if (isFoughtBack)
            //when being fought back,may also go into this segment,the enemy's velocity should maintain in this case
            {
                return;
            }
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            rb.velocity = new Vector2(0f, rb.velocity.y);
            isWaiting = true;
            Wait();//time counter
        }
        else
        {
            isWaiting = false;
            animator.SetBool("isWalking", true);
        }
        Move();
    }
    public virtual void Move()
    {
        if (isFoughtBack || isWaiting||
            animator.GetCurrentAnimatorStateInfo(0).IsName("PreMove")||
            animator.GetCurrentAnimatorStateInfo(0).IsName("SnailEndHiding"))//being controlled or being blocked
        {
            if(!isFoughtBack)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            return;
        }
        rb.velocity = new Vector2(currentSpeed * faceDir * Time.deltaTime, rb.velocity.y);
        
    }
    public virtual Vector2 GetNewPoint()
    {
        Vector2 unitCirclePoint = Random.insideUnitCircle;
        return new Vector2(spawnPoint.x + unitCirclePoint.x * patrolDistance, spawnPoint.y + unitCirclePoint.y * patrolDistance);
    }
    #endregion
    #region Chase
    public virtual void Chase()
    {
        if (((physicalCheck.isTouchingLeftWall || (!physicalCheck.isLeftGround && physicalCheck.isGround)) && faceDir < 0) ||
            ((physicalCheck.isTouchingRightWall || (!physicalCheck.isRightGround && physicalCheck.isGround)) && faceDir > 0))
        {
            if (isFoughtBack)
            //when being fought back,may also go into this segment,the enemy's velocity should maintain in this case
            {
                return;
            }
            faceDir = -faceDir;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        Move();
        animator.SetBool("isRunning", true);
    }
    public virtual void Attack()
    {
        if (Vector2.Distance(weaponPosition,targetPosition) < regularAttack.attackRange)
        {
            if (isFoughtBack||isControlled)
            {
                return;
            }
            isAttacking = true;
            //the intervel of regular attack
            regularAttackRateTimer -= Time.deltaTime;
            if (regularAttackRateTimer <= 0)
            {
                animator.SetTrigger("attack");
                regularAttackRateTimer = regularAttack.attackRate;
            }   
        }
        else//out of the range of attack
        {
            isAttacking = false;
            return;
        }
    }
    #endregion
    #endregion

    #region Timer
    public virtual void Wait()
    {
        if (isWaiting)
        {
            waitTimeTimer -= Time.deltaTime;
            if (waitTimeTimer <= 0)
            {
                waitTimeTimer = waitTime;
                faceDir = -faceDir;
                isWaiting = false;
                animator.SetBool("isWalking", true);
            }
        }
    }

    public void LosingTargetCounting()
    {
        if (isTargeting)
        {
            losingTargetTimeTimer-=Time.deltaTime;
            if (losingTargetTimeTimer <= 0)
            {
                SwitchState(EnemyStateEnum.Patrol);
            }
        }
    }
    #endregion

    #region Being Attacked
    public void OnTakeDamage(Transform attackerTrans)
    {
        animator.SetTrigger("hurt");
        if (attackerTrans.GetComponent<Attack>().haveFightBack)
        {
            rb.velocity = Vector2.zero;
            isFoughtBack = true;
            Vector2 fightBackDir = new Vector2(transform.position.x - attackerTrans.position.x,transform.position.y-attackerTrans.position.y).normalized;
            StartCoroutine(FoughtBackEnumerator(fightBackDir, attackerTrans));
        }
    }
    public void OnDie()
    {
        gameObject.layer = 2;
        touchAttack.gameObject.SetActive(false);
        animator.SetBool("isDead", true);
    }
    private void DieAnimationEvent() { 
        Destroy(gameObject);
    }
    private IEnumerator FoughtBackEnumerator(Vector2 fightBackDir,Transform attackerTrans)
    {
        rb.AddForce(fightBackDir* attackerTrans.GetComponent<Attack>().fightBack,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isFoughtBack =false;
    }
    #endregion

    protected virtual void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireCube((Vector2)transform.position + targetDetectionOffset + new Vector2(faceDir*targetDetectionDistance,0),(Vector3)(targetDetectionSize));
        //Detection
        Gizmos.DrawWireSphere((Vector2)transform.position + targetDetectionOffset + new Vector2((float)faceDir * targetDetectionDistance, 0), 0.2f);
    }
    public void SwitchState(EnemyStateEnum state)
    {
        var newState = state switch
        {
            EnemyStateEnum.Patrol => patrolState,
            EnemyStateEnum.Chase => chaseState,
            EnemyStateEnum.Skill => skillState,
            _=>null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }
}
