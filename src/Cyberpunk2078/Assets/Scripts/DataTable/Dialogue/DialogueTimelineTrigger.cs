using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTimelineTrigger : MonoBehaviour
{
    [SerializeField] private int dialogueID;

    private bool isTriggered = false;


    private void OnEnable()
    {
        isTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.tag == "Player")
        {
            other.GetComponent<PlayerCharacter>().StartDialogue(dialogueID);

            
            isTriggered = true;
            
            gameObject.SetActive(false);
        }
    }
}
