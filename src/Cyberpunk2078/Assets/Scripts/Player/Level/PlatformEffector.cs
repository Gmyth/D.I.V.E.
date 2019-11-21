using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformEffector : MonoBehaviour
{
    private float originalYPosition;
    private float originalXPosition;
    private bool movedUp = true;
    private bool movedDown = true;
    private bool movedLeft = true;
    private bool movedRight = true;
    public float offset = 1f;
    public float Smoothness;

    private PlatformEffector2D effector;
    private float waitTime;
    public float initialWaitTime;
    public bool occupied;
    private bool isController;

    // Start is called before the first frame update
    private void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
        originalYPosition = transform.position.y;
        originalXPosition = transform.position.x;
        Smoothness = Random.Range(0.3f, 0.6f);
        occupied = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S) && !isController)
        {
            waitTime = initialWaitTime;
            if (PlayerCharacter.Singleton.State.isGrounded())
            {
                movedUp = true;
                movedDown = true;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (occupied)
            {
                if (waitTime <= 0)
                {
                    effector.rotationalOffset = 180;
                    PlayerCharacter.Singleton.GetComponent<Rigidbody2D>().gravityScale = 3;
                    movedUp = false;
                    movedDown = false;
                    waitTime = initialWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }       
        }

        if (Input.GetAxis("VerticalJoyStick") >= 0 && isController)
        {
            waitTime = initialWaitTime;
            if (PlayerCharacter.Singleton.State.isGrounded())
            {
                movedUp = true;
                movedDown = true;
            }
        }
        if (Input.GetAxis("VerticalJoyStick") < -0.7f)
        {
            if (occupied)
            {
                if (waitTime <= 0)
                {
                    effector.rotationalOffset = 180;
                    PlayerCharacter.Singleton.GetComponent<Rigidbody2D>().gravityScale = 3;
                    movedUp = false;
                    movedDown = false;
                    waitTime = initialWaitTime;
                }
                else
                {
                    isController = true;
                    waitTime -= Time.deltaTime;
                }
            }
        }

        //Floating Platform
        if (movedUp)
        {
            Vector2 position = Vector2.Lerp(transform.position, new Vector2(originalXPosition, originalYPosition + offset), Smoothness * Time.deltaTime);
            transform.position = new Vector2(transform.position.x, position.y);
        }
        else if (movedDown)
        {
            Vector2 position = Vector2.Lerp((Vector2)(transform.position), new Vector2(originalXPosition, originalYPosition), Smoothness * Time.deltaTime);
            transform.position = new Vector2(transform.position.x, position.y);
        }

        if (transform.position.y - originalYPosition > 0.9 * offset)
        {
            movedUp = false;
        }
        else if (transform.position.y - originalYPosition <= 0.1)
        {
            movedUp = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "PlatformDetector")
        {
            occupied = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "PlatformDetector")
        {
            occupied = false;
            effector.rotationalOffset = 0;
        }
    }
}
