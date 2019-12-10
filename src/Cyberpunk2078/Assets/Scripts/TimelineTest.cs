using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineTest : MonoBehaviour
{

    private void OnEnable()
    {
        //StartCoroutine(SlowDown(5));
    }

    private void OnDisable()
    {
        StopCoroutine("SlowDown");
        Time.timeScale = 1;
    }

    private IEnumerator SlowDown(float duration)
    {
        float offset = 1f / duration * Time.deltaTime;
        float t = 0.0f;
        while (t <= 1)
        {
            Time.timeScale = Mathf.Lerp(t, 1f, 0f); ;
            Debug.Log("Time Scale: " + Time.timeScale);

            t += offset;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

}
