﻿using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; } = null;
    
    [SerializeField]private float slowMotionFactor;
    [SerializeField]private float targetFXAlpha;

    private float velocity;
    private SpriteRenderer blackScreen;
    private float startTime;
    private float endTime;
    private float targetScale;
    private float smoothTime;
    private bool triggered;
    public float TimeFactor = 1;

    private float _PrirorTimeScale;
    private float currentUnscaledDeltaTime;

    public float ScaledDeltaTime
    {
        get
        {
            return Time.deltaTime * TimeFactor;
        }
    }
    

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
        
        Instance = this;
        blackScreen = Camera.main.GetComponentInChildren<SpriteRenderer>();
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale / 60f;
    }
    
    public void startSlowMotionBlink(float duration, float scale){
        Time.timeScale = scale >= 0 ? scale: slowMotionFactor;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StartCoroutine(endSlowMotionDelay(duration));
    }

    public void startSlowMotion(float duration, float scale = -1f, float finishTime = 0f,float delta = 0.1f)
    {
        targetScale = scale >= 0 ? scale: slowMotionFactor;
        StartCoroutine(slowMotion(delta, targetScale, finishTime, duration));
    }

    public void ApplyBlackScreenFadeIn()
    {
        StartCoroutine(blackMask());
    }
    
    public void ApplyBlackScreen()
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, targetFXAlpha);
    }
    
    private IEnumerator blackMask()
    {
        while (blackScreen.color.a < targetFXAlpha)
        {
            float current = blackScreen.color.a;
            blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
            yield return null;
        }
    } 
    
    private IEnumerator slowMotion(float delta, float targetScale,float duration,float endDuration)
    {
        if (targetScale == 0) Time.timeScale = 0.5f; // lower start
        var times = (int)(duration / delta);
        var deltaScale = (Time.timeScale - targetScale)/times;
        if (duration == 0) Time.timeScale = targetScale;
        for (int i = 0; i < times; i++)
        {
            Time.timeScale =  Mathf.Max(targetScale,Time.timeScale - deltaScale);
            yield return new WaitForSecondsRealtime(delta);
        }

        if (endDuration > 0)
        {
            endSlowMotion(endDuration);
        }
    }    

    public void endSlowMotion(float delay = 0f)
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
        if (delay == 0)
            EndSlowMotionImmediately();
        else
            StartCoroutine(endSlowMotionDelay(delay));
    }

    public void EndSlowMotionImmediately()
    {
        Time.timeScale = 1;
        triggered = false;
    }
    
    public void StartFeverMotion()
    {
        TimeFactor = 0.2f;
        StartCoroutine(applyBlackScreen());
    }

    public void EndFeverMotion()
    {
        TimeFactor = 1f;
        blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, 0f);
    }

    private IEnumerator applyBlackScreen()
    {

        while (TimeFactor!= 1f)
        {
            float current = blackScreen.color.a;
            yield return new WaitForSecondsRealtime(0.01f);
            blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
            yield return null;
        }
        
        blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, 0f);
        
        yield return null;
    }

    private IEnumerator endSlowMotionDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        EndSlowMotionImmediately();
    }

    public void Pause()
    {
        _PrirorTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = _PrirorTimeScale;
    }
}
