using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_WallSliding", menuName = "Player State/Wall Sliding")]
public class PSWallSliding: PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_stickWallTime = 0.25f;
    [SerializeField] private float n_wallCheckCoolDown = 0.25f;
    [Header("Fever")]
    [SerializeField] private float f_stickWallTime = 0.25f;
    [SerializeField] private float f_wallCheckCoolDown = 0.25f;
    
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_PSJumping1;
    [SerializeField] private int index_PSJumping2;
    [SerializeField] private int index_PSDashing;
    [SerializeField] private int index_PSAttackGH;
    [SerializeField] private int index_PSAirborne;
    [SerializeField] private int index_PSClimb;

    private Rigidbody2D rigidbody;

    private bool onWall = false;
    private bool isJumpKeyDown = false;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);


        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override int Update()
    {
        var stickWallTime = n_stickWallTime;
        var wallCheckCoolDown = n_wallCheckCoolDown;

        if (playerCharacter.InKillStreak)
        {
            stickWallTime = f_stickWallTime;
            wallCheckCoolDown = f_wallCheckCoolDown;
        }


        float Vy = rigidbody.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");


        int groundType = GetGroundType();

        if (groundType == 1 && Vy <= 0)
        {
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }

        if (groundType == 2)
        {
            Player.CurrentPlayer.jumpForceGate = true;

            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(Global.enemyHeadJumpVerticalForce * playerCharacter.transform.up - Global.enemyHeadJumpHorizontalForce * playerCharacter.transform.right);

            return index_PSJumping1;
        }


        var wallType = isCloseTo("Ground");

        if (wallType == Direction.Right && h > 0)
        {
            playerCharacter.groundDust.transform.localPosition =  new Vector3(0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();

            rigidbody.gravityScale = 1.2f;

            anim.Play("MainCharacter_WallJump", -1, 0f);
        }
        else if (wallType == Direction.Left && h < 0)
        {
            playerCharacter.groundDust.transform.localPosition = new Vector3(-0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();

            rigidbody.gravityScale = 1.2f;

            anim.Play("MainCharacter_WallJump", -1, 0f);
        }
        else
        {
            playerCharacter.groundDust.transform.localPosition = Vector3.zero;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = 0;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Stop();

            rigidbody.gravityScale = 3f;

            anim.Play("MainCharacter_Airborne", -1, 0f);
        }

        if (wallType == Direction.None && groundType == 0)
            return index_PSAirborne;


        if (Input.GetButtonDown("Attack1"))
            return index_PSAttackGH;


        if (Input.GetButtonDown("Jump"))
            return index_PSJumping1;


        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None)
                return index_PSClimb;
        }
        
        // perform Dashing
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return index_PSDashing;
        }
        
        if (Input.GetButtonDown("Special1"))
        {
            Player.CurrentPlayer.triggerReady = false;
            PlayerCharacter.Singleton.PowerDash = true;
            return index_PSDashing;
        }


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        var wallType = isCloseTo("Ground");
        flip = wallType == Direction.Left;

        // flip sprite for correct facing 
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;


        rigidbody. velocity = Vector2.zero;
        rigidbody.gravityScale = 1.8f;


        //kill speed
        onWall = true;


        // Animation
        anim.Play("MainCharacter_Airborne", -1, 0f);
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.groundDust.transform.localPosition = Vector3.zero;
        playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = 0;
        playerCharacter.groundDust.GetComponent<ParticleSystem>().Stop();
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
        onWall = false;
    }

}
