using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformEffector : MonoBehaviour
{
    private PlatformEffector2D effector;
    private float waitTime;
    private bool isController;

    public float initialWaitTime;
    public bool occupied;
    

    // Start is called before the first frame update
    private void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
        occupied = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //Keyboard
        if (Input.GetKeyUp(KeyCode.S) && !isController)
        {
            waitTime = initialWaitTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (occupied)
            {
                if (waitTime <= 0)
                {
                    effector.rotationalOffset = 180;
                    AudioManager.Singleton.PlayEvent("Traverse_down");
                    PlayerCharacter.Singleton.GetComponent<Rigidbody2D>().gravityScale = 3;
                    waitTime = initialWaitTime;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                }
            }       
        }

        //Controller
        if (Input.GetAxis("VerticalJoyStick") >= 0 && isController)
        {
            waitTime = initialWaitTime;
        }
        if (Input.GetAxis("VerticalJoyStick") < -0.7f)
        {
            if (occupied)
            {
                if (waitTime <= 0)
                {
                    effector.rotationalOffset = 180;
                    AudioManager.Singleton.PlayEvent("Traverse_down");
                    PlayerCharacter.Singleton.GetComponent<Rigidbody2D>().gravityScale = 3;
                    waitTime = initialWaitTime;
                }
                else
                {
                    isController = true;
                    waitTime -= Time.deltaTime;
                }
            }
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
            AudioManager.Singleton.StopEvent("Traverse_down");
            effector.rotationalOffset = 0;
        }
    }
}
