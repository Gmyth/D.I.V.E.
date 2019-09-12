using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenuAttribute(fileName = "PS_WallJumping", menuName = "Player State/Wall Jumping")]

public class PSWallJumping: PlayerState
{
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float stickWallTime = 0.25f;
    [SerializeField] private float wallCheckCoolDown = 0.25f;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_PSJumping2;
    [SerializeField] private int index_PSDashing;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    private bool isJumpKeyDown = false;

    private bool onWall = false;
    private float gravityHolder;
    private float lastStickTime;
    private float lastOnWallTime;
    public override int Update()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        if (onWall && Time.unscaledTime > lastStickTime + stickWallTime)
        {
            //stick over, falling start
            rb2d.gravityScale = 3;
            onWall = false;
        }

        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        
        if (isGrounded() && Vy < 0)
        {
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }


        if (onWall)
        {
            //Sticked , ok to perform wall jump again
            if (Input.GetButtonDown("Jump"))
            {
                onWall = false;
                lastOnWallTime = Time.unscaledTime;
                rb2d.gravityScale = 3;
                //re active jumping
                if (RightSideTest("Ground"))
                {
                    flip = true;
                    rb2d.velocity = new Vector2(-jumpSpeed, 0);
                }
                else
                {
                    flip = false; 
                    rb2d.velocity = new Vector2 (jumpSpeed, 0);
                }
                sideWallJump();
            }
        }
        

        if (!onWall && isCloseTo("Ground") && Time.unscaledTime > lastOnWallTime + wallCheckCoolDown )
        {
            //Not stick yet, ok to perform wall jump again
            if (Input.GetButtonDown("Jump"))
            {
                lastOnWallTime = Time.unscaledTime;
                rb2d.gravityScale = 3;
                
                //Check the wall is on the left or right
                if (RightSideTest("Ground"))
                {
                    //right
                    flip = true;
                    rb2d.velocity = new Vector2(-jumpSpeed, 0);
                }
                else
                {
                    //left
                    flip = false; 
                    rb2d.velocity = new Vector2 (jumpSpeed, 0);
                }
                
                // Perform wall jump
                sideWallJump();
            }else{
                OnStateEnter();
            }
            
        }
        
        
        if (Input.GetAxis("Vertical") > 0 && isCloseTo("Ladder"))
        {
            return indexPSClimb;
        }
        // flip sprite for correct facing 
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        
        // perform Dashing
        if (Input.GetAxis("Dashing") != 0)
            return index_PSDashing;

        // Player is sill in air,  wall jumping + second jump are not allowed!
//        if (Vy < jumpForce / 5)
//        {
//            // prevent misclicking
//            if (Input.GetButtonDown("Jump"))
//                // second_jump
//                return index_Jumping2;
//        }
        
        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter()
    {
        // Set flip
        flip = !RightSideTest("Ground");
        
        //kill speed
        onWall = true;
        
        // Animation
        anim.Play("MainCharacter_WallJump", -1, 0f);

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        lastStickTime = Time.unscaledTime;
    }

    private void sideWallJump()
    {
        //Perform jump
        anim.Play("MainCharacter_Jump", -1, 0f);
        
        // prevent applying force twice
        isJumpKeyDown = true;
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        
        // Add Vertical Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
    

}
