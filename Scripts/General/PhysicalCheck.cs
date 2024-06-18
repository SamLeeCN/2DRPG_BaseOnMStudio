using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//创建人： SamLee 
//功能说明：
//***************************************** 
public class PhysicalCheck : MonoBehaviour
{
    [Header("Reference")]
    public LayerMask groundLayer;
    public LayerMask waterLayer;
    public Animator animator;
    public Collider2D coll;
    public PlayerCharacterController playerController;
    public Rigidbody2D rb;

    [Header("Settings")]
    public bool isManul;
    public bool isPlayer;

    [Header("Properties")]
    public float detectingRadius;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public Vector2 leftBottomOffset;
    public Vector2 rightBottomOffset;
    public Vector2 topOffset;
    public float regularDrag;
    public float waterDrag;

    [Header("Status")]
    public bool isGround;
    public bool isLeftGround;
    public bool isRightGround;
    public bool isTouchingLeftWall;
    public bool isTouchingRightWall;
    public bool isTouchingTop;
    public bool isOnWall;
    public bool isInWater;
    public bool canWaterJump;

    [Header("Boyance")]
    public float depthBeforeSubmerged;
    public float displacementAmount;
    public float bouyanceScale;
    public WaterManager currentWater;

    void Start()
    {
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb=GetComponent<Rigidbody2D>();
        //initialize the position of judgement point
        if (!isManul)
        {
            detectingRadius = 0.2f;
            bottomOffset = new Vector2(coll.offset.x,coll.offset.y - coll.bounds.size.y/2f);
            leftOffset = new Vector2(coll.offset.x - coll.bounds.size.x/2,coll.offset.y);
            rightOffset = new Vector2(coll.offset.x + coll.bounds.size.x / 2, coll.offset.y);
            leftBottomOffset= new Vector2(coll.offset.x - coll.bounds.size.x / 2, coll.offset.y - coll.bounds.size.y / 2f);
            rightBottomOffset= new Vector2(coll.offset.x + coll.bounds.size.x / 2, coll.offset.y - coll.bounds.size.y / 2f);
            topOffset = new Vector2(coll.offset.x, coll.offset.y + coll.bounds.size.y / 2f);
            depthBeforeSubmerged=coll.bounds.size.y;
        }
        if (isPlayer)
        {
            playerController=GetComponent<PlayerCharacterController>();
        }
    }
    private void FixedUpdate()
    {
        Buoyancy();
    }
    void Update()
    {
        Check();
    } 
    void Check()
    {
        isGround = Physics2D.OverlapCircle
            ((Vector2)transform.position + bottomOffset, detectingRadius, groundLayer);
        isTouchingLeftWall=Physics2D.OverlapCircle
            ((Vector2)transform.position + leftOffset, detectingRadius, groundLayer);
        isTouchingRightWall=Physics2D.OverlapCircle
            ((Vector2)transform.position + rightOffset, detectingRadius, groundLayer);
        isLeftGround=Physics2D.OverlapCircle
            ((Vector2)transform.position +leftBottomOffset, detectingRadius, groundLayer);
        isRightGround= Physics2D.OverlapCircle
            ((Vector2)transform.position + rightBottomOffset, detectingRadius, groundLayer);
        isTouchingTop= Physics2D.OverlapCircle
            ((Vector2)transform.position + topOffset, detectingRadius, groundLayer);
        if (isPlayer)
        {
            isOnWall = ((isTouchingLeftWall &&playerController.moveInput.x<0)
                ||(isTouchingRightWall && playerController.moveInput.x >0)) 
                && !isGround && (rb.velocity.y < 0);
        }
        canWaterJump = Physics2D.OverlapCircle
              ((Vector2)transform.position + bottomOffset, 2*detectingRadius, waterLayer)
              &&!Physics2D.OverlapCircle
            ((Vector2)transform.position + topOffset, detectingRadius, waterLayer);
    }
   public void Buoyancy()
    {
        if (!isInWater) return;       
        float waveHeight = currentWater.GetWaveHeight(transform.position.x);
        float displaceMultiplier=Mathf.Clamp01(((waveHeight-transform.position.y)/depthBeforeSubmerged)*displacementAmount);
        rb.AddForce(new Vector2(0f, displaceMultiplier*Mathf.Abs(Physics.gravity.y))*rb.mass*rb.gravityScale*bouyanceScale,ForceMode2D.Force);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position+bottomOffset, detectingRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, detectingRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, detectingRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftBottomOffset, detectingRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightBottomOffset, detectingRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + topOffset, detectingRadius);
    }
}
