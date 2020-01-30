using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserLight : MonoBehaviour
{
    // Start is called before the first frame update
    
    private float MoveAngle;
    private float bpm = 120;  //TODO: Audio
    private float idleInterval = 0.2f;
    
    private bool flash;
    private bool moving;

    private float moveInterval;
    
    private float beatStartTime;
    private float lastIdleStart;
    private float lastFlashBeat;
    private float beatCounter;
    private float MoveSpeed;
    void Start()
    {
        moveInterval = 60 / bpm - idleInterval;
        beatCounter = 0;
        MoveAngle= Random.Range(15,40);
    }

    // Update is called once per frame
    void Update()
    {
        if (lastFlashBeat != beatCounter)
        {
            var color = gameObject.GetComponentInChildren<SpriteRenderer>().color;
            if (flash)
            {
                flash = false;
                gameObject.GetComponentInChildren<SpriteRenderer>().color = 
                    new Color(color.r, color.g, color.b, Random.Range(0.4f,0.6f));
            }
            else
            {
                if (Random.Range(0, 100) < 60)
                {
                    // time to flash
                    gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0f);
                    flash = true;
                }
            }

            lastFlashBeat = beatCounter;
        }
        
        if (beatStartTime + moveInterval > Time.time)
        {
            // In Move
            transform.rotation = Quaternion.Euler(0,0,MoveAngle/2 + MoveAngle/2 * Mathf.Cos(Time.time * MoveSpeed));
            
        }else if (beatStartTime + moveInterval + idleInterval > Time.time)
        {
            // In Idle
            //do nothing
        }
        else
        {
            MoveSpeed = Random.Range(1, 3);
            //new set Start
            beatStartTime = Time.time;
            beatCounter++;
        }
        
    }
    
}
