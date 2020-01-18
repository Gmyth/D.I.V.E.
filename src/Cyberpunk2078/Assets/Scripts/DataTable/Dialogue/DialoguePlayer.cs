using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialoguePlayer : MonoBehaviour
{

    public bool isInDialogue = false;

    public TimelineManager CurrentTimelineManager
    {
        set
        {
            if (value != null)
            {
                isInDialogue = true;
                OnDialogueStart();
            }
        }

        get
        {
            return CurrentTimelineManager;
        }
    }

    private Animator playerAnimator;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
//        if (PlayerCharacter.Singleton.State.isGrounded())
//        {
//            
//        }
    }

    public void OnDialogueStart()
    {
        FreezePlayer();
    }

    public void OnDialogueEnd()
    {
        EndTimelineWithPlayerExitState("Idle");
    }

    public void EndTimelineWithPlayerExitState(string name)
    {
        isInDialogue = false;
        CurrentTimelineManager = null;

        PlayerCharacter.Singleton.GetFSM().CurrentStateName = name;
    }

    public void FreezePlayer()
    {
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "NoInput";
    }

    public void PlayAnimation(string name)
    {
        playerAnimator.Play("MainCharacter_" + name);
    }

}
