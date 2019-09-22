using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


[CreateAssetMenuAttribute(fileName = "PS_Moving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [SerializeField] private float speedFactor = 3;
    [SerializeField] private float accelerationFactor = 20;
    [SerializeField] private int indexPSIdle;
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
        
        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;

        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder"))
        {
            // up is pressed
            return indexPSClimb;
        }

        float x = Input.GetAxis("Horizontal");

        
        flip = x <= 0;
        if (x == 0)
            
            return indexPSIdle;
        
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        Move(x);


        if (Input.GetButtonDown("Jump"))
            return indexPSJumping1;

        return Index;
    }
    


    public override void OnStateEnter(State previousState)
    {
        Move(Input.GetAxis("Horizontal"));
        anim.Play("MainCharacter_Run", -1, 0f);
    }


    internal void Move(float axis)
    {
        int direction = axis > 0 ? 1 : -1;
        PhysicsInputHelper(axis,speedFactor,accelerationFactor);
    }
    
    void NormalizeSlope () {
        // Attempt vertical normalization
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 4f);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f && hit.transform.tag == "Ground"){
                Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
                // Apply the opposite force against the slope force 
                // You will need to provide your own slopeFriction to stabalize movement
                rb2d.velocity = new Vector2(rb2d.velocity.x - (hit.normal.x * 0.7f), rb2d.velocity.y);
                //Move Player up or down to compensate for the slope below them
                Vector3 pos = playerCharacter.transform.position;
                pos.y += -hit.normal.x * Mathf.Abs(rb2d.velocity.x) * Time.deltaTime * (rb2d.velocity.x - hit.normal.x > 0 ? 1 : -1);
                playerCharacter.transform.position = pos;
        }
    }
    
}
