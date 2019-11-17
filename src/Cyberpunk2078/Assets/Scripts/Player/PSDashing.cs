﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dashing", menuName = "Player State/Dashing")]
public class PSDashing : PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_dashForce = 8; // Dash Initial speed
    [SerializeField] private float n_dashDelayTime = 0.05f; // time from button press to actual dash
    [SerializeField] private float n_dashReleaseTime = 0.22f; // time to recover rb2d setting after dashing process finished
    [SerializeField] private float n_dashReleaseDelayTime = 0.05f; // time from dash finish to actual controllable
    [SerializeField] private float n_inDashingDragFactor = 3; // In-dash drag factor
    [SerializeField] private float n_JumpListenerInterval = 0.2f;
    
    [Header("Fever")]
    [SerializeField] private float f_dashForce = 8; // Dash Initial speed
    [SerializeField] private float f_dashDelayTime = 0.05f; // time from button press to actual dash
    [SerializeField] private float f_dashReleaseTime = 0.22f; // time to recover rb2d setting after dashing process finished
    [SerializeField] private float f_dashReleaseDelayTime = 0.05f; // time from dash finish to actual controllable
    [SerializeField] private float f_inDashingDragFactor = 3; // In-dash drag factor
    [SerializeField] private float f_JumpListenerInterval = 0.2f;
    
    [Header( "Common" )]

    [SerializeField] private float EnergyConsume = -70;
    
    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexWallJumping;
    [SerializeField] private int indexPSAirborne;

    private float lastDashSecond;
    private bool hyperSpeed;
    private float defaultDrag;
    private bool readyToPush;
    private bool Apply = true;
    private float lastJumpInput;
    

    public override int Update()
    {
        
     
        var dashDelayTime = Player.CurrentPlayer.Fever?f_dashDelayTime:n_dashDelayTime; 
        var dashReleaseTime = Player.CurrentPlayer.Fever?f_dashReleaseTime:n_dashReleaseTime; 
        var dashReleaseDelayTime = Player.CurrentPlayer.Fever?f_dashReleaseTime:n_dashReleaseTime;
        var JumpListenerInterval = Player.CurrentPlayer.Fever?f_JumpListenerInterval:n_JumpListenerInterval;
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");


        // Energy Cost
        if (Player.CurrentPlayer.Fever)
        {
            Player.CurrentPlayer.CostFeverEnergy(Time.time);
        }
        
        if (!Apply)
        {
            if (!isGrounded())
            {
                return indexPSAirborne;
            }
            
            if (h == 0) return indexPSIdle;
            return indexPSMoving;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (Player.CurrentPlayer.secondJumpReady && lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.time)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                return indexPSJumping1;
            }
            else
            {
                // save jump for later
                lastJumpInput = Time.time;
            }
        }

        if (lastDashSecond + dashDelayTime < Time.time && readyToPush)
        {
            // Press delay ends. time to actual dash
            readyToPush = false;
            forceApply();
        }
        
        
        if (lastDashSecond + dashReleaseTime + dashDelayTime < Time.time)
        {
            // the dash has already ended
            if (hyperSpeed)
            {
                // kill speed after dash
                hyperSpeed = false;
                rb2d.drag = defaultDrag;
                rb2d.velocity = rb2d.velocity * 0.1f;
            }
            PhysicsInputHelper(h);
            setAtkBox(false);
            
        } else if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.time)
        {
            //cast listened jump right after dash finish
            if (lastJumpInput + JumpListenerInterval > Time.time)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                    return indexPSJumping1;
            }
            
            // the dash has already ended
            // ok for move input
            PhysicsInputHelper(h);
            setAtkBox(false);
            
            //enable Collision
            playerCharacter.transform.right = Vector3.right;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"), false);
        }
        
        //prevent ground-hitting shifting 
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,1f);
        if (hit1.collider != null)
        {
            if(hit1.transform.CompareTag("Ground")){
                rb2d.velocity = rb2d.velocity * 0f;
                PhysicsInputHelper(h);
                if (!isGrounded())
                {
                    return indexPSAirborne;
                }

                if (h == 0) return indexPSIdle;
                return indexPSMoving;
            }
            else if(hit1.transform.CompareTag("Platform") && rb2d.velocity.normalized.y < 0)
            {
                //upper ward
                rb2d.velocity = rb2d.velocity * 0f;
                PhysicsInputHelper(h);
                if (!isGrounded())
                {
                    return indexPSAirborne;
                }

                if (h == 0) return indexPSIdle;
                return indexPSMoving;
            }
        }
            
        
        // Player is grounded and dash has finished
        if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime  < Time.time)
        {
            rb2d.velocity = rb2d.velocity * 0f;
            PhysicsInputHelper(h);
            if (!isGrounded())
            {
                return indexPSAirborne;
            }

            if (h == 0) return indexPSIdle;
            return indexPSMoving;
        }
        
        
        return Index;
    }


    public override void OnStateEnter(State previousState)
    {
        
        var inDashingDragFactor = Player.CurrentPlayer.Fever?f_inDashingDragFactor:n_inDashingDragFactor; 
        if (!Player.CurrentPlayer.CostEnergy(EnergyConsume))
        {
            // Energy is not enough, Cancel dash
            Apply = false;
            return;
        }
        
        Apply = true;
        //Dash has been pressed, set all config first
        //After delay is over, dash perform
        
        // Add Ghost trail
        if(!Player.CurrentPlayer.Fever)playerCharacter.GetComponent<GhostSprites>().Occupied = true;
        lastDashSecond = Time.time;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        // Record the default Drag of rb2d
        defaultDrag = rb2d.drag;
        readyToPush = true;
        
        // Kill all initial speed
        rb2d.velocity = new Vector2(0,0);
        hyperSpeed = true;
        
        // Kill gravity 
        rb2d.gravityScale = 0;
        
        //Disable Collision
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"));
        
        // Strong Drag during dashing
        rb2d.drag *= inDashingDragFactor;
        
    }


    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"),false);
        
        // reset drag & gravity 
        rb2d.drag = 1;
        rb2d.gravityScale = 3;
        
        // Listening to move input
        PhysicsInputHelper(h);
        
        // reset player facing
        //playerCharacter.GetComponent<CapsuleCollider2D>().isTrigger = false;
        playerCharacter.transform.right = Vector3.right;
            
        // reset sprite flip
        playerCharacter.GetComponent<SpriteRenderer>().flipY = false;
            
        // Kill Trail
        if(!Player.CurrentPlayer.Fever)playerCharacter.GetComponent<GhostSprites>().Occupied = false;
        
        setAtkBox(false);
        
        

    }

    private void forceApply()
    {
        
        var dashForce = Player.CurrentPlayer.Fever?f_dashForce:n_dashForce; 
        
        // Fix sprite flip on X-axis
        playerCharacter.GetComponent<SpriteRenderer>().flipX = false;
        
        // Play Animation
        anim.Play("MainCharacter_Dashing", -1, 0f);
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        
      
        
        //get Mouse direction
        Vector3 direction = getDirectionCorrection(mouse.getAttackDirection(),GroundNormal());
        
        Debug.Log(direction);
        
        var VFX = ObjectRecycler.Singleton.GetObject<SingleEffect>(6);
        VFX.transform.position = playerCharacter.transform.position +  direction * 0.5f;
        VFX.setTarget(playerCharacter.transform);
        
        VFX.transform.right = direction;
        VFX.transform.localScale = new Vector3(4,4,1);
        VFX.gameObject.SetActive(true);
        
        
        
        var trail = ObjectRecycler.Singleton.GetObject<SingleEffect>(7);
        trail.transform.position = playerCharacter.transform.position;
        trail.setTarget(playerCharacter.transform);
        
        trail.transform.right = -direction;
        trail.transform.localScale = new Vector3(7,1,1);
        trail.gameObject.SetActive(true);

        //set correct Y flip based on mouse direction
        if (direction.x < 0 && direction.y != 0) playerCharacter.GetComponent<SpriteRenderer>().flipY = true;


        
        //Apply force to character
       // playerCharacter.GetComponent<CapsuleCollider2D>().isTrigger = true;
        playerCharacter.transform.right = direction; 
        rb2d.AddForce(direction * dashForce * 200f * 1 / Time.timeScale);
        
        //Camera Tricks
        CameraManager.Instance.Shaking(0.04f,0.08f);

        setAtkBox(true);
    }

    private void setAtkBox(bool value)
    {
        playerCharacter.dashAtkBox.SetActive(value);
    }

    
}
