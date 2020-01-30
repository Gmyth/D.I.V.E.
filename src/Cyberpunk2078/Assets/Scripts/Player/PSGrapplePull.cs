using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_GrapplePull", menuName = "Player State/GrapplePull")]
public class PSGrapplePull : PlayerState
{
    
    [Header( "Normal" )]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSClimb;
    [SerializeField] private int indexPSAirborne;
    
    [Header( "Others" )]
    private GameObject grapple;
    
    private State previous;
    private bool initial;
    private float speed;

    public override string Update()
    {
        if (initial) OnStateEnter();
        Vector2 direction = (grapple.transform.position - playerCharacter.transform.position).normalized;
        float distance = (grapple.transform.position - playerCharacter.transform.position).sqrMagnitude;
        //playerCharacter.transform.right = direction.x > 0? direction:-direction;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        float v = Input.GetAxis("VerticalJoyStick");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        Vector2 normalizedInput = new Vector2(h, v).normalized;

        if (distance < 1f)
        {
            if (GetGroundType() == 0) return "Airborne";


            if (h == 0) return "Idle";
            return "Moving";
        }

        speed = speed < maxSpeed ? speed + Time.deltaTime * acceleration : maxSpeed;
        rb2d.velocity = direction * speed;

        if (Input.GetButtonDown("Ultimate"))
        {
            playerCharacter.ActivateFever();
        }

        if (Input.GetButtonDown("Attack1"))
        {
            return "Attack1";
        }

        if (Input.GetAxis("Vertical") > 0 || normalizedInput.y > 0.7f)
        {
            // up is pressed
            if (isCloseTo("Ladder") != Direction.None) return "Climbing";
        }

        var dir = isCloseTo("Ground");

        if (dir != Direction.None && Mathf.Abs(h) > 0 && Vy < 0) { return "WallSliding"; }


        if (Input.GetButtonDown("Jump"))
        {
            return "Jumping";
        }


        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            //break pull
            Player.CurrentPlayer.triggerReady = false;
            return "Dashing";
        }


        return Name;
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.transform.right = Vector3.right;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        grapple.GetComponent<GrappleHook>().Dead();
        rb2d.gravityScale = 3;
        initial = true;
    }

    public override void OnStateEnter(State previousState = null)
    {
        grapple = GameObject.FindGameObjectWithTag("GrappleHook");
        previous = previousState;
        playerCharacter.transform.right = Vector3.right;
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = false;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        speed = 15f;
        rb2d.gravityScale = 0;
        initial = false;
        rb2d.velocity = Vector2.zero;
        anim.Play("MainCharacter_Airborne", -1, 0f);
    }
}
