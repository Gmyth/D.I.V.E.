using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Moving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_speedFactor;
    [SerializeField] private float n_accelerationFactor;
    
    [Header("Fever Mode")]
    [SerializeField] private float f_speedFactor;
    [SerializeField] private float f_accelerationFactor;


    public override string Update()
    {
        NormalizeSlope();
        if (playerCharacter.InKillStreak) anim.speed = 1.1f;
        else anim.speed = 1f;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;

        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (Input.GetButtonDown("Attack1"))
        {
            return "Attack1";
        }
            
        
        if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
        {
            Player.CurrentPlayer.triggerReady = false;
            return "Dashing";
        }
        
        if (Input.GetButtonDown("Special1"))
        {
            Player.CurrentPlayer.triggerReady = false;
            PlayerCharacter.Singleton.PowerDash = true;
            return "Dashing";
        }

        if (Input.GetAxis("Vertical") > 0 || Input.GetAxis("VerticalJoyStick") > 0.7f)
        {
            // up is pressed
            if(isCloseTo("Ladder") != Direction.None) return "Climbing";
        }
        

        float x = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        
        flip = x <= 0;
        if (x == 0)
            return "Idle";
        
        
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = flip;
        Move(x);

        if (GetGroundType() == 0 && Vy < 0)
        {
            return "Airborne";
        }

        if (Input.GetButtonDown("Jump"))
        {
            return "Jumping";
        }
           
        
        return Name;
    }
    


    public override void OnStateEnter(State previousState)
    {
        playerCharacter.AddNormalEnergy(1);
        if (PlayerCharacter.Singleton.InFever) PlayerCharacter.Singleton.AddOverLoadEnergy(1);
        float h = Input.GetAxis("HorizontalJoyStick") != 0
            ? Input.GetAxis("HorizontalJoyStick")
            : Input.GetAxis("Horizontal");
        
        Move(h);
        
        anim.Play("MainCharacter_Run", -1, 0f);

        AudioManager.Instance.PlayEvent("Footsteps");

        //VFX 
        if (Mathf.Abs(h) > 0)
        {
            var Dust = ObjectRecycler.Singleton.GetObject<SingleEffect>(10);
            Dust.transform.position = playerCharacter.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 3f);
            Dust.transform.right =Vector3.right;
            if (hit.collider != null && hit.transform.tag == "Ground")
            {
                Dust.transform.up = hit.normal;
            }

            
            if (h > 0) Dust.transform.localScale = new Vector3(-1, 1, 1);
            else Dust.transform.localScale = Vector3.one;
            ;
//            if (h > 0) Dust.transform.right = Vector3.left;
//            else Dust.transform.right = Vector3.right;
            Dust.gameObject.SetActive(true);
        }

        if (playerCharacter.InKillStreak) anim.speed = 1.1f;
    }
    
    public override void OnStateQuit(State nextState)
    {

        anim.speed = 1f;

        AudioManager.Instance.StopEvent("Footsteps");
        
        Player.CurrentPlayer.OnSlope = false;
    }


    internal void Move(float axis)
    {
        int direction = axis > 0 ? 1 : -1;
        if(!playerCharacter.InKillStreak) {PhysicsInputHelper(axis,n_speedFactor,n_accelerationFactor);}
        else {PhysicsInputHelper(axis,f_speedFactor,f_accelerationFactor);}
    }
    
    void NormalizeSlope () {
        // Attempt vertical normalization
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position, -Vector2.up, 4f);
        if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f && hit.transform.tag == "Ground"){
                Player.CurrentPlayer.OnSlope = true;
                Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
                // Apply the opposite force against the slope force 
                // You will need to provide your own slopeFriction to stabalize movement
                rb2d.velocity = new Vector2(rb2d.velocity.x - (hit.normal.x * 0.8f), rb2d.velocity.y);
                //Move Player up or down to compensate for the slope below them
                Vector3 pos = playerCharacter.transform.position;
                pos.y += -hit.normal.x * Mathf.Abs(rb2d.velocity.x) * Time.deltaTime * (rb2d.velocity.x - hit.normal.x > 0 ? 1 : -1);
                playerCharacter.transform.position = pos;
        }
        else
        {
            Player.CurrentPlayer.OnSlope = false;
        }
    }
    
}
