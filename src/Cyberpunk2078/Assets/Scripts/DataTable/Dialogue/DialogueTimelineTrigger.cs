using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueTimelineTrigger : MonoBehaviour
{
    [SerializeField] private int dialogueID;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();

            playerCharacter.StartDialogue(dialogueID);
        }
    }
}
