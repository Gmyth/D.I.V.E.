using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [SerializeField] private float jumpForce = 8;

    [SerializeField] private float speedFactor = 3;
    [SerializeField] private float accelerationFactor = 20;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexJumping2;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    private bool isJumpKeyDown = false;

    public override int Update()
    {
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        flip = h < 0;
        
        // Energy Recover
        Player.CurrentPlayer.EnergyRecover(Time.time);

        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
         PhysicsInputHelper(h,speedFactor,accelerationFactor);

        if (isCloseTo("Ground"))
        {
            return indexPSWallJumping;
        }

        if (Input.GetAxis("Vertical") > 0 && isCloseTo("Ladder") )
        {
            // up is pressed
            return indexPSClimb;
        }

        if (!isGrounded()&& Vy < 0)
        {
                return indexPSAirborne;
        }

        if (isGrounded())
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
        if (Vy < jumpForce / 5)
        {
            // prevent misclicking
            if (Input.GetButtonDown("Jump"))
                // second_jump
                return indexJumping2;
        }

        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;



        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter()
    {
        anim.Play("MainCharacter_Jump", -1, 0f);
        //Perform jump
        isJumpKeyDown = true;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
}
