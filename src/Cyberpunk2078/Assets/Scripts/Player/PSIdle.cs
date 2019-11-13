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
        
        // Energy Cost
        if (Player.CurrentPlayer.Fever)
        {
            Player.CurrentPlayer.CostFeverEnergy(Time.time);
        }
        
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (!isGrounded())
        {
            return indexPSAirborne;
        }
        
        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder") != Direction.None)
        {
            // up is pressed
            return indexPSClimb;
        }
            
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("HorizontalJoyStick") != 0)
            return indexPSMoving;

        if (Input.GetButtonDown("Jump"))
            return indexPSJumping1;

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
        rb2d.bodyType = RigidbodyType2D.Static;
        rb2d.useFullKinematicContacts = true;
        //rb2d.gravityScale = 0;
        if(grounded)Player.CurrentPlayer.AddNormalEnergy(1);
        anim.Play("MainCharacter_Idle", -1, 0f);
     
        
    }

    public override void OnStateQuit(State nextState)
    {
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        //rb2d.gravityScale = 3;
    }
    

}
