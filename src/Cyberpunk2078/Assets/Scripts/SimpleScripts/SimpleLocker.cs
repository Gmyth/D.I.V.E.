﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocker : MonoBehaviour
{

    public SimpleLockedDoor ConnectedDoor;

    [SerializeField] private GameObject condition;
    [SerializeField] private GameObject notification;
    [SerializeField] private Sprite unlocked;
    private bool triggered = false;

    private bool okForTrigger = false;
    // Start is called before the first frame update
    void Awake()
    {
        ConnectedDoor?.AddLocker(this);
        notification?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (condition && !condition.activeInHierarchy && !triggered)
        {
            okForTrigger = true;
            notification.SetActive(true);
            GetComponent<SpriteRenderer>().color = Color.white;
        }else if (!condition)
        {
            okForTrigger = true;
            notification.SetActive(true);
            GetComponent<SpriteRenderer>().color = Color.white;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (okForTrigger && !triggered && other.gameObject.tag == "Player")
        {
            ConnectedDoor.DeleteLocker();
            notification.SetActive(false);
            triggered = true;
            okForTrigger = false;
            GetComponent<SpriteRenderer>().sprite = unlocked;
        }
    }

}
