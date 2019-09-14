using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_Climb", menuName = "Player State/Climb")]
public class PSClimb : PlayerState
{
    [SerializeField] private float climbSpeed;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSJumping1;


    public override int Update()
    {
        var v = Input.GetAxis("Vertical");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        // Energy Recover
        Player.CurrentPlayer.EnergyRecover(Time.time);

        if (v == 0)
        {
            anim.speed = 0;
            rb2d.velocity = Vector2.zero;
            
        }else if (v > 0)
        {
            anim.speed = 1;
            rb2d.velocity = new Vector2(0, climbSpeed);
        }
        else
        {
            anim.speed = -1;
            rb2d.velocity = new Vector2(0, -climbSpeed);
        }

        if (!isCloseTo("Ladder"))
        {
            anim.speed = 1;
            rb2d.gravityScale = 3;
            return indexPSIdle;
        }

        if (Input.GetAxis("Jump") > 0)
        {
            anim.speed = 1;
            rb2d.gravityScale = 3;
            return indexPSJumping1;
        }
            
        return Index;
    }

    public override void OnStateEnter()
    {
        playerCharacter.GetComponent<SpriteRenderer>().flipX = !RightSideTest("Ladder");
        
       //kill speed
       var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
       rb2d.velocity = Vector2.zero;
       
       //Perform Climb
       anim.Play("MainCharacter_Climb", -1, 0f);
       
       // kill gravity
       rb2d.gravityScale = 0;
    }
}
