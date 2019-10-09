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
        EndTimelineWithPlayerExitState(0);
    }

    public void EndTimelineWithPlayerExitState(int playerExitState)
    {
        isInDialogue = false;
        CurrentTimelineManager = null;
        PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = playerExitState;
    }

    public void FreezePlayer()
    {
        PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 9;
    }

    public void PlayAnimation(string name) {
        playerAnimator.Play("MainCharacter_" + name);
    }

}
