using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocker : MonoBehaviour
{

    public SimpleLockedDoor ConnectedDoor;

    [SerializeField] private GameObject condition;
    [SerializeField] private GameObject notification;
    [SerializeField] private Sprite unlocked;
    private bool triggered = true;

    private bool okForTrigger = false;
    // Start is called before the first frame update
    void Awake()
    {
        ConnectedDoor.AddLocker(this);
        notification.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (condition && !condition.activeInHierarchy)
        {
            okForTrigger = true;
            notification.SetActive(true);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (okForTrigger && triggered && (other.gameObject.tag == "PlayerHitBox" || other.gameObject.tag == "Player"))
        {
            ConnectedDoor.DeleteLocker();
            gameObject.SetActive(false);
            notification.SetActive(false);
            triggered = false;
            GetComponent<SpriteRenderer>().sprite = unlocked;
        }
    }

}
