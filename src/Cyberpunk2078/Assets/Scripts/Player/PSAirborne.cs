﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Airborne", menuName = "Player State/Airborne")]
public class PSAirborne : PlayerState
{
    [Header( "Normal" )]
    [SerializeField] private float n_jumpTolerance;
    
    [Header( "Fever" )]
    [SerializeField] private float f_jumpTolerance;

    private State previous;


    public override string Update()
    {
        var jumpTolerance = playerCharacter.InKillStreak ? f_jumpTolerance : n_jumpTolerance;

        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        float v = Input.GetAxis("VerticalJoyStick") != 0 ? Input.GetAxis("VerticalJoyStick") : Input.GetAxis("Vertical");

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        Vector2 normalizedInput = new Vector2(h, v).normalized;
        if(Player.CurrentPlayer.LastBounceSec + 0.5f < Time.time) PhysicsInputHelper(h,v);
        
        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        


        if (Input.GetButtonDown("Attack1"))
        {
            return "Attack1";
        }

        if (Input.GetAxis("Vertical") > 0 || normalizedInput.y > 0.7f)
        {
            // up is pressed
            if (isCloseTo("Ladder") != Direction.None && Player.CurrentPlayer.climbReady) return "Climbing";
        }

        var dir = isCloseTo("Ground");

        if (dir != Direction.None)
            return "WallSliding";


        if (Input.GetButtonDown("Jump") && Time.time < lastGroundedSec + jumpTolerance)
        {
            return "Jumping";
        }


        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("RightTrigger") > 0 && Player.CurrentPlayer.RightTriggerReady))
        {
            Player.CurrentPlayer.RightTriggerReady = false;
            return "Dashing";
        }
        
        if (Input.GetButtonDown("Special1") || (Input.GetAxis("LeftTrigger") > 0 && Player.CurrentPlayer.LeftTriggerReady))
        {
            Player.CurrentPlayer.LeftTriggerReady = false;
            PlayerCharacter.Singleton.PowerDash = true;
            return "Dashing";
        }
        // temp code


        if (Vy <= 0)
            switch (GetGroundType())
            {
                case 1:
                    return (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("HorizontalJoyStick") != 0) ? "Moving" : "Idle";


                case 2:
                    Player.CurrentPlayer.JumpForceGate = true;

                    if (Mathf.Abs(rb2d.velocity.x) < 0.5)
                    {
                        rb2d.velocity = Vector2.zero;

                        rb2d.AddForce(Global.enemyHeadJumpVerticalForce * playerCharacter.transform.up + Global.enemyHeadJumpHorizontalForce * playerCharacter.transform.right);
                    }
                    else
                    {
                        Vector2 velocity = rb2d.velocity;
                        velocity.y = 0;

                        rb2d.velocity = velocity;

                        rb2d.AddForce(Global.enemyHeadJumpVerticalForce * playerCharacter.transform.up);
                    }

                    return "Jumping";
            }


        return Name;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = playerCharacter.Gravity;
        Player.CurrentPlayer.JumpForceGate = false; 
    }

    public override void OnStateEnter(State previousState)
    {
        // Add Ghost trail
        if (previousState.Name == "Idle" || previousState.Name == "Moving")
        {
            lastGroundedSec = Time.time;
        }
        
        previous = previousState;

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        //rb2d.gravityScale = playerCharacter.Gravity;

        anim.Play("MainCharacter_Airborne", -1, 0f);
    }
}
