using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping1", menuName = "Player State/Attack Jumping 1")]
public class PSJumping1 : PlayerState
{
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_Jumping2;
    private bool isJumpKeyDown = false;


    public override int Update()
    {
        float Vy = playerCharacter.GetComponent<Rigidbody2D>().velocity.y;

        if (Vy == 0)
        {
            RaycastHit2D hitM = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f, -0.35f, 0f), -playerCharacter.transform.up, 0.5f);
            RaycastHit2D hitR = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.1f, -0.35f, 0f), -playerCharacter.transform.up, 0.5f);
            RaycastHit2D hitL = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.1f, -0.35f, 0f), -playerCharacter.transform.up, 0.5f);

            if ((hitM.collider && hitM.transform.CompareTag("Ground")) || (hitR.collider && hitR.transform.CompareTag("Ground")) || (hitL.collider && hitL.transform.CompareTag("Ground")))
            {
                if (Input.GetAxis("Horizontal") == 0)
                    return index_PSIdle;

                return index_PSMoving;
            }
        }
        else if (Vy < jumpForce / 5)
        {
            if (Input.GetButtonDown("Jump"))
                return index_Jumping2;
        }

        isJumpKeyDown = Input.GetButtonDown("Jump");

        return Index;
    }

    public override void OnStateEnter()
    {
        isJumpKeyDown = true;

        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;

        V.y = jumpForce;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
