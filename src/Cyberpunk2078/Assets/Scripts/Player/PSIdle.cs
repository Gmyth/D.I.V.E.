using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Idle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    [Header( "Transferable States" )]
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;


    public override int Update()
    {

       // NormalizeSlope();
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }


        if (Input.GetButtonDown("Jump"))
            return indexPSJumping1;

        if (!isGrounded())
        {
            return indexPSAirborne;
        }
        
        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return indexPSClimb;
        }
            
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("HorizontalJoyStick") != 0)
            return indexPSMoving;


        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return indexPSDashing;
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    { 
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
       rb2d.gravityScale = 3;
    }
    

}
