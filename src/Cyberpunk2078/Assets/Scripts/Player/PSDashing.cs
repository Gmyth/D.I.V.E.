using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dashing", menuName = "Player State/Dashing")]
public class PSDashing : PlayerState
{
    [SerializeField] private float dashForce = 8; // Dash Initial speed
    [SerializeField] private float dashDelayTime = 0.05f; // time from button press to actual dash
    [SerializeField] private float dashReleaseTime = 0.22f; // time to recover rb2d setting after dashing process finished
    [SerializeField] private float dashReleaseDelayTime = 0.05f; // time from dash finish to actual controllable
    [SerializeField] private float inDashingDragFactor = 3; // In-dash drag factor
    [SerializeField] private float EnergyConsume = -70; // In-dash drag factor
    
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexJumping2;
    [SerializeField] private int indexWallJumping;
    [SerializeField] private int indexPSAirborne;
    
    private float lastDashSecond;
    private bool hyperSpeed;
    private float defaultDrag;
    private bool readyToPush;
    private bool Apply = true;


    public override int Update()
    {
        
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input

        if (!Apply)
        {
            if (!isGrounded())
            {
                return indexPSAirborne;
            }
            
            if (h == 0) return indexPSIdle;
            return indexPSMoving;
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
                rb2d.velocity = rb2d.velocity * 0.1f;
            }
        } else if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime < Time.unscaledTime)
        {
            // the dash has already ended
            // ok for move input
            PhysicsInputHelper(h);
        }
        else {
            //prevent ground-hitting shifting 
            RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position,rb2d.velocity.normalized,0.5f);
            if (hit1.collider != null && hit1.transform.CompareTag("Ground"))
            {
                // kill all speed
                rb2d.velocity = new Vector2(0,0);
                
                //reset direction
                playerCharacter.transform.right = Vector3.right;
                
                // reset drag & gravity 
                rb2d.drag = defaultDrag;
                rb2d.gravityScale = 3;
                
                // Disable trails
                playerCharacter.GetComponent<GhostSprites>().RenderOnMotion = false;
                // Landed
                if (h == 0)
                    // not moving
                    return indexPSIdle;
                return indexPSMoving;
            }
            
        }
        
        // Player is grounded and dash has finished
        if (lastDashSecond + dashReleaseTime + dashDelayTime + dashReleaseDelayTime  < Time.unscaledTime)
        {
            // reset drag & gravity 
            rb2d.drag = defaultDrag;
            rb2d.gravityScale = 3;
            
            // Listening to move input
            PhysicsInputHelper(h);
            
            // reset player facing
            playerCharacter.transform.right = Vector3.right;
            
            // reset sprite flip
            playerCharacter.GetComponent<SpriteRenderer>().flipY = false;
            
            // Kill Trail
            playerCharacter.GetComponent<GhostSprites>().Occupied = false;
            
            if (!isGrounded())
            {
                return indexPSAirborne;
            }
            
            if (h == 0) return indexPSIdle;
            return indexPSMoving;
        }

        return Index;
    }

    public override void OnStateEnter()
    {
        if (!Player.CurrentPlayer.ApplyEnergyChange(EnergyConsume))
        {
            // Energy is not enough, Cancel dash
            Apply = false;
            return;
        }

        Apply = true;
        //Dash has been pressed, set all config first
        //After delay is over, dash perform
        
        // Add Ghost trail
        playerCharacter.GetComponent<GhostSprites>().Occupied = true;
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
        
        // Strong Drag during dashing
        rb2d.drag *= inDashingDragFactor;
    }

    private void forceApply()
    {
        // Fix sprite flip on X-axis
        playerCharacter.GetComponent<SpriteRenderer>().flipX = false;
        // Play Animation
        anim.Play("MainCharacter_Dashing", -1, 0f);
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        
        //get Mouse direction
        Vector3 direction = getDirectionCorrection(mouse.getAttackDirection(),GroundNormal());
        
        //set correct Y flip based on mouse direction
        if (direction.x < 0 && direction.y != 0) playerCharacter.GetComponent<SpriteRenderer>().flipY = true;
        
        //Apply force to character
        playerCharacter.transform.right = direction;
        rb2d.AddForce(direction * dashForce * 100f);
        
        //Camera Tricks
        CameraManager.Instance.Shaking(0.08f,0.15f);
    }
}
