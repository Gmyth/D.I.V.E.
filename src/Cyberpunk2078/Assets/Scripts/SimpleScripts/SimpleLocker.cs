using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocker : Restorable
{

    public SimpleLockedDoor ConnectedDoor;

    [SerializeField] private GameObject condition;
    [SerializeField] private GameObject notification;
    [SerializeField] private Sprite unlocked;
    private bool triggered = false;

    private bool okForTrigger = false;


    /// ////////////////////////////////
    private Sprite s_sprite;
    private bool s_notification;
    private bool s_triggered;
    private bool s_okForTrigger;


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

    public override void Save()
    {
        s_notification = notification.activeInHierarchy;
        s_sprite = GetComponent<SpriteRenderer>().sprite;
        s_triggered = triggered;
        s_okForTrigger = okForTrigger;
    }

    public override void Restore()
    {
        notification.SetActive(s_notification);
        GetComponent<SpriteRenderer>().sprite = s_sprite;
        triggered = s_triggered;
        okForTrigger = s_okForTrigger;
    }

}
