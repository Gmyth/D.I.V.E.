using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping2", menuName = "Player State/Attack Jumping 2")]
public class PSJumping2 : PlayerState
{
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float speed_factor = 3;
    [SerializeField] private float acceleration_factor = 20;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_PSWallJumping;
    [SerializeField] private int index_PSDashing;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;


    public override int Update()
    {
        float Vy = playerCharacter.GetComponent<Rigidbody2D>().velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
        PhysicsInputHelper(h,speed_factor,acceleration_factor);
        
        // Energy Recover
        Player.CurrentPlayer.EnergyRecover(Time.time);
        
        if (!isGrounded()&& Vy < 0)
        {
            return indexPSAirborne;
        }
        
        if (Input.GetAxis("Vertical") > 0  && isCloseTo("Ladder") != Direction.None)
        {
            // up is pressed
            return indexPSClimb;
        }
        
        if (isGrounded())
            {
                if (Input.GetAxis("Horizontal") == 0)
                    return index_PSIdle;

                return index_PSMoving;
        }
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (isCloseTo("Ground")!= Direction.None)
        {
            return index_PSWallJumping;
        }
        
        if (Input.GetAxis("Dashing") != 0)
            return index_PSDashing;
        return Index;
    }


    public override void OnStateEnter(State previousState)
    {
        anim.Play("MainCharacter_Jump", -1, 0f);
        
        //Perform jump
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
   
}
