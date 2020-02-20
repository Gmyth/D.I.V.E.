using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{

    [SerializeField] private TriggerObject[] triggers;
    [SerializeField] private string tag;

    [SerializeField] private bool oneWay;
    // Start is called before the first frame update
    
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag(tag))
        {
            foreach (var trigger in triggers)
            {
                trigger.Enable();
            }
        }
    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!oneWay && other.transform.CompareTag(tag))
        {
            foreach (var trigger in triggers)
            {
                trigger.Disable();
            }
        }
    }
}
