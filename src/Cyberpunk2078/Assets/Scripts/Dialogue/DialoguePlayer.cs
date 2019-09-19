using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialoguePlayer : MonoBehaviour
{
    public bool inDialogueZone = false;
    public bool isDialogueOngoing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.G)) return;

        if (inDialogueZone != true || isDialogueOngoing != false) return;

        StartCoroutine(DialogueManager.Instance.PlayDialogue(0, gameObject.transform));

        isDialogueOngoing = true;
    }

    public void SetDialogueOngoing(bool boolean)
    {
        isDialogueOngoing = boolean;
    }

    public bool isInDialogue;
    public TimelineManager currentTimelineManager;
    public void PrepareForTimeline(TimelineManager tm)
    {
        isInDialogue = true;
        currentTimelineManager = tm;
        PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 9;

    }

    public void EndTimelineWithPlayerExitState(int playerExitState)
    {
        isInDialogue = false;
        currentTimelineManager = null;
        PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = playerExitState;
    }

}
