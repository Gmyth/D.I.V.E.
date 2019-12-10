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
    [SerializeField]private float targetFXAlpha;
    private float velocity;
    private SpriteRenderer blackScreen;
    private float startTime;
    private float endTime;

    private bool triggered;
    void Awake()
    {
        Instance = this;
        blackScreen = Camera.main.GetComponentInChildren<SpriteRenderer>();
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (triggered)
        {
            if (Time.unscaledTime < endTime)
            { 
                var newScale = Time.timeScale - (1f / releaseSmoothTime) * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Clamp(newScale, slowMotionFactor, 1f);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                float current = blackScreen.color.a;
                blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
            }
            else if(Time.unscaledTime >= endTime)
            {
                triggered = false;
                Time.timeScale = 1;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, 0f);
            }
        }
    }

    public void startSlowMotion(float duration)
    {
        startTime = Time.unscaledTime;
        endTime = Time.unscaledTime + duration;
        triggered = true;
    }

    public void endSlowMotion(float delay = 0.02f)
    {
        StartCoroutine(endSlowMotionDelay(delay));
    }

    private IEnumerator endSlowMotionDelay(float delay)
    {
        
        yield return  new WaitForSeconds(delay);
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, 0f);
        triggered = false;
    }
}
