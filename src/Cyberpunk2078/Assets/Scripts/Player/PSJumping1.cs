using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_Jumping2;
    [SerializeField] private int index_PSWallJumping;
    [SerializeField] private int index_PSDashing;
    private bool isJumpKeyDown = false;

    public override int Update()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
         PhysicsInputHelper(h);
        
        if (isCloseToWall())
        {
            return index_PSWallJumping;
        }
        
        if (isGrounded() && Vy < 0)
            {
                // Landed
                if (h == 0)
                    return index_PSIdle;

                return index_PSMoving;
        }
        
        //Player is sill in air
        if (Vy < jumpForce / 5)
        {
            // prevent misclicking
            if (Input.GetButtonDown("Jump"))
                // second_jump
                return index_Jumping2;
        }
        
        if (Input.GetAxis("Dashing") != 0)
            return index_PSDashing;

       

        //isJumpKeyDown = Input.GetButtonDown("Jump");
        
        return Index;
    }

    public override void OnStateEnter()
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
