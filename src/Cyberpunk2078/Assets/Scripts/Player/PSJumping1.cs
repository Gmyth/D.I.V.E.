using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float wallJumpForce = 8;
    [SerializeField] private float speedFactor = 3;
    [SerializeField] private float accelerationFactor = 20;
    [SerializeField] private float jumpCD = 0.02f;
    [SerializeField] private float wallJumpCD;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexJumping2;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    private float lastJumpSec;
    private State previous;

    public override int Update()
    {
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        flip = h < 0;
        

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

        if (Input.GetButtonDown("Dashing"))
            return indexPSDashing;
        
        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        previous = previousState;
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
