using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dashing", menuName = "Player State/Dashing")]
public class PSDashing : PlayerState
{
    [SerializeField] private float dashForce = 8;
    [SerializeField] private float dashReleaseTime = 0.22f;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_Jumping2;
    [SerializeField] private int index_WallJumping;
    private float lastDashSecond;
    private bool hyperSpeed;

    public override int Update()
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        float Vy = rb2d.velocity.y;
        float h = Input.GetAxis("Horizontal");
        //Still support Horizontal update during jumping, delete following to kill Horizzontal input
        if (lastDashSecond + dashReleaseTime < Time.unscaledTime)
        {
            if (hyperSpeed)
            {
                hyperSpeed = false;
                rb2d.velocity = rb2d.velocity * 0.4f;
            }

            rb2d.gravityScale = 3;
            PhysicsInputHelper(h);
        }

        if (Input.GetAxis("Dashing") != 0 && lastDashSecond+dashReleaseTime < Time.unscaledTime)
            OnStateEnter();

        if (isGrounded() && Vy <= 0)
        {
            rb2d.gravityScale = 3;
            // Landed
            if (h == 0)
                return index_PSIdle;

            return index_PSMoving;
        }
        
        //Player is sill in air
        if (!isGrounded())
        {
            rb2d.gravityScale = 3;
            // prevent misclicking
            if (Input.GetButtonDown("Jump") && lastDashSecond+dashReleaseTime < Time.unscaledTime)
                // second_jump
                return index_Jumping2;
        }

       

        //isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter()
    {
        //Perform jump
        lastDashSecond = Time.unscaledTime;
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        Vector3 direction = mouse.getAttackDirection();
        rb2d.velocity = new Vector2(0,0);
        rb2d.AddForce(direction * dashForce * 100f);
        hyperSpeed = true;
        rb2d.gravityScale = 0;
    }
}
