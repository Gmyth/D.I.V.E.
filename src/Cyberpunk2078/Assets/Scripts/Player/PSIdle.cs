using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Idle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    public override string Update()
    {
        // NormalizeSlope();
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        

        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (Input.GetButtonDown("Attack1"))
            return "Attack1";


        if (Input.GetButtonDown("Jump"))
            return "Jumping";

        if (GetGroundType() == 0)
            return "Airborne";
        
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None)
                return "Climbing";
        }
            
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("HorizontalJoyStick") != 0)
            return "Moving";


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


        return Name;
    }

    public override void OnStateEnter(State previousState)
    { 
        playerCharacter.AddNormalEnergy(1);
        if (PlayerCharacter.Singleton.InFever) PlayerCharacter.Singleton.AddOverLoadEnergy(1);
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
       // rb2d.bodyType = RigidbodyType2D.Kinematic;
       rb2d.velocity = Vector2.zero;
       rb2d.gravityScale = 0;
       anim.Play("MainCharacter_Idle", -1, 0f);
        
        
    }

    public override void OnStateQuit(State nextState)
    {
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
       // rb2d.bodyType = RigidbodyType2D.Dynamic;
       rb2d.gravityScale = playerCharacter.Gravity;
    }
    

}
