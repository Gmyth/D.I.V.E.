using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Airborne", menuName = "Player State/Airborne")]
public class PSAirborne : PlayerState
{
    [Header( "Normal" )]
    [SerializeField] private float n_jumpTolerance;
    
    [Header( "Fever" )]
    [SerializeField] private float f_jumpTolerance;
    
    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSClimb;

    private State previous;

    public override int Update()
    {
        var jumpTolerance = playerCharacter.IsInFeverMode ? f_jumpTolerance : n_jumpTolerance;

        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        float v = Input.GetAxis("VerticalJoyStick");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        Vector2 normalizedInput = new Vector2(h, v).normalized;
        PhysicsInputHelper(h);


        if (Input.GetButtonDown("Attack1"))
        {
            return indexPSAttackGH;
        }
        
        if (Input.GetAxis("Vertical") > 0 || normalizedInput.y > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return indexPSClimb;
        }

        var dir = isCloseTo("Ground");

        if (dir != Direction.None && Mathf.Abs(h) > 0 && Vy < 0) { return indexPSWallJumping; }


        if (Input.GetButtonDown("Jump") && Time.time <  lastGroundedSec+ jumpTolerance)
        {
            return indexPSJumping1;
        }

        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return indexPSDashing;
        }
        
        if (isGrounded())
            return (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("HorizontalJoyStick") != 0) ? indexPSMoving : indexPSIdle;
        
        return Index;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
    }

    public override void OnStateEnter(State previousState)
    {
        // Add Ghost trail
        if (previousState.Index == indexPSIdle || previousState.Index == indexPSMoving)
        {
            lastGroundedSec = Time.time;
        }
        
        previous = previousState;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
        anim.Play("MainCharacter_Airborne", -1, 0f);
    }
}
