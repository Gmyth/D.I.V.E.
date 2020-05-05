using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; } = null;
    
    [SerializeField]private float slowMotionFactor;
    [SerializeField]private float targetFXAlpha;

    private float velocity;
    [SerializeField] private SpriteRenderer BM_Ground;
    [SerializeField] private SpriteRenderer BM_ForeGround;
    private float startTime;
    private float endTime;
    private float targetScale;
    private float smoothTime;
    private bool triggered;
    public float TimeFactor = 1;

    private float _PrirorTimeScale;
    private float currentUnscaledDeltaTime;

    private bool paused = false;

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
        //blackScreenUnderPlayer = Camera.main.GetComponentInChildren<SpriteRenderer>();
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

    public void ApplyBlackScreenFadeIn(float _alpha = -1, bool isForeGround = false)
    {
        float alpha = _alpha > 0 ? _alpha : targetFXAlpha;
        var render = isForeGround ? BM_ForeGround : BM_Ground;
        StartCoroutine(blackMask(alpha,render));
    }
    
    public void EndBlackScreen(bool isForeGround = false)
    {
        var render = isForeGround ? BM_ForeGround : BM_Ground;
        render.color =new Color(BM_Ground.color.r,BM_Ground.color.g,BM_Ground.color.b, 0f);
    }
    
    public void ApplyBlackScreen()
    {
        BM_Ground.color = new Color(BM_Ground.color.r, BM_Ground.color.g, BM_Ground.color.b, targetFXAlpha);
    }
    
    private IEnumerator blackMask(float alpha, SpriteRenderer renderer)
    {
        while (renderer.color.a < alpha)
        {
            float current = renderer.color.a;
            renderer.color =new Color(renderer.color.r,renderer.color.g,renderer.color.b, Mathf.SmoothDamp(current, alpha, ref velocity, 0.1f));
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

    public void endSlowMotion(float delay = 0f, bool rageQuit = false)
    {
        if (!rageQuit && Time.timeScale == 0) return;
        BM_Ground.color = new Color(BM_Ground.color.r, BM_Ground.color.g, BM_Ground.color.b, 0f);
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
        BM_Ground.color =new Color(BM_Ground.color.r,BM_Ground.color.g,BM_Ground.color.b, 0f);
    }

    private IEnumerator applyBlackScreen()
    {

        while (TimeFactor!= 1f)
        {
            float current = BM_Ground.color.a;
            yield return new WaitForSecondsRealtime(0.01f);
            BM_Ground.color =new Color(BM_Ground.color.r,BM_Ground.color.g,BM_Ground.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
            yield return null;
        }
        
        BM_Ground.color =new Color(BM_Ground.color.r,BM_Ground.color.g,BM_Ground.color.b, 0f);
        
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
        paused = true;
    }

    public void Resume()
    {
      
        if (paused == true)
        {
            Time.timeScale = _PrirorTimeScale;
            paused = false;
        }
        
    }
}
