using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping2", menuName = "Player State/Attack Jumping 2")]
public class PSJumping2 : PlayerState
{
    [Header( "Normal" )]
    [SerializeField] private float n_jumpForce = 3f;
    [SerializeField] private float n_speedFactor = 3;
    [SerializeField] private float n_accelerationFactor = 20;
    
    [Header( "Fever" )]
    [SerializeField] private float f_jumpForce = 3f;
    [SerializeField] private float f_speedFactor = 3;
    [SerializeField] private float f_accelerationFactor = 20;


    public override string Update()
    {
        var jumpForce =  n_jumpForce;
        var speedFactor = n_speedFactor;
        var accelerationFactor = n_accelerationFactor;
        if (playerCharacter.InKillStreak)
        {
            jumpForce =  f_jumpForce;
            speedFactor = f_speedFactor; 
            accelerationFactor = f_accelerationFactor;
        }
        
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;
        float Vy = playerCharacter.GetComponent<Rigidbody2D>().velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
        PhysicsInputHelper(h,speedFactor,accelerationFactor);

        bool ground = GetGroundType() == 1;

        if (!ground && Vy < 0)
        {
            return "Airborne";
        }
        
        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return "Climbing";
        }
        
        
        if (ground)
        {
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("HorizontalJoyStick") == 0)
                return "Idle";

            return "Moving";
        }
        
        if (Input.GetButtonDown("Attack1"))
        {
            return "Attack1";
        }
        
        if (isCloseTo("Ground")!= Direction.None)
        {
            return "WallSliding";
        }
        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return "Dashing";
        }


        return Name;
    }


    public override void OnStateEnter(State previousState)
    {
        
        var jumpForce = playerCharacter.InKillStreak ? f_jumpForce : n_jumpForce;
        anim.Play("MainCharacter_Jump", -1, 0f);
        
        //Perform jump
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        // Add Verticial Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
   
}
