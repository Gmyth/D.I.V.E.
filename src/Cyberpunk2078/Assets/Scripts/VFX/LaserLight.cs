using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserLight : MonoBehaviour
{
    // Start is called before the first frame update
    
    private float MoveAngle; 
    //private float idleInterval = Random.Range(0.05f,0.2f);
    
    private bool flash;
    private bool moving;

    
    private float acceptableBeat;
    private float lastIdleStart;
    private float lastFlashBeat;
    private float beatCounter;
    private float MoveSpeed;
    private float beatDivider;

    void Start()
    {
        beatCounter = 0;
        MoveAngle= Random.Range(15,40);
        beatDivider = 2;
        acceptableBeat = Random.Range(0, 1);

        BeatSystem.onBeat.AddListener(onBeat);
        //AudioProcessor.Instance.onBeat.AddListener(onBeat);
    }

    void onBeat()
    {
        MoveSpeed = Random.Range(0.5f, 2f);
        beatCounter++;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (beatCounter % beatDivider == acceptableBeat)
        {
            // In Move
             if (flash) 
             {
//                 flash = false;
//                  var color = gameObject.GetComponentInChildren<SpriteRenderer>().color;
//                 gameObject.GetComponentInChildren<SpriteRenderer>().color = 
//                     new Color(color.r, color.g, color.b, Random.Range(0.4f,0.6f));
                 
                  transform.rotation = Quaternion.Euler(0,0,MoveAngle/2 + MoveAngle/2 * Mathf.Cos(Time.time * MoveSpeed));
             }
            
           
            
        }else{
            // In Idle
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
        }

    }
    
}
