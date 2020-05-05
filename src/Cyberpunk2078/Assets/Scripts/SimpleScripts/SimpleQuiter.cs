using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleQuiter : MonoBehaviour
{
    private bool triggered;
    // Start is called before the first frame update
    void OnEnable()
    {
        triggered = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(endScreen());
        }
    }

    private IEnumerator endScreen()
    {
        TimeManager.Instance.ApplyBlackScreenFadeIn(1);
        yield return  new WaitForSeconds(3f);
        GameProcessManager.Singleton.BK_MainMenu();
    }
}
