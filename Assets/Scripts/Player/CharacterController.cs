using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [HideInInspector] public bool CanMove=true;
    [SerializeField]
    private float sidewaysForce, sidewaysForceInAir, MaxSidewaysSpeed, maxAirVelocity, jumpForce, jumpForceWall, jumpForceWallAway, wallPerpendicular, wallDownwardsSpeedMax,wallDownwardsSpeedMaxHoldingDown;
    private float jumpForceWallAwayLocal;
    [SerializeField]
    private bool canTurnInAir;
    private bool pastApex;
    private float checkBelow = 0.03f;
    private float checkSide= 0.03f;

    private bool onWall,jumped,wasJustOnWall; 
    [HideInInspector] public bool onGround;
    [HideInInspector] public bool LookingLeft,LookingRight;
    public Animator[] animators;
    private Rigidbody2D rb2D;
    private bool wallOnLeft, wallOnRight=false;
    private bool countJumpTime=false, DelayJump=false, DelayWallJump=false;
    private float jumpTimer, jumpTime=0.3f, delayJumpTimer;
    [SerializeField] private float ghostJumpDelay;
    private float horizontalInput = 0, verticalInput = 0;
    private managePlayerAudio audioMngr;
    [SerializeField] private AudioClip jumpAudio,landAudio,inAirAudio;
    private bool landed=false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        checkSide=checkSide*transform.localScale.x;
        checkBelow = checkBelow*transform.localScale.x;
        audioMngr=GetComponent<managePlayerAudio>();
    }

    public void OnMove(InputAction.CallbackContext context){
        horizontalInput = context.ReadValue<float>();
    }
    public void OnJump(InputAction.CallbackContext context){
        if(CanMove){
            if(context.started){
                if(onGround == true || ((pastApex && onWall) || DelayJump || DelayWallJump)){
                    if(countJumpTime == false){
                        if(rb2D.velocity.y < maxAirVelocity){
                            if(!jumped){
                                jumped=true;
                                Vector2 jf = (onWall||DelayWallJump)&&!onGround?new Vector2(jumpForceWallAwayLocal,jumpForceWall):new Vector2(0,jumpForce);
                                rb2D.AddForce(jf);
                                pastApex = !onGround;
                                jumpTimer=0;
                                DelayJump=false;
                                DelayWallJump=false;
                                countJumpTime=true;
                                // animator.SetBool("PastJumpApex",!onWall);
                                updateAnimators(true,!onWall,"PastJumpApex",0);
                                audioMngr.SetAndPlaySound(jumpAudio,0.5f,0.4f,0.1f);
                                landed=false;
                            }
                        }
                    }
                }
            }
            if(context.canceled){
                if(!onWall && !onGround)
                {
                    if(!pastApex){
                        rb2D.velocity = new Vector2(rb2D.velocity.x,0);
                    }
                }
            }
        }
    }
    public void OnDuck(InputAction.CallbackContext context){
        verticalInput = context.ReadValue<float>();
    }

    void FixedUpdate()
    {
        if(countJumpTime && jumpTimer<=jumpTime){
            jumpTimer+=Time.deltaTime;
        }else{
            countJumpTime=false;
        }
        if(DelayJump || DelayWallJump){
            if(delayJumpTimer >=ghostJumpDelay){
                DelayJump=false;
                DelayWallJump=false;
            }
            delayJumpTimer+=Time.deltaTime;
        }
        if(CanMove){
            //Movement
            if(countJumpTime == false){
                if(rb2D.velocity.y < maxAirVelocity){
                    if(jumped){
                        jumped=false;
                    }
                }
            }
            if(onGround == true){
                SidewaysMovement(sidewaysForce);
                DelayWallJump=false;
            }else{
                if(canTurnInAir){
                    SidewaysMovement(sidewaysForceInAir);
                    //limit air movement
                    float speed = Vector3.Magnitude (rb2D.velocity);  // test current object speed
                    if(speed > maxAirVelocity){
                        float brakeSpeed = speed - maxAirVelocity;  // calculate the speed decrease
                        Vector3 normalisedVelocity = rb2D.velocity.normalized;
                        Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
                        rb2D.AddForce(-brakeVelocity);  // apply opposing brake force
                    }
                }
                if(pastApex){
                    if(onWall){
                        //wall hang upwards force
                        if(rb2D.velocity.y<=0){
                            wasJustOnWall=true;
                            if(rb2D.velocity.y<=-wallDownwardsSpeedMax)
                            {
                                float tempDownwardsMax = wallDownwardsSpeedMax;
                                if(verticalInput > 0.2f){
                                    tempDownwardsMax = wallDownwardsSpeedMaxHoldingDown;
                                }
                                float brakeSpeed = rb2D.velocity.y + tempDownwardsMax;  // calculate the speed decrease
                                Vector3 brakeVelocity = Vector2.down * -brakeSpeed/2 * rb2D.mass * rb2D.gravityScale;  // make the brake Vector3 value                            
                                rb2D.AddForce(-brakeVelocity);  // apply opposing brake force
                            }
                            if(wallOnLeft &&!wallOnRight){
                                rb2D.AddForce(Vector2.left*wallPerpendicular);
                            }
                            if(wallOnRight&& !wallOnLeft){
                                rb2D.AddForce(Vector2.right*wallPerpendicular);
                            }
                        }
                    }else{
                        //gap in slide, move in if pressing horizontal input
                        if(!jumped){
                            if(wasJustOnWall){
                                wasJustOnWall=false;
                                float directionalH = Mathf.Round(horizontalInput+0.1f);
                                if(Mathf.Abs(directionalH)==1){
                                    Vector2 bounds1 = GetComponent<CapsuleCollider2D>().bounds.extents;
                                    float edgeOffset1 = -bounds1.y;
                                    RaycastHit2D belowInDirection = Physics2D.Raycast(transform.position + new Vector3(bounds1.x* directionalH, edgeOffset1, 0), Vector2.right * directionalH, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
                                    if(belowInDirection.collider!=null){
                                        rb2D.velocity=new Vector2(rb2D.velocity.x + directionalH*2,5);
                                    }
                                }
                            }
                        }
                    }
                }else{
                    if(rb2D.velocity.y<=0){
                        pastApex=true;
                        // audioMngr.SetAndPlaySound(inAirAudio,0,0);
                        // animator.SetBool("PastJumpApex",true);
                        updateAnimators(true,true,"PastJumpApex",0);
                    }
                    else{
                        // animator.SetBool("PastJumpApex",false);
                        updateAnimators(true,false,"PastJumpApex",0);
                    }
                }
            }
        }
        Vector2 bounds = GetComponent<CapsuleCollider2D>().bounds.extents;
        float edgeOffset = 0.04f;
        //__________Jumping and ground check
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(bounds.x,bounds.y/2,0), Vector2.down, checkBelow*3.75f, 1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(-bounds.x,bounds.y/2,0), Vector2.down, checkBelow*3.75f, 1 <<LayerMask.NameToLayer("Terrain"));
        // Debug.DrawRay(transform.position + new Vector3(bounds.x,edgeOffset/2,0), Vector2.down*checkBelow*2, Color.yellow);
        // Debug.DrawRay(transform.position+ new Vector3(-bounds.x,edgeOffset/2,0), Vector2.down*checkBelow*2, Color.yellow);
        Debug.DrawRay(transform.position + new Vector3(bounds.x,bounds.y/2,0), Vector2.down*checkBelow*3.75f, Color.yellow);
        Debug.DrawRay(transform.position+ new Vector3(-bounds.x,bounds.y/2,0), Vector2.down*checkBelow*3.75f, Color.yellow);

        if (hit.collider != null || hit1.collider != null)
        {
            onGround = true;
            if(!landed){
                landed=true;
                // audioMngr.SetAndPlaySound(landAudio,0.3f,0);
            }
            // animator.SetBool("OnGround",true);
            updateAnimators(true,true,"OnGround",0);
            delayJumpTimer=0;
            if(!jumped){
                if(delayJumpTimer <=ghostJumpDelay){
                    DelayJump = true;
                }
            }
        }else{
            onGround = false;
            // animator.SetBool("OnGround",false);
            updateAnimators(true,false,"OnGround",0);
        }
        //___________Wall Climbing
        RaycastHit2D hitLeftTop = Physics2D.Raycast(transform.position + new Vector3(-bounds.x, bounds.y, 0), Vector2.left, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitLeftTippyTop = Physics2D.Raycast(transform.position + new Vector3(-bounds.x, bounds.y*2-edgeOffset, 0), Vector2.left, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitLeftBottom = Physics2D.Raycast(transform.position + new Vector3(-bounds.x, edgeOffset, 0), Vector2.left, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitRightTop = Physics2D.Raycast(transform.position + new Vector3(bounds.x, bounds.y, 0), Vector2.right, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitRightTippyTop = Physics2D.Raycast(transform.position + new Vector3(bounds.x, bounds.y*2-edgeOffset, 0), Vector2.right, checkSide,1 <<LayerMask.NameToLayer("Terrain"));
        RaycastHit2D hitRightBottom = Physics2D.Raycast(transform.position + new Vector3(bounds.x, edgeOffset, 0), Vector2.right, checkSide,1 <<LayerMask.NameToLayer("Terrain"));

        Debug.DrawRay(transform.position + new Vector3(-bounds.x, bounds.y, 0), Vector2.left*checkSide, Color.red);
        Debug.DrawRay(transform.position + new Vector3(-bounds.x, bounds.y*2-edgeOffset, 0), Vector2.left*checkSide, Color.red);
        Debug.DrawRay(transform.position + new Vector3(-bounds.x, edgeOffset, 0), Vector2.left*checkSide, Color.red);
        Debug.DrawRay(transform.position + new Vector3(bounds.x, bounds.y, 0), Vector2.right*checkSide, Color.red);
        Debug.DrawRay(transform.position + new Vector3(bounds.x, bounds.y*2-edgeOffset, 0), Vector2.right*checkSide, Color.red);
        Debug.DrawRay(transform.position + new Vector3(bounds.x, edgeOffset, 0), Vector2.right*checkSide, Color.red);


        if(hitLeftTippyTop.collider != null||hitRightTippyTop.collider != null||hitLeftTop.collider != null || hitLeftBottom.collider != null || hitRightTop.collider != null || hitRightBottom.collider != null){
            onWall = true;
            DelayWallJump = true;
            delayJumpTimer = ghostJumpDelay/2;
            // animator.SetBool("AgainstWall",true);
            updateAnimators(true,true,"AgainstWall",0);
            if(hitLeftTop.collider != null || hitLeftTippyTop.collider != null || hitLeftBottom.collider!=null){
                wallOnLeft = true;
                wallOnRight =false;
                jumpForceWallAwayLocal = jumpForceWallAway;
            }else{
                wallOnLeft = false;
                wallOnRight = true;
                jumpForceWallAwayLocal = -jumpForceWallAway;
            }
            rb2D.velocity -= new Vector2 (rb2D.velocity.x,0); 
        }else{
            onWall = false;
            // animator.SetBool("AgainstWall",false);
            updateAnimators(true,false,"AgainstWall",0);
            wallOnLeft = false;
            wallOnRight = false;
        }
    }

    private void SidewaysMovement(float f){
        if(Mathf.Abs(rb2D.velocity.x) < MaxSidewaysSpeed || (rb2D.velocity.x < 0 && horizontalInput > 0) || (rb2D.velocity.x > 0 && horizontalInput < 0))
        {
            if(Mathf.Abs(horizontalInput)>0.3f){
                if((wallOnLeft && horizontalInput > 0.3f) || (wallOnRight && horizontalInput < 0.3f) || (!wallOnRight && !wallOnLeft)){
                    rb2D.AddForce(new Vector2(f * horizontalInput,0)*Time.deltaTime);
                }
            }
            LookingLeft = horizontalInput < -0.3f;
            LookingRight = horizontalInput > 0.3f;
        }else{
            if(onGround){
                float brakeSpeed = rb2D.velocity.x + MaxSidewaysSpeed;  // calculate the speed decrease
                Vector3 brakeVelocity = Vector2.right* -horizontalInput * brakeSpeed;  // make the brake Vector3 value                            
                rb2D.AddForce(brakeVelocity);  // apply opposing brake force
            }
        }

        if(wallOnLeft){
            transform.GetComponentInChildren<SpriteRenderer>().flipX = false;
        }else
        if(wallOnRight){
            transform.GetComponentInChildren<SpriteRenderer>().flipX = true;
        }else{
            if(Mathf.Abs(horizontalInput) > 0.3f){// || Mathf.Abs(rb2D.velocity.x)>0.3f){
                transform.GetComponentInChildren<SpriteRenderer>().flipX = rb2D.velocity.x>0f;
            }
        }
        // animator.SetFloat("Speed",Mathf.Abs(rb2D.velocity.x));
        updateAnimators(false,false,"Speed",Mathf.Abs(rb2D.velocity.x));
    }

    private void updateAnimators(bool boolOrNo, bool boolVal, string WhatToSet, float Value){
        for(int i=0; i<animators.Length;i++){
            if(boolOrNo){
                animators[i].SetBool(WhatToSet,boolVal);
            }else{
                animators[i].SetFloat(WhatToSet,Value);
            }
        }
    }
    public void DeathHide(){
        for(int i=0; i<animators.Length;i++){
            animators[i].gameObject.SetActive(false);
        }
        GetComponent<CapsuleCollider2D>().enabled=false;
        GetComponent<AimController>().stopShooting();
        CanMove=false;
    }
}



