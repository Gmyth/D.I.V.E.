using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAudio : MonoBehaviour
{
    Rigidbody2D rdbd2d;

    [FMODUnity.EventRef]
    public string Moving;

    FMOD.Studio.EventInstance MovingEvent;
    FMOD.Studio.EventDescription MovingDesription;

    FMOD.Studio.PLAYBACK_STATE Movingstate;
    // Start is called before the first frame update
    void Start()
    {
        rdbd2d = GetComponent<Rigidbody2D>();
        MovingEvent = FMODUnity.RuntimeManager.CreateInstance(Moving);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(MovingEvent, gameObject.transform, rdbd2d);
        MovingEvent.setParameterValue("Speed", 0);
        //MovingEvent.setVolume(0);
        MovingEvent.start();
    }

    // Update is called once per frame 
    void FixedUpdate()
    {
        if (rdbd2d.velocity.magnitude > 0)
        {
            MovingEvent.getDescription(out MovingDesription);
            int count = 0;

            if (MovingDesription.getInstanceCount(out count) == FMOD.RESULT.OK && MovingEvent.getPlaybackState(out Movingstate) == FMOD.RESULT.OK)
            {
                Debug.Log("Count is：" + count);
                Debug.Log("Enter Check");
                Debug.Log(Movingstate.ToString());
                if (count <= 1)
                {
                    Debug.Log("Enter Start");
                    //MovingEvent.setVolume(1);
                    MovingEvent.setParameterValue("Speed", 1);
                }
            }
        }
        else
        {
            MovingEvent.setParameterValue("Speed", 0);
        }
    }

}
