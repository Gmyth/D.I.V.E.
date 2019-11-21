using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour
{

    public GameObject startPoint;
    public GameObject endPoint;

    public Text timerText;

    public float totalTime;
    private int minute;
    private int second;
    private int millisecond;
    private GameObject restartInfo;

    [SerializeField] private bool isCounting;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartTimer());
        restartInfo = GameObject.Find("RestartInfo");
        restartInfo.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.Equals(startPoint))
        {
            other.gameObject.SetActive(false);
            totalTime = 0;
            isCounting = true;
        }

        if (other.gameObject.Equals(endPoint))
        {
            // ended;
            timerText.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
            timerText.GetComponent<RectTransform>().localScale = new Vector2(3f, 3f);
            restartInfo.SetActive(true);
            other.gameObject.SetActive(false);
            isCounting = false;
        }
    }

}
