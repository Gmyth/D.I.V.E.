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

    private Rigidbody2D rigidbody;

    private bool onWall = false;
    private bool isJumpKeyDown = false;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);


        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override string Update()
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
                return "Idle";

            return "Moving";
        }

        if (groundType == 2)
        {
            Player.CurrentPlayer.JumpForceGate = true;

            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(Global.enemyHeadJumpVerticalForce * playerCharacter.transform.up - Global.enemyHeadJumpHorizontalForce * playerCharacter.transform.right);

            return "Jumping";
        }


        var wallType = isCloseTo("Ground");

        if (wallType == Direction.Right && h > 0)
        {
            playerCharacter.groundDust.transform.localPosition =  new Vector3(0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();

            rigidbody.gravityScale = 1.2f;

            anim.Play("MainCharacter_WallJump", -1, 0f);
            AudioManager.Singleton.PlayEvent("WallSlide");
        }
        else if (wallType == Direction.Left && h < 0)
        {
            playerCharacter.groundDust.transform.localPosition = new Vector3(-0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();

            rigidbody.gravityScale = 1.2f;

            anim.Play("MainCharacter_WallJump", -1, 0f);
            AudioManager.Singleton.PlayEvent("WallSlide");
        }
        else
        {
            playerCharacter.groundDust.transform.localPosition = Vector3.zero;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = 0;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Stop();

            rigidbody.gravityScale = playerCharacter.DefaultGravity;

            anim.Play("MainCharacter_Airborne", -1, 0f);
            AudioManager.Singleton.PlayEvent("WallSlide");
        }

        if (wallType == Direction.None && groundType == 0)
            return "Airborne";


        if (Input.GetButtonDown("Attack1"))
            return "Attack1";


        if (Input.GetButtonDown("Jump"))
            return "Jumping";


        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None && Player.CurrentPlayer.climbReady) return "Climbing";
        }
        
        // perform Dashing
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("RightTrigger") > 0 && Player.CurrentPlayer.RightTriggerReady))
        {
            Player.CurrentPlayer.RightTriggerReady = false;
            return "Dashing";
        }
        
        if (Input.GetButtonDown("Special1") || (Input.GetAxis("LeftTrigger") > 0 && Player.CurrentPlayer.LeftTriggerReady))
        {
            Player.CurrentPlayer.LeftTriggerReady = false;
            PlayerCharacter.Singleton.PowerDash = true;
            return "Dashing";
        }


        return Name;
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
        rb2d.gravityScale = playerCharacter.DefaultGravity;
        onWall = false;

        AudioManager.Singleton.StopEvent("WallSlide");
    }

}
