using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour
{

    public Text timerText;

    public float totalTime;
    private int minute;
    private int second;
    private int millisecond;
    private GameObject restartInfo;
    private Vector3 defaultPos;
    private Vector3 defaultScale;
    [SerializeField] private bool isCounting;
    private bool triggered = false;

    // Start is called before the first frame update
    void Awake()
    {
        defaultPos = timerText.GetComponent<RectTransform>().localPosition;
        defaultScale = timerText.GetComponent<RectTransform>().localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCounting){
            totalTime += Time.deltaTime;
            minute = (int)totalTime / 60;
            second = second = (int)totalTime - minute * 60;
            millisecond = (int)(Math.Round((totalTime - (int)totalTime), 2) * 100);
            timerText.text = string.Format("{0:d2}:{1:d2}.{2:d2}", minute, second, millisecond);
        }
        
    }

    private IEnumerator StartTimer()
    {
        totalTime = 0;
        while (isCounting) {
            yield return new WaitForSeconds(0.001f);
            totalTime+= 0.01f;
            minute = (int)(totalTime / 60);
            second = (int)(totalTime);
            millisecond = (int)(totalTime % 1.0f * 1000);
            timerText.text = string.Format("{0:d2}:{1:d2}.{2:d3}", minute, second, millisecond);
        }
    }

    public void GetTimer()
    {
        timerText.GetComponent<RectTransform>().localPosition = defaultPos;
        timerText.GetComponent<RectTransform>().localScale = defaultScale;
        totalTime = 0;
        minute = (int)(totalTime / 60);
        second = (int)(totalTime);
        millisecond = (int)(totalTime % 1.0f * 1000);
        isCounting = false;
        timerText.text = string.Format("{0:d2}:{1:d2}.{2:d3}", minute, second, millisecond);
        triggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StartTimer"))
        {
            other.gameObject.SetActive(false);
            totalTime = 0;
            isCounting = true;
        }
    
        if (other.CompareTag("EndTimer"))
        {
            // ended;
            timerText.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0); 
            timerText.GetComponent<RectTransform>().localScale = new Vector2(3f, 3f);
            other.gameObject.SetActive(false);
            isCounting = false;
        }
        
        if (other.CompareTag("Quit") && !triggered)
        {
            // ended;
            triggered = true;
            StartCoroutine(endScreen());
        }
        
    }
    
    
    private IEnumerator endScreen()
    {
        TimeManager.Instance.ApplyBlackScreenFadeIn(1,true);
        MouseIndicator.Singleton.Hide();
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").gameObject.SetActive(false);
        yield return  new WaitForSeconds(3f);
        TimeManager.Instance.EndBlackScreen(true);
        StartCoroutine(GameProcessManager.Singleton.LoadingScreen(5, transform.parent.gameObject));
    }

}
