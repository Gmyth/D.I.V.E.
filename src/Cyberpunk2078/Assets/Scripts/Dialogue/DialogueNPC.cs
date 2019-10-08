using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour
{
    private Animator npcAnimator;

    void Awake()
    {
        npcAnimator = GetComponent<Animator>();
    }

    public void PlayAnimation(string name)
    {
        npcAnimator.Play("NPC_" + name);
    }

}
