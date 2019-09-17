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

    public override void OnStateEnter()
    {
        anim.Play("MainCharacter_Idle", -1, 0f);
        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;
        V.x = 0;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
