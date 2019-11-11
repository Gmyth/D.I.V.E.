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
        var jumpTolerance = Player.CurrentPlayer.Fever ? f_jumpTolerance : n_jumpTolerance;
        float h = Input.GetAxis("Horizontal");
        PhysicsInputHelper(h);

        // Energy Cost
        if (Player.CurrentPlayer.Fever)
        {
            Player.CurrentPlayer.CostFeverEnergy(Time.time);
        }

        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder") != Direction.None)
        {
            // up is pressed
            return indexPSClimb;
        }

        var dir = isCloseTo("Ground");

        if (dir == Direction.Right && h > 0) { return indexPSWallJumping; }
        else if (dir == Direction.Left && h < 0) { return indexPSWallJumping; }


        if (Input.GetButtonDown("Jump") && Time.time <  lastGroundedSec+ jumpTolerance)
        {
            return indexPSJumping1;
        }

        
        if (Input.GetButtonDown("Dashing"))
            return indexPSDashing;
        
        if (isGrounded())
            return Input.GetAxis("Horizontal") != 0?indexPSMoving:indexPSIdle;
        
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
        anim.Play("MainCharacter_Airborne", -1, 0f);
    }
}
