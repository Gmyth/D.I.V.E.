using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEffector : MonoBehaviour
{
    public enum PlatformType
    {
        WalkThrough = 0,
        Stair

    }
    private PlatformEffector2D effector;

    private float waitTime;

    public float initialWaitTime;

    private GameObject player;

    public float radius;

    // Start is called before the first frame update
    private void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {          
            if (CheckOnPlatform())
            {
                effector.rotationalOffset = 180;
            }
        }


        //if (PlayerCharacter.Singleton.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 )
        //{
        //    if (platformType == PlatformType.WalkThrough)
        //        effector.rotationalOffset = 0;
        //    else
        //    {
        //        effector.rotationalOffset = stairAngle;
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "PlatformDetector")
        {
            //collision.posit
            var detectorPos = collision.gameObject.transform.position;
            var platformPos = gameObject.transform.position;
            //detector is above the platform
            if ((detectorPos - platformPos).y > 0)
            {
                effector.rotationalOffset = 0;
            }
            else if((detectorPos - platformPos).y < 0)
            {
                effector.rotationalOffset = 0;
            }
        }
    }

    private bool CheckOnPlatform()
    {
        Debug.Log("Enter Check");
        RaycastHit2D hit = Physics2D.CircleCast(gameObject.transform.position, radius, new Vector2(0,1));
        if (hit.collider != null && hit.transform.CompareTag("Player"))
            return true;
        else
            return false;
    }
}
