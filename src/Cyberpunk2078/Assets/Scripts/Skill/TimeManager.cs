using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TimeManager Instance { get; private set; } = null;

    [SerializeField]private float releaseSmoothTime;
    [SerializeField]private float slowMotionFactor;
    private float startTime;
    private float endTime;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledTime > endTime)
        { 
            var newScale = Time.timeScale + (1f / releaseSmoothTime) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(newScale, 0f, 1f);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        
    }

    public void startSlowMotion(float duration)
    {
        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        startTime = Time.unscaledTime;
        endTime = startTime + duration;
    }
    
    public void endSlowMotion()
    {
        endTime = Time.unscaledTime;;
    }
}
