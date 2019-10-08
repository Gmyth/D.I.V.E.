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

    public PlatformType platformType;

    public float stairAngle;

    private GameObject player;

    // Start is called before the first frame update
    private void Start()
    {
        effector = gameObject.GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            waitTime = initialWaitTime;
            
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (waitTime <= 0)
            {
                if(platformType == PlatformType.WalkThrough)
                    effector.rotationalOffset = 180;
                else
                {
                    effector.rotationalOffset = -180 + stairAngle;
                }
                waitTime = initialWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }

        }

        if (PlayerCharacter.Singleton.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 )
        {
            if (platformType == PlatformType.WalkThrough)
                effector.rotationalOffset = 0;
            else
            {
                effector.rotationalOffset = stairAngle;
            }
        }
    }


}
