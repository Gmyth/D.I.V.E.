using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SimpleAreaTrigger : MonoBehaviour
{
    public UnityEvent onEnterEvent;
    public UnityEvent onExitEvent;

    public Color GizmoColor = Color.white;
    public void ShowButtonHint(string s)
    {
        GUITutorial.Singleton.Show(s);
    }

    public void HideButtonHint()
    {
        GUITutorial.Singleton.Hide();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            onEnterEvent.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            onExitEvent.Invoke();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

}
