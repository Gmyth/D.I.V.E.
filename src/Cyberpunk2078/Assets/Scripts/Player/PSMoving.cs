using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


[CreateAssetMenuAttribute(fileName = "PS_Moving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_speedFactor;
    [SerializeField] private float n_accelerationFactor;
    
    [Header("Fever Mode")]
    [SerializeField] private float f_speedFactor;
    [SerializeField] private float f_accelerationFactor;
    
    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    [SerializeField] private int indexPSWallJumping;
    public override int Update()
    {
        NormalizeSlope();

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;

        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
            
        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return indexPSDashing;
        }

        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return indexPSClimb;
        }
        
        
        float x = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        
        flip = x <= 0;
        if (x == 0)
            return indexPSIdle;
        
        
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        Move(x);

        if (!isGrounded() && Vy < 0)
        {
            return indexPSAirborne;
        }

        if (Input.GetButtonDown("Jump"))
        {
            return indexPSJumping1;
        }
           
        
        return Index;
    }
    


    public override void OnStateEnter(State previousState)
    {
        Move(Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal"));
        if (grounded)
            playerCharacter.AddNormalEnergy(1);
        anim.Play("MainCharacter_Run", -1, 0f);
    }


    internal void Move(float axis)
    {
        int direction = axis > 0 ? 1 : -1;
        if(!playerCharacter.IsInFeverMode) {PhysicsInputHelper(axis,n_speedFactor,n_accelerationFactor);}
        else {PhysicsInputHelper(axis,f_speedFactor,f_accelerationFactor);}
    }
    
    void NormalizeSlope () {
        // Attempt vertical normalization
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 4f);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f && hit.transform.tag == "Ground"){
                Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
                // Apply the opposite force against the slope force 
                // You will need to provide your own slopeFriction to stabalize movement
                rb2d.velocity = new Vector2(rb2d.velocity.x - (hit.normal.x * 0.8f), rb2d.velocity.y);
                //Move Player up or down to compensate for the slope below them
                Vector3 pos = playerCharacter.transform.position;
                pos.y += -hit.normal.x * Mathf.Abs(rb2d.velocity.x) * Time.deltaTime * (rb2d.velocity.x - hit.normal.x > 0 ? 1 : -1);
                playerCharacter.transform.position = pos;
        }
    }
    
}
