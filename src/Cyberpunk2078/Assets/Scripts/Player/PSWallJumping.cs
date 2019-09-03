﻿using System.Collections;
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


        if (onWall)
        {
            if (Input.GetButtonDown("Jump"))
            {
                onWall = false;
                lastOnWallTime = Time.unscaledTime;
                rb2d.gravityScale = 3;
                //re active jumping
                if (RightSideTest()) rb2d.velocity = new Vector2 (-jumpSpeed, 0);
                else rb2d.velocity = new Vector2 (jumpSpeed, 0);
                sideWallJump();
            }
        }

        if (!onWall && isCloseToWall() && Time.unscaledTime > lastOnWallTime + wallCheckCoolDown )
        {
            //stick again
            if (Input.GetButtonDown("Jump"))
            {
                lastOnWallTime = Time.unscaledTime;
                rb2d.gravityScale = 3;
                //re active jumping
                if (RightSideTest()) rb2d.velocity = new Vector2 (-jumpSpeed, 0);
                else rb2d.velocity = new Vector2 (jumpSpeed, 0);
                sideWallJump();
            }else{
                OnStateEnter();
            }
            
        }
        
        if (Input.GetAxis("Dashing") != 0)
            return index_PSDashing;

        //Player is sill in air
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
        //kill speed
        onWall = true;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        //rb2d.velocity = new Vector2 (0, 0);
        //kill Gravity
        ///rb2d.gravityScale = 0;
        lastStickTime = Time.unscaledTime;
    }

    private void sideWallJump()
    {
        //Perform jump
        isJumpKeyDown = true;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
    

}
