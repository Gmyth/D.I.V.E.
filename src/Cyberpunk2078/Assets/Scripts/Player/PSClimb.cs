using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_Climb", menuName = "Player State/Climb")]
public class PSClimb : PlayerState
{
    [Header("Normal")]
    [SerializeField] private float climbSpeed;
    
    [Header("Fever Mode")]
    [SerializeField] private float f_climbSpeed;

    private float t0;
    private float timeInterval = 0.5f;
    private Vector2 climbBoundary;


    public override string Update()
    {
        float feverFactor = playerCharacter.InFever ? Player.CurrentPlayer.FeverFactor : 1f;
        var v = Input.GetAxis("VerticalJoyStick")!=0? Input.GetAxis("VerticalJoyStick"): Input.GetAxis("Vertical");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        

        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return "Dashing";
        }
        
        if (Input.GetButtonDown("Special1"))
        {
            Player.CurrentPlayer.triggerReady = false;
            PlayerCharacter.Singleton.PowerDash = true;
            return "Dashing";
        }
        
        
        if (v == 0)
        {
            anim.speed = 0;
            rb2d.velocity = Vector2.zero;
            
        }
        else if (v > 0)
        {
            anim.speed = 1;
            rb2d.velocity = new Vector2(0, playerCharacter.InKillStreak ? f_climbSpeed * feverFactor:climbSpeed * feverFactor);
        }
        else
        {
            anim.speed = 1;
            rb2d.velocity = new Vector2(0, playerCharacter.InKillStreak ? -f_climbSpeed * feverFactor:-climbSpeed * feverFactor);
        }

        // climbBoundary = (minHeight, maxHeight)
        if (playerCharacter.transform.position.y < climbBoundary.x || playerCharacter.transform.position.y > climbBoundary.y)
        {
            anim.speed = 1;
            rb2d.gravityScale = playerCharacter.Gravity;
            return "Idle";
        }
        

        if (GetGroundType() == 1)
        {
            if(v < 0){
                anim.speed = 1;
                rb2d.gravityScale = playerCharacter.Gravity;
                return "Idle";
            }
        }


        if (Input.GetAxis("Jump") > 0)
        {
            anim.speed = 1;
            rb2d.gravityScale = playerCharacter.Gravity;
            return "Jumping";
        }

            
        return Name;
    }
    
    public override void OnStateEnter(State previousState)
    { 
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>(); 
        rb2d.velocity = Vector2.zero;
        climbBoundary = LadderMargin();
        t0 = Time.time;
       //kill speed
        Debug.LogWarning("min:" +climbBoundary.x + " max:" + climbBoundary.y);
       
       //Perform Climb
       anim.Play("MainCharacter_Climb", -1, 0f);
       
       // kill gravity
       rb2d.gravityScale = 0;
    }
}
