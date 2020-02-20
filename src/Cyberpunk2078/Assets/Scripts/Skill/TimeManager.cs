using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; } = null;


    [SerializeField]private float defaultSmoothTime;
    [SerializeField]private float slowMotionFactor;
    [SerializeField]private float targetFXAlpha;

    private float velocity;
    private SpriteRenderer blackScreen;
    private float startTime;
    private float endTime;
    private float targetScale;
    private float smoothTime;
    private bool triggered;
    public float GlobalGravity = 6;
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
        Instance = this;
        blackScreen = Camera.main.GetComponentInChildren<SpriteRenderer>();
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale / 60f;
        //Debug.LogWarning(Time.fixedDeltaTime);
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
//        if (triggered)
//        {
//            smoothTime += 0.085f;
//
//            if (startTime + 0.15f < Time.unscaledTime)
//            {
//                CameraManager.Instance.FocusTo(FindObjectOfType<PlayerCharacter>().transform.position,0.2f);
//                //CameraManager.Instance.FlashIn(7.5f,0.05f,0.1f,0.01f);
//            }
//
//            if (Time.unscaledTime < endTime)
//            {
//                var newScale = Time.timeScale - (1f / smoothTime) * Time.unscaledDeltaTime;
//                Time.timeScale = Mathf.Clamp(newScale, targetScale, 1f);
//                //Time.fixedDeltaTime = Time.timeScale * 0.02f;
//                float current = blackScreen.color.a;
//                blackScreen.color =new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, Mathf.SmoothDamp(current, targetFXAlpha, ref velocity, 0.1f));
//            }
//            else if(Time.unscaledTime >= endTime)
//            {
//                triggered = false;
//                Time.timeScale = 1;
//                //Time.fixedDeltaTime = Time.timeScale * 0.02f;
//                blackScreen.color = new Color(blackScreen.color.r,blackScreen.color.g,blackScreen.color.b, 0f);
//            }
//        }

        
    }
    public void startSlowMotionBlink(float duration, float scale){
        Time.timeScale = scale >= 0 ? scale: slowMotionFactor;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StartCoroutine(endSlowMotionDelay(duration));
    }

    public void startSlowMotion(float duration, float scale = -1f, float finishTime = 0f,float delta = 0.1f)
    {
//        startTime = Time.unscaledTime;
        targetScale = scale >= 0 ? scale: slowMotionFactor;
//        smoothTime = _smoothTime >= 0 ? _smoothTime : defaultSmoothTime;
//        endTime = Time.unscaledTime + duration;
        //triggered = true;
        StartCoroutine(slowMotion(delta, targetScale, finishTime,duration));
    }

    private IEnumerator slowMotion(float delta, float targetScale,float duration,float endDuration)
    {
        var times = (int)(duration / delta);
        var deltaScale = Time.timeScale - targetScale;
        if (duration == 0) Time.timeScale = targetScale;
        for (int i = 0; i < times; i++)
        {
            Time.timeScale -= deltaScale;
            yield return new WaitForSecondsRealtime(delta);
        }

        if (endDuration!= 0)
        {
            endSlowMotion(endDuration);
        }
    }    

    public void endSlowMotion(float delay = 0f)
    {
        if (delay == 0)
            EndSlowMotionImmediately();
        else
            StartCoroutine(endSlowMotionDelay(delay));
    }

    public void EndSlowMotionImmediately()
    {
        Time.timeScale = 1;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0f);
        triggered = false;
    }

    public void ApplyBlackScreen()
    {

        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, 0.7f);

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
