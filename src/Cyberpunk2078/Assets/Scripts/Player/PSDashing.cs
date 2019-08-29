using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dashing", menuName = "Player State/Dashing")]
public class PSDashing : PlayerState
{
    [SerializeField] private float dashForce = 8;
    [SerializeField] private float dashReleaseTime = 0.22f; // time to recover rb2d setting after entire dash finished
    
    [SerializeField] private float inDashingDragFactor = 3;
    
  
    [SerializeField] private float dashDelayTime = 0.05f; // time from button press to actual dash

    [SerializeField] private float dashReleaseDelayTime = 0.05f; // time from button press to actual dash

    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexJumping2;
    [SerializeField] private int indexWallJumping;
    
    private float lastDashSecond;
    private bool hyperSpeed;
    private float defaultDrag;
    private bool readyToPush;


    public override int Update()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input

        if (lastDashSecond + dashDelayTime < Time.unscaledTime && readyToPush)
        {
            // Press delay ends. time to push
            readyToPush = false;
            forceApply();
        }
        
        
        if (lastDashSecond + dashReleaseTime + dashDelayTime < Time.unscaledTime)
        {
            // the dash has already ended
            if (hyperSpeed)
            {
                hyperSpeed = false;
                rb2d.velocity = rb2d.velocity * 0f;
            }
            
        } else if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.unscaledTime)
        {
            // the dash has already ended
            rb2d.drag = defaultDrag;
            rb2d.gravityScale = 3;
            PhysicsInputHelper(h);
        }
        else {
            //prevent ground-hitting shifting 
            RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,1.5f);
     
            if (hit1.collider != null && hit1.transform.CompareTag("Ground") )
            {
                rb2d.velocity = new Vector2(0,0);
                rb2d.drag = defaultDrag;
                rb2d.gravityScale = 3;
                // Landed
                if (h == 0)
                    return indexPSIdle;

                return indexPSMoving;
            }
        }



//        if (Input.GetAxis("Dashing") != 0 && lastDashSecond+dashReleaseTime < Time.unscaledTime)
//            OnStateEnter();

        if (isGrounded() && Vy <= 0 && lastDashSecond+dashReleaseTime < Time.unscaledTime)
        {
            rb2d.drag = defaultDrag;
            rb2d.gravityScale = 3;
            // Landed
            if (h == 0)
                return indexPSIdle;

            return indexPSMoving;
        }
        
        //Player is sill in air
        if (!isGrounded())
        {
            rb2d.drag = defaultDrag;
            rb2d.gravityScale = 3;
            // prevent misclicking
            if (Input.GetButtonDown("Jump") && lastDashSecond+dashReleaseTime < Time.unscaledTime)
                // second_jump
                return indexJumping2;
        }

       

        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter()
    {
        //Perform Dash
        lastDashSecond = Time.unscaledTime;
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        defaultDrag = defaultDrag == 0 ? rb2d.drag:defaultDrag;
        readyToPush = true;
        rb2d.velocity = new Vector2(0,0);
        hyperSpeed = true;
        rb2d.gravityScale = 0;
        rb2d.drag *= inDashingDragFactor;
    }

    private void forceApply()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        Vector3 direction = mouse.getAttackDirection();
        rb2d.AddForce(direction * dashForce * 100f);
        
    }
}
