﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using System.Runtime.InteropServices;
using static FMOD.Studio.STOP_MODE;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton { get; private set; } = null;

    //dic to match acutal fmod string with sound string
    private Dictionary<string, string> dic;

    private Dictionary<string, EventInstance> instanceDic;

    private EventInstance eventInsatance;
    private FMOD.Studio.EventDescription MovingDesription;

    [SerializeField] private AudioData[] AudioData;

    // Start is called before the first frame update
    void Awake()
    {
        dic = new Dictionary<string, string>();
        instanceDic = new Dictionary<string, EventInstance>();
        Singleton = this;

        for(int i=0; i < AudioData.Length; ++i)
        {
            foreach (string s in AudioData[i].events)
            {
                int pFrom = s.LastIndexOf("/") + "/".Length;
                int pTo = s.Length;

                string result = s.Substring(pFrom, pTo - pFrom);
                dic.Add(result, s);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParameter(string _event, string para, float value)
    {
        EventInstance ins = GetEventInstance(_event);
        ins.setParameterValue(para, value);
    }

    public EventInstance GetEventInstance(string _event)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {
                return instanceDic[_event];
            }
        }

        Debug.LogError("Can't get eventinstance");
        return FMODUnity.RuntimeManager.CreateInstance(dic[_event]);
    }

    public bool PlayEvent(string _event)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {
                return false;
            }
            else
            {
                EventInstance ins = FMODUnity.RuntimeManager.CreateInstance(dic[_event]);
                instanceDic.Add(_event, ins);
                ins.start();
                ins.release();
                return true;
            }     
        }
        else
        {
            Debug.LogError("[AudioManager] " + " Can't find " + _event);
            return false;
        }
    }
    public void StopBus(string busName)
    {
        Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/" + busName);

        bus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopEvent(string _event, bool fade = true)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {
                instanceDic[_event].release();
                if(fade == true)
                    instanceDic[_event].stop(ALLOWFADEOUT);
                else
                    instanceDic[_event].stop(IMMEDIATE);

                instanceDic.Remove(_event);
            }
            else
            {
                //Debug.LogError("[AudioManager] " + " Can't release " + _event);
            }
        }
        else
        {
            Debug.LogError("[AudioManager] " + " Can't find " + _event);
        }
    }

    public void ReleaseEvent(string _event)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {
                instanceDic[_event].release();
                //instanceDic[_event].stop(ALLOWFADEOUT);
                instanceDic.Remove(_event);
            }
            else
            {
                //Debug.LogError("[AudioManager] " + " Can't release " + _event);
            }
        }
        else
        {
            Debug.LogError("[AudioManager] " + " Can't find " + _event);
        }
    }


    public void PlayOnce(string _event, string param = null, float value = 0)
    {
        if (dic.ContainsKey(_event))
        {
            EventInstance ins = FMODUnity.RuntimeManager.CreateInstance(dic[_event]);

            if(param != null)
            {
                ins.setParameterValue(param, value);
            }
            
            ins.start();
            ins.release();
        }
        else
        {
            Debug.LogError("[AudioManager] " + " Can't find " + _event);
        }
    }

    
}
