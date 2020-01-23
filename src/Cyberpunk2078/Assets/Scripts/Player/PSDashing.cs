using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dashing", menuName = "Player State/Dashing")]
public class PSDashing : PlayerState
{
    [Header("Normal")]
    [SerializeField] private float n_dashForce = 8; // Dash Initial speed
    [SerializeField] private float n_dashDelayTime = 0.05f; // time from button press to actual dash
    [SerializeField] private float n_dashReleaseTime = 0.22f; // time to recover rb2d setting after dashing process finished
    [SerializeField] private float n_dashReleaseDelayTime = 0.05f; // time from dash finish to actual controllable
    [SerializeField] private float n_inDashingDragFactor = 3; // In-dash drag factor
    [SerializeField] private float n_JumpListenerInterval = 0.2f;

    [Header("Fever")]
    [SerializeField] private float f_dashForce = 8; // Dash Initial speed
    [SerializeField] private float f_dashDelayTime = 0.05f; // time from button press to actual dash
    [SerializeField] private float f_dashReleaseTime = 0.22f; // time to recover rb2d setting after dashing process finished
    [SerializeField] private float f_dashReleaseDelayTime = 0.05f; // time from dash finish to actual controllable
    [SerializeField] private float f_inDashingDragFactor = 3; // In-dash drag factor
    [SerializeField] private float f_JumpListenerInterval = 0.2f;

    [Header( "Common" )]
    [SerializeField] private float EnergyConsume = -70;

    [Header( "Transferable States" )]
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexWallJumping;
    [SerializeField] private int indexPSAirborne;

    private float lastDashSecond;
    private bool hyperSpeed;
    private float defaultDrag;
    private bool readyToPush;
    private bool Apply = true;
    private float lastJumpInput;


    public override int Update()
    {

        var dashDelayTime = playerCharacter.InKillStreak ? f_dashDelayTime:n_dashDelayTime;
        var dashReleaseTime = playerCharacter.InKillStreak ? f_dashReleaseTime:n_dashReleaseTime;
        var dashReleaseDelayTime = playerCharacter.InKillStreak ? f_dashReleaseTime:n_dashReleaseTime;
        var JumpListenerInterval = playerCharacter.InKillStreak ? f_JumpListenerInterval:n_JumpListenerInterval;

        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (!Apply)
        {
            if (GetGroundType() == 0)
            {
                return indexPSAirborne;
            }

            if (h == 0) return indexPSIdle;
            return indexPSMoving;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (Player.CurrentPlayer.secondJumpReady && lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.unscaledTime)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                return indexPSJumping1;
            }
            else
            {
                // save jump for later
                lastJumpInput = Time.unscaledTime;
            }
        }

        if (lastDashSecond + dashDelayTime < Time.unscaledTime && readyToPush)
        {
            // Press delay ends. time to actual dash
            readyToPush = false;
            forceApply();
        }


