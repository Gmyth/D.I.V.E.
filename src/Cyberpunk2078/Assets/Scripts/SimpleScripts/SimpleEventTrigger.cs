using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class SimpleEventTrigger : Restorable
{
    public UnityEvent[] triggeredEvents;
    public Color GizmoColor = Color.white;

    private bool isPaused = false;

    private bool isInvoked = false;


    private bool s_isInvoked;
    public void PrintHUDText(string str)
    {
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText(str);
    }

    public void ShowDialogue(int dialogueID)
    {
        isPaused = true;

        PlayerCharacter.Singleton.StartDialogue(dialogueID, Unpause);
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Unpause()
    {
        isPaused = false;
    }


    public void ShowButtonHint(string s)
    {
        GUITutorial.Singleton.Show(s);
    }

    public void HideButtonHint()
    {
        GUITutorial.Singleton.Hide();
    }


    private IEnumerator InvokeEvents()
    {
        //CheckPointManager.Instance.RestoreObject(gameObject);

        if(isInvoked == false)
        {
            for (int i = 0; i < triggeredEvents.Length; ++i)
            {
                triggeredEvents[i].Invoke();

                while (isPaused)
                    yield return null;
            }
        }
        

        isInvoked = true;
        CheckPointManager.Instance.RegisterObj(gameObject);
        //gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            StartCoroutine(InvokeEvents());
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    public override void Save()
    {
        s_isInvoked = isInvoked;
    }

    public override void Restore()
    {
        isInvoked = s_isInvoked;
        
    }
}
