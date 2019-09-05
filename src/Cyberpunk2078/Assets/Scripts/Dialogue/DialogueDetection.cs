using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Press A to talk");
        if(other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<DialoguePlayer>().inDialogueZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exit");
        if (other.gameObject.name == "Player")
        {
            other.gameObject.GetComponent<DialoguePlayer>().inDialogueZone = false;
            GUIManager.Singleton.Close("DialogueUI");
        }
    }

}
