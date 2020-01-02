using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPointTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log(LogUtility.MakeLogStringFormat("CheckPointTrigger", "Enter check point"));
            CheckPointManager.Instance.RecordPosition(gameObject.transform);
        }
    }
}
