using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Attack_GH", menuName = "Player State/Attack GH")]
public class PSAttackGH: PlayerState
{
    [SerializeField] private float pushForce = 2f;
    [SerializeField] private float actionTime = 0.3f;
    [SerializeField] private float recoveryTime = 0.2f;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private float EnergyConsume = -10; 
    
    [SerializeField] private GameObject SplashFX; 
    private float t0 = 0;
    private float defaultDrag;


    public override int Update()
    {
        float h = Input.GetAxis("Horizontal");
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        PhysicsInputHelper(h);
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,0.5f);
        if (hit1.collider != null && hit1.transform.CompareTag("Ground"))
        {
            // kill all speed
            rb2d.velocity = new Vector2(0,0);
            
            // reset drag & gravity 
//            rb2d.drag = defaultDrag;
//            rb2d.gravityScale = 3;

            // Landed
                     
            // Kill Trail
            playerCharacter.GetComponent<GhostSprites>().Occupied = false;
            if (h == 0)
                // not moving
            
                return indexPSIdle;
            return indexPSMoving;
        }

        if (Time.time - t0 > actionTime)
        {
            // ok for dashing 
            if (Input.GetButtonDown("Dashing")) {
                return indexPSDashing;
            }
                
        }
        
        if (Time.time - t0 > (recoveryTime + actionTime))
        {

            if (!isGrounded()&& Vy < 0)
            {
                return indexPSAirborne;
            }
            
            if (Input.GetAxis("Horizontal") == 0)
                return indexPSIdle;

            return indexPSMoving;
        }

        if (Input.GetButtonDown("HealthConsume"))
            playerCharacter.ConsumeFever();
        
        return Index;
    }

    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        
        //rb2d.drag = defaultDrag;
        rb2d.gravityScale = 3;
                     
        // Kill Trail
        playerCharacter.GetComponent<GhostSprites>().Occupied = false;

    }

    public override void OnStateEnter(State previousState)
    {
        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerCharacter.GetComponent<GhostSprites>().Occupied = true;
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        t0 = Time.time;
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        
        //get Mouse direction
        Vector3 direction = getDirectionCorrection(mouse.getAttackDirection(),GroundNormal());
        var obj = Instantiate(SplashFX);
        obj.transform.position = playerCharacter.transform.position;
        obj.transform.right = direction;
        obj.transform.parent = playerCharacter.transform;
        anim.Play("MainCharacter_Atk", -1, 0f);
        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerCharacter.GetComponent<Rigidbody2D>().AddForce(direction * pushForce * 100f);
        playerCharacter.GetComponent<SpriteRenderer>().flipX = direction.x < 0;
        Destroy(obj,0.3f);
        //kill gravity
//        rb2d.gravityScale = 0;
//        defaultDrag = rb2d.drag;
//        rb2d.drag = 0;
        
        //Camera Tricks
        CameraManager.Instance.Shaking(0.03f,0.05f);
    }
}
