using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMoving : MonoBehaviour
{
    [FMODUnity.EventRef]
    [SerializeField] private string Moving;

    private FMOD.Studio.EventInstance MovingEvent;

    private Rigidbody2D rdbd2d;

    private bool isPlaying = false;

    private bool inCamera = false;

    // Update is called once per frame 
    void FixedUpdate()
    {
        if (rdbd2d.velocity.magnitude > 1f)
            MovingEvent.setParameterValue("Speed", 1);
        else
            MovingEvent.setParameterValue("Speed", 0);

        if(inCamera == true)
        {
            int i = CreateInstanceWithCount();
            if (i == 0)
            {
                Play();
            }
        }
    }
    private void OnEnable()
    {
        rdbd2d = GetComponent<Rigidbody2D>();
    }

    private void OnBecameVisible()
    {
        inCamera = true;

        int i = CreateInstanceWithCount();
        if(i == 0)
        {
            Play();
        } 
    }

    private void OnBecameInvisible()
    {
        inCamera = false;
        if (isPlaying == true)
        {
            MovingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            FMODUnity.RuntimeManager.DetachInstanceFromGameObject(MovingEvent);
            MovingEvent.release();
            isPlaying = false;
            
        }
    }

    private void OnDisable()
    {
        MovingEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    private int CreateInstanceWithCount()
    {
        FMOD.Studio.EventDescription ed;
        ed = FMODUnity.RuntimeManager.GetEventDescription(Moving);
        ed.getInstanceCount(out int count);

        return count;
    }

    private void Play()
    {
        MovingEvent = FMODUnity.RuntimeManager.CreateInstance(Moving);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(MovingEvent, gameObject.transform, rdbd2d);
        MovingEvent.setParameterValue("Speed", 0);
        MovingEvent.start();
        isPlaying = true;
    }
}
