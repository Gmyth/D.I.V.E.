using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueDetection : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Press A to talk");
        if(other.gameObject.name == "Player")
        {
            //other.gameObject.GetComponent<DialoguePlayer>().inDialogueZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit");
        switch (other.gameObject.name)
        {
            case "Player":
                //other.gameObject.GetComponent<DialoguePlayer>().inDialogueZone = false;
                //other.gameObject.GetComponent<DialoguePlayer>().isDialogueOngoing = false;
                GUIManager.Singleton.Close("DialogueUI");
                break;
            default:
                break;
        }
    }
}
