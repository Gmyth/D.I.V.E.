using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping2", menuName = "Player State/Attack Jumping 2")]
public class PSJumping2 : PlayerState
{
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;


    public override int Update()
    {
        float Vy = playerCharacter.GetComponent<Rigidbody2D>().velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
        PhysicsInputHelper(h);
        
        if (playerCharacter.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            if (isGrounded())
            {
                if (Input.GetAxis("Horizontal") == 0)
                    return index_PSIdle;

                return index_PSMoving;
            }
        }

        return Index;
    }

    public override void OnStateEnter()
    {
        //Perform jump
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
   
}
