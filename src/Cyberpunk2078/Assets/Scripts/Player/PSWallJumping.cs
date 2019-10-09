using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_WallJumping", menuName = "Player State/Wall Jumping")]
public class PSWallJumping: PlayerState
{
    [SerializeField] private float jumpForce = 4.5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float stickWallTime = 0.25f;
    [SerializeField] private float wallCheckCoolDown = 0.25f;
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
    private float lastStickTime;
    private float lastOnWallTime;
    public override int Update()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        

        
        if (onWall && Time.time > lastStickTime + stickWallTime)
        {
            //stick over, falling start
            rb2d.gravityScale = 3;
        }

        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        
        if (isGrounded() && Vy <= 0)
        {
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return index_PSAttackGH;
        }

        if (onWall)
        {
            //Sticked , ok to perform wall jump again
            if (Input.GetButtonDown("Jump"))
            {
                onWall = false;
                lastOnWallTime = Time.unscaledTime;
                //re active jumping
                if (RightSideTest("Ground"))
                {
                    flip = true;
                    Player.CurrentPlayer.lastWallJumpRight = true;
                    rb2d.velocity = h <= 0? new Vector2(-jumpSpeed*1.3f, 0):new Vector2(-jumpSpeed * 1.3f, 0);
                }
                else
                {
                    flip = false; 
                    Player.CurrentPlayer.lastWallJumpRight = false; 
                    rb2d.velocity = h >= 0? new Vector2(jumpSpeed*1.3f, 0):new Vector2(jumpSpeed * 1.3f, 0);
                }

                return index_PSJumping1;
            }
        }
        var dir = isCloseTo("Ground");
        if (h == 0)
        {
            return index_PSAirborne;
        }
        else if (dir == Direction.Right && h < 0) { return index_PSAirborne; }
        else if (dir == Direction.Left && h > 0) { return index_PSAirborne; }

        if (dir == Direction.None && !isGrounded())
        {
            return index_PSAirborne;
        }


//        if (!onWall && isCloseTo("Ground") && Time.time > lastOnWallTime + wallCheckCoolDown )
//        {
//            //Not stick yet, ok to perform wall jump again
//            if (Input.GetButtonDown("Jump"))
//            {
//                lastOnWallTime = Time.unscaledTime;
//                rb2d.gravityScale = 3;
//                
//                //Check the wall is on the left or right
//                if (RightSideTest("Ground"))
//                {
//                    //right
//                    flip = true;
//                    rb2d.velocity = new Vector2(-jumpSpeed, 0);
//                }
//                else
//                {
//                    //left
//                    flip = false; 
//                    rb2d.velocity = new Vector2 (jumpSpeed, 0);
//                }
//                
//                // Perform wall jump
//                sideWallJump();
//            }
//            else
//            {
//                OnStateEnter(this);
//            }
//        }
        
        
        if (Input.GetAxis("Vertical") > 0 && isCloseTo("Ladder") != Direction.None)
        {
            return index_PSClimb;
        }
        
        // flip sprite for correct facing 
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        
        // perform Dashing
        if (Input.GetButtonDown("Dashing"))
            return index_PSDashing;
        
        if (Input.GetButtonDown("HealthConsume"))
        {
            Player.CurrentPlayer.CostHealthEnergy();
        }
        
        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        // Set flip
        flip = !RightSideTest("Ground");
        
        //kill speed
        onWall = true;
        
        // Animation
        anim.Play("MainCharacter_WallJump", -1, 0f);

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        rb2d. velocity = Vector2.zero;

        rb2d.gravityScale = 1.8f;
        
        lastStickTime = Time.time;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 3;
        onWall = false;
    }

    private void sideWallJump()
    {
        //Perform jump
        anim.Play("MainCharacter_Jump", -1, 0f);
        
        // prevent applying force twice
        isJumpKeyDown = true;
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        // kill any Y-axis speed
        rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
        
        // Add Vertical Speed
        rb2d.AddForce(playerCharacter.transform.up * jumpForce * 100);
    }
    

}
