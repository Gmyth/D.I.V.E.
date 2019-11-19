﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_WallJumping", menuName = "Player State/Wall Jumping")]
public class PSWallJumping: PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_jumpForce = 4.5f;
    [SerializeField] private float n_jumpSpeed = 10f;
    [SerializeField] private float n_stickWallTime = 0.25f;
    [SerializeField] private float n_wallCheckCoolDown = 0.25f;
    [Header("Fever")]
    [SerializeField] private float f_jumpForce = 4.5f;
    [SerializeField] private float f_jumpSpeed = 10f;
    [SerializeField] private float f_stickWallTime = 0.25f;
    [SerializeField] private float f_wallCheckCoolDown = 0.25f;
    
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_PSJumping1;
    [SerializeField] private int index_PSJumping2;
    [SerializeField] private int index_PSDashing;
    [SerializeField] private int index_PSAttackGH;
    [SerializeField] private int index_PSAirborne;
    [SerializeField] private int index_PSClimb;
    private bool isJumpKeyDown = false;

    private bool onWall = false;
    private float gravityHolder;
    private float lastStickTime;
    private float lastOnWallTime;
    public override int Update()
    {
        var jumpSpeed = n_jumpSpeed;
        var stickWallTime = n_stickWallTime;
        var wallCheckCoolDown = n_wallCheckCoolDown;

        if (playerCharacter.IsInFeverMode)
        {
            jumpSpeed = f_jumpSpeed;
            stickWallTime = f_stickWallTime;
            wallCheckCoolDown = f_wallCheckCoolDown;
        }


        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
//        if (onWall && Time.time > lastStickTime + stickWallTime)
//        {
//            //stick over, falling start
//            rb2d.gravityScale = 3;
//        }
        
        
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        
        if (isGrounded() && Vy <= 0)
        {
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return index_PSAttackGH;
        }

        var dir = isCloseTo("Ground");
        if (onWall)
        {
            //Sticked , ok to perform wall jump again
            if (Input.GetButtonDown("Jump"))
            {
                onWall = false;
                lastOnWallTime = Time.unscaledTime;
                //re active jumping
                if (dir == Direction.Right)
                {
                    flip = true;
                    Player.CurrentPlayer.ChainWallJumpReady = true;
                    rb2d.velocity = new Vector2(-jumpSpeed * 1.3f, 0);
                }
                else
                {
                    flip = false; 
                    Player.CurrentPlayer.ChainWallJumpReady = true; 
                    rb2d.velocity = new Vector2(jumpSpeed*1.3f, 0);
                }

                return index_PSJumping1;
            }
        }
       
        
        if ( dir ==  Direction.Right && h > 0) { rb2d.gravityScale = 1.2f;}
        else if (dir ==  Direction.Left && h < 0) {  rb2d.gravityScale = 1.2f; }
        else{rb2d.gravityScale = 3f;}
        if (dir == Direction.None && !isGrounded())
        {
            return index_PSAirborne;
        }


        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return index_PSClimb;
        }
        // flip sprite for correct facing 
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;
        
        // perform Dashing
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return index_PSDashing;
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        var dir = isCloseTo("Ground");
        // Set flip
        flip = dir == Direction.Left;
        
        //kill speed
        onWall = true;
        
        // Animation
        anim.Play("MainCharacter_WallJump", -1, 0f);

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        rb2d. velocity = Vector2.zero;

        rb2d.gravityScale = 1.8f;
        
        lastStickTime = Time.time;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
        onWall = false;
    }

    private void sideWallJump()
    {
        var jumpForce = playerCharacter.IsInFeverMode ? f_jumpForce: n_jumpForce;
        
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
