using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEventTrigger : MonoBehaviour
{

    public UnityEvent[] triggeredEvents;
    public Color GizmoColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InvokeEvents()
    {
        CheckPointManager.Instance.RestoreObject(gameObject);
        for (int i = 0; i < triggeredEvents.Length; i++)
        {
            triggeredEvents[i].Invoke();
        }
    }

    public void PrintHUDText(string str)
    {
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText(str);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            InvokeEvents();
            gameObject.SetActive(false);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

}
