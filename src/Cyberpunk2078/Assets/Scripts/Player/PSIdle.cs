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


    public override int Update()
    {

        NormalizeSlope();
        
        // Energy Recover
        Player.CurrentPlayer.EnergyRecover(Time.time);
        
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder"))
        {
            // up is pressed
            return indexPSClimb;
        }

        if (Input.GetAxis("Horizontal") != 0)
            return indexPSMoving;

        if (Input.GetAxis("Jump") > 0)
            return indexPSJumping1;
        
        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        anim.Play("MainCharacter_Idle", -1, 0f);
        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;
        V.x = 0;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }


    void NormalizeSlope()
    {
        // Attempt vertical normalization
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 4f);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f && hit.transform.tag == "Ground")
        {
            Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
            // Apply the opposite force against the slope force 
            // You will need to provide your own slopeFriction to stabalize movement
            rb2d.velocity = new Vector2(rb2d.velocity.x - (hit.normal.x * 0.5f), rb2d.velocity.y);
        }
        
    }

}
