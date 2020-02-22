using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
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

    private float lastDashSecond;
    private bool hyperSpeed;
    private float defaultDrag;
    private bool readyToPush;
    private bool Apply = true;
    private float lastJumpInput;


    public override string Update()
    {
        var dashDelayTime = playerCharacter.InKillStreak ? f_dashDelayTime:n_dashDelayTime;
        var dashReleaseTime = playerCharacter.InKillStreak ? f_dashReleaseTime:n_dashReleaseTime;
        var dashReleaseDelayTime = playerCharacter.InKillStreak ? f_dashReleaseTime:n_dashReleaseTime;
        var JumpListenerInterval = playerCharacter.InKillStreak ? f_JumpListenerInterval:n_JumpListenerInterval;
        
        if (playerCharacter.PowerDash)
        {
            dashDelayTime = 0.3f;
            dashReleaseTime *= 2.15f;
        }
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("HorizontalJoyStick") != 0 ? Input.GetAxis("HorizontalJoyStick") : Input.GetAxis("Horizontal");
        float v = Input.GetAxis("VerticalJoyStick") != 0 ? Input.GetAxis("VerticalJoyStick") : Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Ultimate"))
        {
            //TODO add another ultimate
            playerCharacter.ActivateFever();
        }
        
        if (!Apply)
        {
            if (GetGroundType() == 0)
                return "Airborne";

            if (h == 0)
                return "Idle";

            return "Moving";
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (Player.CurrentPlayer.secondJumpReady && lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.time)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                return "Jumping";
            }
            else
            {
                // save jump for later
                lastJumpInput = Time.time;
            }
        }

        if (lastDashSecond + dashDelayTime < Time.time && readyToPush)
        {
            // Press delay ends. time to actual dash
            readyToPush = false;
            forceApply();
        }


        if (lastDashSecond + dashReleaseTime + dashDelayTime < Time.time)
        {
            // the dash has already ended
            if (hyperSpeed)
            {
                // kill speed after dash
                hyperSpeed = false;
                rb2d.drag = defaultDrag;
                //rb2d.velocity = rb2d.velocity * 0.3f;
                var volX = h * rb2d.velocity.x > 0?rb2d.velocity.x * 0.3f:0;
                var volY = rb2d.velocity.y * 0.1f;
                rb2d.velocity = new Vector2(volX,volY);
            }
            PhysicsInputHelper(h,v);

        } else if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.time)
        {
            //cast listened jump right after dash finish
            if (lastJumpInput + JumpListenerInterval > Time.time)
            {
                Player.CurrentPlayer.secondJumpReady = false;
                return "Jumping";
            }

            // the dash has already ended
            // ok for move input
            PhysicsInputHelper(h,v);

            //enable Collision
            playerCharacter.transform.right = Vector3.right;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"), false);
        }

        //prevent ground-hitting shifting
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,1.5f);
        if (hit1.collider != null)
        {
            if(hit1.transform.CompareTag("Ground")){
                if( Mathf.Abs(hit1.normal.x) > 0f)rb2d.velocity = new Vector2(rb2d.velocity.x * 0.4f, 0 );
                if( Mathf.Abs(hit1.normal.y) > 0f)rb2d.velocity =  new Vector2(0, rb2d.velocity.y * 0.8f);
            }
            else if(hit1.transform.CompareTag("Platform") && rb2d.velocity.normalized.y < 0)
            {
                //upper ward
                if( Mathf.Abs(hit1.normal.x) > 0f)rb2d.velocity = new Vector2(rb2d.velocity.x * 0.4f, 0 );
                if( Mathf.Abs(hit1.normal.y) > 0f)rb2d.velocity =  new Vector2(0, rb2d.velocity.y * 0.8f);
            }
        }


        // Player is grounded and dash has finished
        if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime  < Time.time)
        {
//            var volX = h * rb2d.velocity.x > 0?rb2d.velocity.x * 0.3f:0;
//            var volY = rb2d.velocity.y > 0?rb2d.velocity.y * 0.2f : rb2d.velocity.y;
//            rb2d.velocity = new Vector2(volX,volY);
            PhysicsInputHelper(h,v);

            if (GetGroundType() == 0)
                return "Airborne";

            if (h == 0)
                return "Idle";

            return "Moving";
        }
        
        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        var inDashingDragFactor = playerCharacter.InKillStreak ? f_inDashingDragFactor:n_inDashingDragFactor;
        if (playerCharacter.PowerDash)
        {
            if (!playerCharacter.PowerDashReady)
            {
                // Energy is not enough, Cancel dash
                Apply = false;
                AudioManager.Singleton.PlayOnce("Negative_dash");
                return;
            }


            playerCharacter.PowerDashReady = false;
            playerCharacter.LastPowerDash = Time.time;
            TimeManager.Instance.StartFeverMotion();
            playerCharacter.Spark.SetActive(true);
            playerCharacter.Spark.GetComponent<Animator>().Play("Spark", -1, 0f);
            
            var mouse = GameObject.FindObjectOfType<MouseIndicator>();
            Vector3 direction = mouse.GetDirectionCorrection(GroundNormal());
            playerCharacter.Spark.transform.right = direction;
        }
        else
        {
            if (playerCharacter.ConsumeEnergy(EnergyConsume) <= 0)
            {
                // Energy is not enough, Cancel dash
                Apply = false;
                AudioManager.Singleton.PlayOnce("Negative_dash");
                return;
            }
        }

        AudioManager.Singleton.PlayOnce("Dash");


        Apply = true;
        //Dash has been pressed, set all config first
        //After delay is over, dash perform


        // Add Ghost trail
        if(!playerCharacter.InKillStreak) playerCharacter.SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        lastDashSecond = Time.time;
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
        float v = Input.GetAxis("VerticalJoyStick") != 0 ? Input.GetAxis("VerticalJoyStick") : Input.GetAxis("Vertical");

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dummy"),false);
        
        // reset drag & gravity
        rb2d.drag = 1;
        rb2d.gravityScale = playerCharacter.Gravity;

        // Listening to move input
        PhysicsInputHelper(h,v);
        
        playerCharacter.Spark.SetActive(false);
        
        playerCharacter.SpriteHolder.GetComponent<TrailRenderer>().emitting = false;

        // reset player facing
        //playerCharacter.GetComponent<CapsuleCollider2D>().isTrigger = false;
        playerCharacter.SpriteHolder.right = Vector3.right;

        // reset sprite flip
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipY = false;

        // Kill Trail
        if(!playerCharacter.InKillStreak) playerCharacter.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;

        if (playerCharacter.PowerDash) playerCharacter.PowerDash = false;
    }

    private void forceApply()
    {
        //avoid Slow Motion
        //TimeManager.Instance.endSlowMotion(0f);
        if(!playerCharacter.InFever)TimeManager.Instance.EndFeverMotion();
        else TimeManager.Instance.StartFeverMotion();
        
        
        var dashForce = playerCharacter.InKillStreak ? f_dashForce:n_dashForce;

        // Fix sprite flip on X-axis
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().flipX = false;

        // Play Animation
        anim.Play("MainCharacter_Dashing", -1, 0f);
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();

        playerCharacter.Spark.SetActive(false);

        //get Mouse direction
        Vector3 direction = mouse.GetDirectionCorrection(GroundNormal());


        var attack = ObjectRecycler.Singleton.GetObject<SingleEffect>(6);
        attack.GetComponentInChildren<HitBox>().hit.source = playerCharacter;
        attack.setTarget(playerCharacter.transform);
        attack.transform.position = playerCharacter.transform.position + direction * 0.5f;
        attack.transform.parent = playerCharacter.transform;
        attack.transform.right = direction;
        attack.transform.localScale = new Vector3(4,4,1);
        attack.gameObject.SetActive(true);
        

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

        if (playerCharacter.PowerDash)
        {
            rb2d.AddForce(direction * dashForce * 200f * 1.8f);
        }
        else
        {
            rb2d.AddForce(direction * dashForce * 200f * 1);
        }

        //Camera Tricks

        CameraManager.Instance.Shaking(0.1f,0.10f);
    }
}
