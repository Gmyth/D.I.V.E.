using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    [SerializeField] private string Moving;

    private FMOD.Studio.EventInstance MovingEvent;
    private FMOD.Studio.EventDescription MovingDesription;
    private FMOD.Studio.PLAYBACK_STATE Movingstate;
    private Rigidbody2D rdbd2d;

    // Update is called once per frame 
    void FixedUpdate()
    {
        if (rdbd2d.velocity.magnitude > 0)
        {
            Debug.LogWarning("V is" + rdbd2d.velocity.magnitude);

            MovingEvent.getDescription(out MovingDesription);

            int count = 0;

            MovingEvent.setParameterValue("Speed", 1);

            //if (MovingDesription.getInstanceCount(out count) == FMOD.RESULT.OK && MovingEvent.getPlaybackState(out Movingstate) == FMOD.RESULT.OK)
            //{
            //    if (count <= 1)
            //    {
                    
            //    }
            //}
        }
        else
        {
            MovingEvent.setParameterValue("Speed", 0);
        }
    }
    private void OnEnable()
    {
        rdbd2d = GetComponent<Rigidbody2D>();
        MovingEvent = FMODUnity.RuntimeManager.CreateInstance(Moving);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(MovingEvent, gameObject.transform, rdbd2d);
        MovingEvent.setParameterValue("Speed", 0);
        MovingEvent.start();
    }

    private void OnDisable()
    {
        MovingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
