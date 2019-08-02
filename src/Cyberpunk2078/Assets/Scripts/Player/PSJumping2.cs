using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Jumping2", menuName = "Player State/Attack Jumping 2")]
public class PSJumping2 : PlayerState
{
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;


    public override int Update()
    {
        if (playerCharacter.GetComponent<Rigidbody2D>().velocity.y == 0)
        {
            RaycastHit2D hitM = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f, -0.5f, 0f), -playerCharacter.transform.up, 0.5f);
            RaycastHit2D hitR = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.1f, -0.5f, 0f), -playerCharacter.transform.up, 0.5f);
            RaycastHit2D hitL = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.1f, -0.5f, 0f), -playerCharacter.transform.up, 0.5f);

            Debug.LogWarning(hitM.collider);
            Debug.LogWarning(hitM.transform.CompareTag("Ground"));

            Debug.LogWarning(hitR.collider);
            Debug.LogWarning(hitR.transform.CompareTag("Ground"));

            Debug.LogWarning(hitL.collider);
            Debug.LogWarning(hitL.transform.CompareTag("Ground"));

            if ((hitM.collider && hitM.transform.CompareTag("Ground")) || (hitR.collider && hitR.transform.CompareTag("Ground")) || (hitL.collider && hitL.transform.CompareTag("Ground")))
            {
                if (Input.GetAxis("Horizontal") == 0)
                    return index_PSIdle;

                return index_PSMoving;
            }
        }

        return Index;
    }

    public override void OnStateEnter()
    {
        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;

        V.y = jumpForce;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
