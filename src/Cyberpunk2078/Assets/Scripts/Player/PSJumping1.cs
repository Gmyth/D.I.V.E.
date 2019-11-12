﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [Header( "Normal" )]
    [SerializeField] private float n_jumpForce = 8;
    [SerializeField] private float n_wallJumpForce = 8;
    [SerializeField] private float n_speedFactor = 3;
    [SerializeField] private float n_accelerationFactor = 20;
    [SerializeField] private float n_wallJumpCD;
    
    [Header( "Fever" )]
    [SerializeField] private float f_jumpForce = 8;
    [SerializeField] private float f_wallJumpForce = 8;
    [SerializeField] private float f_speedFactor = 3;
    [SerializeField] private float f_accelerationFactor = 20;
    [SerializeField] private float f_wallJumpCD;
    
    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexJumping2;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    [SerializeField] private float jumpIncreaser = 1.5f;
    [SerializeField] private float jumpIncreaserThreshold = 1.5f;
    private float lastJumpSec;
    private State previous;
    private float timer;

    public override int Update()
    {
        
        var jumpForce =  n_jumpForce;
        var speedFactor = n_speedFactor;
        var accelerationFactor = n_accelerationFactor;
        var wallJumpCD = n_wallJumpCD;
        if (Player.CurrentPlayer.Fever)
        {
            jumpForce =  f_jumpForce;
            speedFactor = f_speedFactor; 
            accelerationFactor = f_accelerationFactor;
            wallJumpCD = f_wallJumpCD;
        }
        
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        flip = h < 0;
        
        // Energy Cost
        if (Player.CurrentPlayer.Fever)
        {
            Player.CurrentPlayer.CostFeverEnergy(Time.time);
        }

        //Still support Horizontal update during jumping, delete following to kill Horizontal input
         PhysicsInputHelper(h,speedFactor,accelerationFactor);
         
         var dir = isCloseTo("Ground");
         if (dir != Direction.None )
         {
             if (dir == Direction.Right &&  h > 0 )
             {
                 if (previous.Index == indexPSWallJumping && Time.time > lastJumpSec + wallJumpCD/4 && !Player.CurrentPlayer.lastWallJumpRight)
                 {
                     return indexPSWallJumping; 
                 }
                 else if (Time.time > lastJumpSec + wallJumpCD)
                 {
                     return indexPSWallJumping; 
                 }


             }
             else if(dir == Direction.Left  && h < 0)
             {
                 if (previous.Index == indexPSWallJumping &&  Player.CurrentPlayer.lastWallJumpRight && Time.time > lastJumpSec + wallJumpCD/4)
                 {
                     return indexPSWallJumping; 
                 }
                 else if (Time.time > lastJumpSec + wallJumpCD)
                 {
                     return indexPSWallJumping; 
                 }
             }
         }
         
         
         if (Input.GetAxis("Vertical") > 0 && isCloseTo("Ladder") != Direction.None)
        {
            // up is pressed
            return indexPSClimb;
        }

        if (!isGrounded()&& Vy < 0)
        {
                return indexPSAirborne;
        }
       else if (previous.Index != indexPSWallJumping)
       {
            if (Input.GetButton("Jump"))
            {
                timer -= Time.deltaTime;
                if (timer > 0)
                {                    
                    Debug.Log(timer);
                    rb2d.AddForce(playerCharacter.transform.up * jumpForce * jumpIncreaser);
                }
                
            }
        }

        if (isGrounded()&& Vy < 0)
        {
            // Landed
            if (h == 0)
                return indexPSIdle;

            return indexPSMoving;
        }

        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }

        //Player is sill in air
        //if (lastJumpSec + jumpCD < Time.time)
        //{
        //    // prevent misclicking
        //    if (Input.GetButtonDown("Jump"))
        //    {
        //        // second_jump
        //        Player.CurrentPlayer.secondJumpReady = false;
        //        return indexJumping2;
        //    }
               
        //}

        if (Input.GetButtonDown("Dashing") || Input.GetButtonDown("Trigger"))
            return indexPSDashing;
        
        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        var jumpForce = Player.CurrentPlayer.Fever?f_jumpForce : n_jumpForce;
        var wallJumpForce = Player.CurrentPlayer.Fever? f_wallJumpForce : n_wallJumpForce;
        
        previous = previousState;
        timer = jumpIncreaserThreshold;
        anim.Play("MainCharacter_Jump", -1, 0f);
        //Perform jump
        lastJumpSec = Time.time;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        if (previousState.Index == indexPSWallJumping)
        {
            rb2d.AddForce(playerCharacter.transform.up * wallJumpForce * 100);
        }
        else if (Player.CurrentPlayer.jumpForceGate) {
            // skip the force add
            Player.CurrentPlayer.jumpForceGate = false;
        }
        else
        {
            rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
        }

       
    }
}
