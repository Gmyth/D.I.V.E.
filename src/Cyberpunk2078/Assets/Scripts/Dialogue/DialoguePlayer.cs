using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour
{
    public bool inDialogueZone = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            if(inDialogueZone == true)
            {
                StartCoroutine(DialogueManager.Instance.PlayDialogue(0));
                inDialogueZone = false;
            }
        }
    }
}
