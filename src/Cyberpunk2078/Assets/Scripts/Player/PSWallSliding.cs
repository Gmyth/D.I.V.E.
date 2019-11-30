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
    private bool isJumpKeyDown = false;

    private bool onWall = false;
    private float gravityHolder;

    public override int Update()
    {
        var stickWallTime = n_stickWallTime;
        var wallCheckCoolDown = n_wallCheckCoolDown;

        if (playerCharacter.IsInFeverMode)
        {
            stickWallTime = f_stickWallTime;
            wallCheckCoolDown = f_wallCheckCoolDown;
        }


        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        
        
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        
        if (isGrounded() && Vy <= 0)
        {
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }
        
        if (Input.GetButtonDown("Attack1"))
        {
            return index_PSAttackGH;
        }

        
        if (Input.GetButtonDown("Jump"))
        {
            return index_PSJumping1;
        }

        var dir = isCloseTo("Ground");
        if (dir == Direction.Right && h > 0)
        {
            
            rb2d.gravityScale = 1.2f;
            playerCharacter.groundDust.transform.localPosition =  new Vector3(0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();
            anim.Play("MainCharacter_WallJump", -1, 0f);
        }
        else if (dir == Direction.Left && h < 0)
        {
            playerCharacter.groundDust.transform.localPosition = new Vector3(-0.2f,-0.5f,0);
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = -1;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Play();
            rb2d.gravityScale = 1.2f;
            anim.Play("MainCharacter_WallJump", -1, 0f);
        }
        else
        {
            anim.Play("MainCharacter_Airborne", -1, 0f);
            playerCharacter.groundDust.transform.localPosition = Vector3.zero;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().gravityModifier = 0;
            playerCharacter.groundDust.GetComponent<ParticleSystem>().Stop();
            rb2d.gravityScale = 3f;
        }
        
        if (dir == Direction.None && !isGrounded())
        {
           
            return index_PSAirborne;
        }


        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return index_PSClimb;
        }
        // flip sprite for correct facing 
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;
        
        // perform Dashing
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return index_PSDashing;
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        
        
        var dir = isCloseTo("Ground");
        // Set flip
        flip = dir == Direction.Left;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();

        //kill speed
        onWall = true;
        
        // Animation
        anim.Play("MainCharacter_Airborne", -1, 0f);

        rb2d. velocity = Vector2.zero;

        rb2d.gravityScale = 1.8f;
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
