using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PlayerCharacterController : MonoBehaviour
{
    //input
    public PlayerInputController inputController;
    public Vector2 moveInput;

    [Header("Event Listen")]
    public VoidEventSO afterSceneLoadEvent;
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    [Header("Reference")]
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PhysicalCheck physicalCheck;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CapsuleCollider2D coll;
    [HideInInspector] public Character character;
    [HideInInspector] public PlayerAnimation playerAnimation;
    public PhysicsMaterial2D normalPhysicalMaterial;
    public PhysicsMaterial2D smoothPhysicalMaterial;
    public GameObject slideAttack;

    [Header("Properties")]
    //public float accelaration;
    //public float maxSpeed;
    public float currentSpeed;
    public float runSpeed;
    public float walkSpeed;
    public float waterMovingAccelaration;
    public float jumpForce;
    public float jumpHoldForce;
    public float jumpHoldingMaxTime;//0.2f recommended
    public float wallJumpForce;
    public float wallSlidingSpeed;
    public float slideDistance;
    public float slideSpeed;
    public int size;

    [Header("Bool Status")]
    public bool isCrouching;
    public bool isAttacking;
    public bool isFoughtBack;
    public bool isControlled;
    public bool isWallJumping;
    public bool isJumping;
    public bool isSliding;

    [Header("Assistant")]
    private float jumpForceStopTime;

    [Header("Coroutine")]
    private Coroutine slideCoroutine;

    [Header("AudioClip")]
    public AudioDefination jumpAudioDefination;

    //other status
    int faceDir;
    private void Awake()
    {
        #region Initialize the Properties
        currentSpeed = runSpeed;
        faceDir = 1;
        #endregion

        #region Initialize Reference
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicalCheck = GetComponent<PhysicalCheck>();
        inputController = new PlayerInputController();
        coll = GetComponent<CapsuleCollider2D>();
        character = GetComponent<Character>();
        playerAnimation = GetComponent<PlayerAnimation>();
        #endregion

        #region Initialize Input
        inputController=new PlayerInputController();
        inputController.GamePlay.Jump.started += OnJumpStarted;
        inputController.GamePlay.Jump.canceled += OnJumpReleased;
        inputController.GamePlay.Attack.started += PlayerAttack;
        inputController.GamePlay.WalkSwitch.performed += (InputAction.CallbackContext ctx) =>
        {
            currentSpeed = walkSpeed;
        };
        inputController.GamePlay.WalkSwitch.canceled += (InputAction.CallbackContext ctx) =>
        {
            currentSpeed = runSpeed;
        };
        inputController.GamePlay.Slide.started += Slide;
        #endregion

        #region Initialize Status
        isFoughtBack = false;
        #endregion

    }

    void Start()
    {

    }

    void Update()
    {
        moveInput=inputController.GamePlay.Move.ReadValue<Vector2>();
        #region Judge whether being controlled
        if (isFoughtBack)
        {
            isControlled = true;
        }
        else
        {
            isControlled = false;
        }
        #endregion
        #region Align Friction
        if(!physicalCheck.isGround)
        {
            rb.sharedMaterial = smoothPhysicalMaterial;
            coll.sharedMaterial = smoothPhysicalMaterial;
        }
        else
        {
            rb.sharedMaterial = normalPhysicalMaterial;
            coll.sharedMaterial = normalPhysicalMaterial;
        }
        #endregion
    }
    private void FixedUpdate()
    {
        FaceDirection();
        Move();
        JumpHold();
        OnWall();
        ColliderAdjustment();
    }
    private void OnEnable()
    {
        inputController.Enable();
        sceneLoadEvent.OnSceneLoadEventRaised += OnSceneLoad;
        afterSceneLoadEvent.OnEventRaised += OnAfterSceneLoad;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
    }

    

    private void OnDisable()
    {
        inputController.Disable();
        sceneLoadEvent.OnSceneLoadEventRaised -= OnSceneLoad;
        afterSceneLoadEvent.OnEventRaised -= OnAfterSceneLoad;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
    }

    

    private void OnSceneLoad(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputController.Disable();
    }
    private void OnAfterSceneLoad()
    {
        inputController.Enable();
    }
    #region Behaviour Control
    private void FaceDirection()
    {
        if (isAttacking||isSliding)
        {
            return;
        }
        if (moveInput.x > 0)
        {
            faceDir = 1;
        }
        else if (moveInput.x < 0)
        {
            faceDir = -1;

        }
        transform.localScale = new Vector3
                (size * faceDir, transform.localScale.y, transform.localScale.z);
    }
    private void Move()
    {
        if (isControlled || (isAttacking && physicalCheck.isGround) || isWallJumping||isSliding)
        {
            return;
        }
        rb.velocity = new Vector2
                (currentSpeed * moveInput.x * Time.deltaTime, rb.velocity.y);
        if(physicalCheck.isInWater)
        {
            rb.AddForce (new Vector2(0f,rb.mass*waterMovingAccelaration*moveInput.y),ForceMode2D.Force);
        }
        isCrouching = physicalCheck.isGround && moveInput.y < -0.5f;
    }
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        //stop sliding
        if (slideCoroutine != null && !physicalCheck.isTouchingTop)
        {
            StopCoroutine(slideCoroutine);
            OnCoroutineSlideExit();
        }
        if (physicalCheck.isGround)
        {//jump from ground 
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (physicalCheck.isTouchingLeftWall || physicalCheck.isTouchingRightWall)
        {//jump from wall
            StartCoroutine(WallJumpEnumerator());
        }
        else if (physicalCheck.canWaterJump)
        {//jump from water surface
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Force);
        }
        else
        {
            return;
        }
        jumpAudioDefination.Play();
        jumpForceStopTime = Time.time + jumpHoldingMaxTime;
        isJumping = true;
    }

    
    private void JumpHold()
    {

        if (!inputController.GamePlay.Jump.IsPressed()||!isJumping||Time.time>=jumpForceStopTime)
        {
            return;
        }
        //holding jump button in the air after jump
        rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
    }
    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        isJumping = false;
    }
    IEnumerator WallJumpEnumerator()
    {
        isWallJumping = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(transform.up * wallJumpForce, ForceMode2D.Impulse);
        rb.AddForce(transform.right * wallJumpForce* -faceDir * 0.5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.4f);
        OnCoroutineWallJumpExit();
    }
    private void OnCoroutineWallJumpExit()
    {
        isWallJumping = false;
    }
    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (physicalCheck.isOnWall)
        {
            return;
        }
        isAttacking = true;
        if(physicalCheck.isGround)
        {
            rb.velocity = Vector2.zero;
        }
        playerAnimation.PlayAttack();
    }

    private void OnWall()
    {
        if (physicalCheck.isOnWall&&!physicalCheck.isInWater) {
            rb.velocity=new Vector2(rb.velocity.x, -wallSlidingSpeed);
        }
    }
    private void Slide(InputAction.CallbackContext context)
    {
        if (character.currentPower < character.slidePowerCost)
        {
            return;
        }
        slideCoroutine=StartCoroutine(SlideEnumerator());
    }
    IEnumerator SlideEnumerator()
    {
        physicalCheck.leftOffset = new Vector2((coll.offset.x - coll.bounds.size.x / 2 - 0.1f) * size, 0.42f * size);
        physicalCheck.rightOffset = new Vector2((coll.offset.x + coll.bounds.size.x / 2 + 0.1f) * size, 0.42f * size);
        physicalCheck.isTouchingLeftWall = false;
        physicalCheck.isTouchingRightWall = false;
        bool isOutOfDistance = false;
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        isSliding = true;
        character.CostPower(character.slidePowerCost);
        float destinationX = transform.position.x + faceDir * slideDistance;
        do
        {
            yield return null;
            slideAttack.SetActive(true);
            if (physicalCheck.isTouchingTop)
            {
                isOutOfDistance = true;
            }
            rb.velocity = new Vector2(faceDir * slideSpeed, rb.velocity.y);
            if ((faceDir == 1 && physicalCheck.isTouchingRightWall)||(faceDir == -1 && physicalCheck.isTouchingLeftWall))
            {
                break;
            }
        } while ((Math.Abs(transform.position.x-destinationX)>0.1f&&!isOutOfDistance)||physicalCheck.isTouchingTop);
        OnCoroutineSlideExit();
    }
    private void OnCoroutineSlideExit()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        isSliding = false;
        slideAttack.SetActive(false);
    }
    #endregion

    #region Being Attack
    public void BeFoughtBack(Transform attacker)
    {
        isFoughtBack = true;
        rb.velocity = Vector2.zero;
        Vector2 fightBackDir = new Vector2
            (transform.position.x - attacker.transform.position.x, transform.position.y - attacker.transform.position.y).normalized;
        StartCoroutine(FoughtBackEnumerator(fightBackDir, attacker));
    }
    private IEnumerator FoughtBackEnumerator(Vector2 fightBackDir, Transform attackerTrans)
    {
        rb.AddForce(fightBackDir * attackerTrans.GetComponent<Attack>().fightBack, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isFoughtBack = false;
    }
    public void OnTakeDamage(Transform attackerTrans)
    {
        if (attackerTrans.GetComponent<Attack>().haveFightBack)
        {
            BeFoughtBack(attackerTrans);
        }
    }
    #endregion

    private void ColliderAdjustment()
    {
        if (isCrouching)
        {
            coll.direction = CapsuleDirection2D.Vertical;
            coll.offset = new Vector2(0f, 0.73f * size);
            coll.size = new Vector2(0.7f * size, 1.43f * size);
            physicalCheck.leftOffset = new Vector2(-0.35f * size, 0.88f * size);
            physicalCheck.rightOffset = new Vector2(0.35f * size, 0.88f * size);
            physicalCheck.topOffset = new Vector2(0.05f * size, 2.07f * size);
        }
        else if (isSliding)
        {
            coll.direction = CapsuleDirection2D.Horizontal;
            coll.offset = new Vector2(0f, 0.36f * size);
            coll.size = new Vector2(1.3f * size, 0.72f * size);
            physicalCheck.leftOffset = new Vector2((coll.offset.x - coll.bounds.size.x / 2 - 0.1f) * size, 0.42f * size);
            physicalCheck.rightOffset = new Vector2((coll.offset.x + coll.bounds.size.x / 2 + 0.1f) * size, 0.42f * size);
            physicalCheck.topOffset = new Vector2(0.05f * size, 2.07f * size);
        }
        else
        {//regular
            coll.direction = CapsuleDirection2D.Vertical;
            coll.offset = new Vector2(0f, 1.03f * size);
            coll.size = new Vector2(0.7f * size, 2.08f * size);
            physicalCheck.leftOffset = new Vector2(-0.35f * size, 0.88f * size);
            physicalCheck.rightOffset = new Vector2(0.35f * size, 0.88f * size);
            physicalCheck.topOffset = new Vector2(0.05f * size, 2.07f * size);
        }
    }
    public void SetInputControllerStatus(bool status)
    {
        if (status)
        {
            inputController.Enable();
        }
        else
        {
            inputController.Disable();
        }
    }
    private void OnLoadDataEvent()
    {
        character.isDead = false;
        playerAnimation.Respawn();
    }
    private void OnBackToMenuEvent()
    {
        character.isDead = false;
        playerAnimation.Respawn();
    }
}






/*move by acceleration
    if (moveInput.x==0)
        {
            animator.SetBool("isRunning", false);
            animator.speed = 1f;
            return;
        }
        animator.SetBool("isRunning", true);
        animator.speed=Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0f, 1f);
        if (moveInput.x > 0)
        {
            faceDir = 1;
            
        }
        else if(moveInput.x < 0)
        {
            faceDir = -1;
            
        }
        transform.localScale = new Vector3
            (size * faceDir, transform.localScale.y, transform.localScale.z);
        rb.velocity = new Vector2
            (Mathf.Clamp(rb.velocity.x+accelaration*moveInput.x,-maxSpeed,maxSpeed), rb.velocity.y);*/
