using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Idle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;

    private float energyRecoverDelaySec = 0;
    private float energyRecoverTime;


    public override int Update()
    {

        NormalizeSlope();
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        
        // Energy Recover
        //if (energyRecoverTime < Time.time)
        //    Player.CurrentPlayer.EnergyRecover(Time.time, 100);
        
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (!isGrounded()&& Vy < 0)
        {
            return indexPSAirborne;
        }
        
        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder") != Direction.None)
        {
            // up is pressed
            return indexPSClimb;
        }
            
        if (Input.GetAxis("Horizontal") != 0)
            return indexPSMoving;

        if (Input.GetButtonDown("Jump"))
            return indexPSJumping1;
        
        if (Input.GetButtonDown("Dashing"))
            return indexPSDashing;
        
        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        energyRecoverTime = Time.time + energyRecoverDelaySec;
        if (Player.CurrentPlayer.haveEnemyBuff)
            Player.CurrentPlayer.ApplyEnergyChange(100);
        else if (Player.CurrentPlayer.Energy < 100)
            Player.CurrentPlayer.ApplyEnergyChange(100);

        anim.Play("MainCharacter_Idle", -1, 0f);
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(0,rb2d.velocity.y);
    }

    public override void OnStateQuit(State nextState)
    {
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
    }

    void NormalizeSlope()
    {
        // Attempt vertical normalization
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 1f);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f && hit.transform.tag == "Ground")
        {
            Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
            rb2d.velocity = Vector2.zero;
            rb2d.gravityScale = 0;
            // Apply the opposite force against the slope force 
            // You will need to provide your own slopeFriction to stabalize movement
            //rb2d.velocity = new Vector2(rb2d.velocity.x - (hit.normal.x * 0.703f), rb2d.velocity.y);
        }
        else
        {
            Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
            rb2d.gravityScale = 3;
        }

    }

}
