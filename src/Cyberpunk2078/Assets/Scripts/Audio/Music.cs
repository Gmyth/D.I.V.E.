using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Music : MonoBehaviour
{
    //[FMODUnity.EventRef]
    //[SerializeField] private string BGM;

    private FMOD.Studio.EventInstance instance;
    private BeatSystem bS;

    void Start()
    {
        bS = GetComponent<BeatSystem>();

        if (AudioManager.Singleton.PlayEvent("BGM"))
        {
            instance = AudioManager.Singleton.GetEventInstance("BGM");
        }
       
        bS.AssignBeatEvent(instance);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    instance = FMODUnity.RuntimeManager.CreateInstance(BGM);
        //    instance.start();
        //    bS.AssignBeatEvent(instance);
        //}

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            bS.StopAndClear(instance);
        }
    }
}