        if (lastDashSecond + dashReleaseTime + dashDelayTime < Time.unscaledTime)
        {
            // the dash has already ended
            if (hyperSpeed)
            {
                // kill speed after dash
                hyperSpeed = false;
                rb2d.drag = defaultDrag;
                rb2d.velocity = rb2d.velocity * 0.3f;
            }
            PhysicsInputHelper(h);

        } else if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.unscaledTime)
        {
            //cast listened jump right after dash finish
            if (lastJumpInput + JumpListenerInterval > Time.unscaledTime)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                    return indexPSJumping1;
            }

            // the dash has already ended
            // ok for move input
            PhysicsInputHelper(h);

            //enable Collision
            playerCharacter.transform.right = Vector3.right;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"), false);
        }

        //prevent ground-hitting shifting
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,1.5f);
        if (hit1.collider != null)
        {
            if(hit1.transform.CompareTag("Ground")){
                if( Mathf.Abs(hit1.normal.x) > 0f)rb2d.velocity = new Vector2(rb2d.velocity.x, 0 );
                if( Mathf.Abs(hit1.normal.y) > 0f)rb2d.velocity =  new Vector2(0, rb2d.velocity.y );
            }
            else if(hit1.transform.CompareTag("Platform") && rb2d.velocity.normalized.y < 0)
            {
                //upper ward
                if( Mathf.Abs(hit1.normal.x) > 0f)rb2d.velocity = new Vector2(rb2d.velocity.x, 0 );
                if( Mathf.Abs(hit1.normal.y) > 0f)rb2d.velocity =  new Vector2(0, rb2d.velocity.y );
            }
        }


        // Player is grounded and dash has finished
        if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime  < Time.unscaledTime)
        {
            rb2d.velocity = rb2d.velocity * 0.3f;
            PhysicsInputHelper(h);
            if (GetGroundType() == 0)
            {
                return indexPSAirborne;
            }

            if (h == 0) return indexPSIdle;
            return indexPSMoving;
        }


        return Index;
    }


    public override void OnStateEnter(State previousState)
    {

        var inDashingDragFactor = playerCharacter.InKillStreak ? f_inDashingDragFactor:n_inDashingDragFactor;
        if (playerCharacter.ConsumeEnergy(EnergyConsume) <= 0)
        {
            // Energy is not enough, Cancel dash
            Apply = false;
            return;
        }

        Apply = true;
        //Dash has been pressed, set all config first
        //After delay is over, dash perform


        // Add Ghost trail
        if(!playerCharacter.InKillStreak) playerCharacter.SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        lastDashSecond = Time.unscaledTime;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();

        // Record the default Drag of rb2d
        defaultDrag = rb2d.drag;
        readyToPush = true;

        // Kill all initial speed
        rb2d.velocity = new Vector2(0,0);
        hyperSpeed = true;

        // Kill gravity
        rb2d.gravityScale = 0;

        //Disable Collision
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"));

        // Strong Drag during dashing
        rb2d.drag *= inDashingDragFactor;

    }


    public override void OnStateQuit(State nextState)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"),false);

        // reset drag & gravity
        rb2d.drag = 1;
        rb2d.gravityScale = 3;

        // Listening to move input
        PhysicsInputHelper(h);

        playerCharacter.SpriteHolder.GetComponent<TrailRenderer>().emitting = false;

        // reset player facing
        //playerCharacter.GetComponent<CapsuleCollider2D>().isTrigger = false;
        playerCharacter.SpriteHolder.right = Vector3.right;

        // reset sprite flip
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipY = false;

        // Kill Trail
        if(!playerCharacter.InKillStreak) playerCharacter.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;


    }

    private void forceApply()
    {
        //avoid Slow Motion
        //TimeManager.Instance.endSlowMotion(0f);

        var dashForce = playerCharacter.InKillStreak ? f_dashForce:n_dashForce;

        // Fix sprite flip on X-axis
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = false;

        // Play Animation
        anim.Play("MainCharacter_Dashing", -1, 0f);
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();



        //get Mouse direction
        Vector3 direction = mouse.GetDirectionCorrection(GroundNormal());


        var attack = ObjectRecycler.Singleton.GetObject<SingleEffect>(6);
        attack.transform.position = playerCharacter.transform.position +  direction * 0.5f;
        
        attack.setTarget(playerCharacter.transform);
        attack.transform.parent = playerCharacter.transform;
        attack.transform.right = direction;
        attack.transform.localScale = new Vector3(4,4,1);
        attack.gameObject.SetActive(true);
        
        attack.GetComponentInChildren<HitBox>().hit.source = playerCharacter;

        var Dust = ObjectRecycler.Singleton.GetObject<SingleEffect>(9);
        Dust.transform.position = playerCharacter.transform.position +  direction * 0.5f;
        //Dust.setTarget(playerCharacter.transform);

        Dust.transform.right = -direction;
        Dust.transform.localScale = new Vector3(1,1,1);
        Dust.gameObject.SetActive(true);


        playerCharacter.SpriteHolder.GetComponent<TrailRenderer>().emitting = true;

        //set correct Y flip based on mouse direction
        if (direction.x < 0 && direction.y != 0) playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipY = true;



        //Apply force to character
       // playerCharacter.GetComponent<CapsuleCollider2D>().isTrigger = true;
        playerCharacter.SpriteHolder.right = direction;
        //Time.fixedDeltaTime = 0.02f;
        if (Time.timeScale!=0) rb2d.AddForce(direction * dashForce * 200f * 1 / Time.timeScale);
        else rb2d.AddForce(direction * dashForce * 200f * 1);

        //Camera Tricks

        CameraManager.Instance.Shaking(0.1f,0.10f);

    }


}
