﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Attack_GH", menuName = "Player State/Attack GH")]
public class PSAttackGH: PlayerState
{
    [SerializeField] private float pushForce = 2f;
    [SerializeField] private float recoveryTime = 0.5f;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAirborne;
    
    [SerializeField] private GameObject SplashFX; 
    private float t0 = 0;
    private float defaultDrag;


    public override int Update()
    {
        float h = Input.GetAxis("Horizontal");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        PhysicsInputHelper(h);
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,0.5f);
        if (hit1.collider != null && hit1.transform.CompareTag("Ground"))
        {
            // kill all speed
            rb2d.velocity = new Vector2(0,0);
            
            // reset drag & gravity 
//            rb2d.drag = defaultDrag;
//            rb2d.gravityScale = 3;

            // Landed
            if (h == 0)
                // not moving
            
                return indexPSIdle;
            return indexPSMoving;
        }
        
        if (Time.time - t0 > recoveryTime)
        {
            rb2d.drag = defaultDrag;
            rb2d.gravityScale = 3;
            
            if (!isGrounded()&& Vy < 0)
            {
                return indexPSAirborne;
            }
            
            if (Input.GetAxis("Horizontal") == 0)
                return indexPSIdle;

            return indexPSMoving;
        }

        return Index;
    }

    public override void OnStateEnter()
    {
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        t0 = Time.time;
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        
        //get Mouse direction
        Vector3 direction = getDirectionCorrection(mouse.getAttackDirection(),GroundNormal());
        var obj = Instantiate(SplashFX);
        obj.transform.position = playerCharacter.transform.position;
        obj.transform.right = direction;
        obj.transform.parent = playerCharacter.transform;
        anim.Play("MainCharacter_Atk", -1, 0f);
        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerCharacter.GetComponent<Rigidbody2D>().AddForce(direction * pushForce * 100f);
        playerCharacter.GetComponent<SpriteRenderer>().flipX = direction.x < 0;
        Destroy(obj,0.3f);
        //kill gravity
//        rb2d.gravityScale = 0;
//        defaultDrag = rb2d.drag;
//        rb2d.drag = 0;
        
        //Camera Tricks
        CameraManager.Instance.Shaking(0.04f,0.1f);
    }
}
