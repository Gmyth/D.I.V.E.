using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Attack_GH", menuName = "Player State/Attack GH")]
public class PSAttackGH: PlayerState
{
    [Header( "Normal" )]
    [SerializeField] private float n_pushForce = 2f;
    [SerializeField] private float n_actionTime = 0.25f;
    [SerializeField] private float n_recoveryTime = 0.10f;
    
    [Header( "Fever" )]
    [SerializeField] private float f_pushForce = 3f;
    [SerializeField] private float f_actionTime = 0.25f;
    [SerializeField] private float f_recoveryTime = 0.05f;
    
    [SerializeField] private float EnergyConsume = -10;
    
    [SerializeField] private GameObject SplashFX;

    private float t0 = 0;
    private float defaultDrag;


    public override string Update()
    {
        var pushForce = n_pushForce;
        var actionTime = n_actionTime;
        var recoveryTime = n_recoveryTime;

        if (playerCharacter.InKillStreak)
        {
            pushForce = f_pushForce;
            actionTime = f_actionTime;
            recoveryTime = f_recoveryTime;
        }

        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        PhysicsInputHelper(h);

        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,0.8f);
        if (hit1.collider != null && hit1.transform.CompareTag("Ground"))
        {
            // kill all speed
            rb2d.velocity = new Vector2(0,0);
            
            // reset drag & gravity 
//            rb2d.drag = defaultDrag;
//            rb2d.gravityScale = 3;

            // Landed
                     
            // Kill Trail
          //  playerCharacter.GetComponent<GhostSprites>().Occupied = false;
            if (h == 0)
                // not moving
                return "Idle";


            return "Moving";
        }

        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }


        int groundType = GetGroundType();


        if (Time.time - t0 > actionTime)
        {
            // ok for dashing 
            if (Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady))
            {
                Player.CurrentPlayer.triggerReady = false;
                return "Dashing";
            }
            else if (Input.GetButtonDown("Jump") && grounded)
            {
                return "Jumping";
            }
        }
        
        if (Time.time - t0 > (recoveryTime + actionTime))
        {
            if (groundType == 0 && Vy < 0)
            {
                return "Airborne";
            }
            
            if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("HorizontalJoyStick") == 0)
                 return "Idle";

            return "Moving";
        }


        return Name;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        //rb2d.drag = defaultDrag;
        rb2d.gravityScale = 3;
                     
        // Kill Trail
        //playerCharacter.GetComponent<GhostSprites>().Occupied = false;

    }

    public override void OnStateEnter(State previousState)
    {
        
        var pushForce = playerCharacter.InKillStreak ? f_pushForce : n_pushForce;

        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //playerCharacter.GetComponent<GhostSprites>().Occupied = true;
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        t0 = Time.time;
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        
        //get Mouse direction
        
        Vector3 direction = mouse.GetDirectionCorrection(GroundNormal());


        var attack = Instantiate(SplashFX);
        attack.transform.position = playerCharacter.transform.position;
        attack.transform.right = direction;
        attack.transform.parent = playerCharacter.transform;
        attack.GetComponentInChildren<HitBox>().hit.source = playerCharacter;

        Destroy(attack, 0.2f);


        anim.Play("MainCharacter_Atk", -1, 0f);
        AudioManager.Instance.PlayOnce("Swing");

        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerCharacter.GetComponent<Rigidbody2D>().AddForce(direction * pushForce * 100f);
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = direction.x < 0;


        // kill gravity
        //rb2d.gravityScale = 0;
        //defaultDrag = rb2d.drag;
        //rb2d.drag = 0;


        //Camera Tricks
        CameraManager.Instance.Shaking(0.03f,0.05f);
    }
}
