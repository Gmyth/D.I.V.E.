using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using static FMOD.Studio.STOP_MODE;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null;

    [FMODUnity.EventRef] 
    [SerializeField] private List<string> events;

    //dic to match acutal fmod string with sound string
    private Dictionary<string, string> dic;

    private Dictionary<string, EventInstance> instanceDic;

    private EventInstance eventInsatance;
    private FMOD.Studio.EventDescription MovingDesription;
    

    // Start is called before the first frame update
    void Awake()
    {
        dic = new Dictionary<string, string>();
        instanceDic = new Dictionary<string, EventInstance>();
        Instance = this;

        foreach (string s in events)
        {
            int pFrom = s.LastIndexOf("/") + "/".Length;
            int pTo = s.Length;

            string result = s.Substring(pFrom, pTo - pFrom);
            dic.Add(result, s);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayEvent(string _event)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {

            }
            else
            {
                EventInstance ins = FMODUnity.RuntimeManager.CreateInstance(dic[_event]);
                instanceDic.Add(_event, ins);
                ins.start();

            }     
        }
        else
        {
            Debug.LogError("[AudioManager] " + " Can't find " + _event);
        }
    }

    public void StopEvent(string _event)
    {
        if (dic.ContainsKey(_event))
        {
            if (instanceDic.ContainsKey(_event))
            {
                instanceDic[_event].release();
                instanceDic[_event].stop(ALLOWFADEOUT);
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
