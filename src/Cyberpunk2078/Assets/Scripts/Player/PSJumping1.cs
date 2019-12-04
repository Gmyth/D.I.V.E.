﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [Header( "Normal" )]
  
    [SerializeField] private float n_jumpForce = 8;
    [SerializeField] private float n_speedFactor = 3;
    [SerializeField] private float n_accelerationFactor = 20;
    [SerializeField] private float n_wallJumpCD;
    [SerializeField] private float n_wallJumpForce = 4.5f;
    [SerializeField] private float n_wallJumpSpeed = 10f;
    
    [Header( "Fever" )]
    [SerializeField] private float f_jumpForce = 8;
    [SerializeField] private float f_speedFactor = 3;
    [SerializeField] private float f_accelerationFactor = 20;
    [SerializeField] private float f_wallJumpCD;
    [SerializeField] private float f_wallJumpForce = 4.5f;
    [SerializeField] private float f_wallJumpSpeed = 10f;
    
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
        var wallJumpSpeed = n_wallJumpSpeed;

        if (playerCharacter.IsInFeverMode)
        {
            jumpForce =  f_jumpForce;
            speedFactor = f_speedFactor; 
            accelerationFactor = f_accelerationFactor;
            wallJumpSpeed = f_wallJumpSpeed;
        }
        
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        float v = Input.GetAxis("VerticalJoyStick");
        Vector2 normalizedInput = new Vector2(h, v).normalized;
        flip = h < 0;
        

        //Still support Horizontal update during jumping, delete following to kill Horizontal input
            if(lastJumpSec + 0.2f < Time.time) PhysicsInputHelper(h,speedFactor,accelerationFactor);
         
            var dir = isCloseTo("Ground");
            if (dir != Direction.None)
            {
                if (lastJumpSec + 0.2f < Time.time && Player.CurrentPlayer.ChainWallJumpReady)
                {
                    return indexPSWallJumping;
                }
         }


        bool ground = GetGroundType() == 1;

        if (Input.GetAxis("Vertical") > 0 || normalizedInput.y > 0.7f)
            {
                // up is pressed
                if(isCloseTo("Ladder") != Direction.None) return indexPSClimb;
            }

        if (!ground && Vy < 0)
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
                    rb2d.AddForce(playerCharacter.transform.up * jumpForce * jumpIncreaser);
                }
            }
        }


        if (ground && Vy < 0)
        {
            // Landed
            if (h == 0)
                return indexPSIdle;

            return indexPSMoving;
        }

        if (Input.GetButtonDown("Attack1"))
        {
            return indexPSAttackGH;
        }

        if (Input.GetButtonDown("Jump") && isCloseTo("Ground") != Direction.None)
        {
            performWallJump();
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
        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return indexPSDashing;
        }
        
        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        var jumpForce = playerCharacter.IsInFeverMode ? f_jumpForce : n_jumpForce;

        
        playerCharacter.groundDust.transform.localPosition = new Vector3(0,-0.5f,0);
        playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();
        
        previous = previousState;
        timer = jumpIncreaserThreshold;
        anim.Play("MainCharacter_Jump", -1, 0f);
        //Perform jump
        lastJumpSec = Time.time;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Vertical Speed
        if (previousState.Index == indexPSWallJumping && !Player.CurrentPlayer.jumpForceGate)
        {
            performWallJump();
        }
        else if (Player.CurrentPlayer.jumpForceGate) {
            // skip the force add
            Player.CurrentPlayer.jumpForceGate = false;
        }
        else
        {
            rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
        }
        
        //VFX 
        var Dust = ObjectRecycler.Singleton.GetObject<SingleEffect>(11);
        Dust.transform.position = playerCharacter.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 3f);
            
        Dust.transform.right =-(new Vector2(rb2d.velocity.x,25f).normalized);

        Dust.gameObject.SetActive(true);
        Dust.GetComponentInChildren<Animator>().speed = 1.5f;
       
    }
    
    public override void OnStateQuit(State nextState)
    {
        playerCharacter.groundDust.transform.localPosition = Vector3.zero;
        playerCharacter.groundDust.GetComponent<ParticleSystem>().Stop();
    }

    private void performWallJump()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var wallJumpForce = playerCharacter.IsInFeverMode ? f_wallJumpForce : n_wallJumpForce;
        var wallJumpSpeed = playerCharacter.IsInFeverMode ? f_wallJumpSpeed : n_wallJumpSpeed;
        var dir = isCloseTo("Ground");
        //re active jumping
        if (dir == Direction.Right)
        {
            flip = true;
            Player.CurrentPlayer.ChainWallJumpReady = true;
            rb2d.velocity = new Vector2(-wallJumpForce * 1.3f, 0);
        }
        else
        {
            flip = false; 
            Player.CurrentPlayer.ChainWallJumpReady = true; 
            rb2d.velocity = new Vector2(wallJumpSpeed*1.3f, 0);
        }
            
        rb2d.AddForce(playerCharacter.transform.up * wallJumpForce * 100);
    }
}
