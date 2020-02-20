using System;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Dialogue", menuName = "Player State/Dialogue")]
public class PSDialogue : PlayerState
{
    private enum Status
    {
        None = 0,
        Waiting,
        InDialogue,
        Done,
    }
    
    private Rigidbody2D rb2d;

    private Status currentStatus = Status.None;
    
    
    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
        
        
        rb2d = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override string Update()
    {
        if (currentStatus == Status.Waiting)
        {
            switch (GetGroundType())
            {
                case 1:
                    playerCharacter.AddNormalEnergy(1);
                    if (PlayerCharacter.Singleton.InFever)
                        PlayerCharacter.Singleton.AddOverLoadEnergy(1);
                
                    rb2d.velocity = Vector2.zero;
                    rb2d.gravityScale = 0;

                    anim.Play("MainCharacter_Idle", -1, 0f);

                    GUIHUD hud = GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD");
                    if (!hud)
                        return "Idle";

                    Action[] newCallbacks = new Action[playerCharacter.currentDialogueCallbacks.Length + 1];
                    playerCharacter.currentDialogueCallbacks.CopyTo(newCallbacks, 0);
                    newCallbacks[playerCharacter.currentDialogueCallbacks.Length] = EndDialogue;

                    hud.ShowDialogue(DataTableManager.singleton.GetDialogueData(playerCharacter.currentDialogueID), newCallbacks);
                    
                    currentStatus = Status.InDialogue;
                    
                    break;
            }
        }


        if (currentStatus == Status.Done)
            return "Idle";
        
        
        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);

        

        currentStatus = Status.Waiting;


        Vector2 v = rb2d.velocity;
        v.x = 0;
        v.y = Mathf.Min(0, v.y);
        rb2d.velocity = v;
        
        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        mouse.Lock();

        switch (GetGroundType())
        {
            case 0:
                rb2d.gravityScale = playerCharacter.DefaultGravity;
                anim.Play("MainCharacter_Airborne", -1, 0f);
                break;
            
            
            case 1:
                playerCharacter.AddNormalEnergy(1);
                if (PlayerCharacter.Singleton.InFever)
                    PlayerCharacter.Singleton.AddOverLoadEnergy(1);
                
                rb2d.velocity = Vector2.zero;
                rb2d.gravityScale = 0;

                anim.Play("MainCharacter_Idle", -1, 0f);

                GUIHUD hud = GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD");
                if (hud)
                {
                    Action[] newCallbacks = new Action[playerCharacter.currentDialogueCallbacks.Length + 1];
                    playerCharacter.currentDialogueCallbacks.CopyTo(newCallbacks, 0);
                    newCallbacks[playerCharacter.currentDialogueCallbacks.Length] = EndDialogue;

                    hud.ShowDialogue(DataTableManager.singleton.GetDialogueData(playerCharacter.currentDialogueID), newCallbacks);

                    currentStatus = Status.InDialogue;
                }
                else
                    currentStatus = Status.Done;

                break;
        }
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);

        var mouse = GameObject.FindObjectOfType<MouseIndicator>();
        mouse.Unlock();
        
        currentStatus = Status.Done;

        
        playerCharacter.currentDialogueID = -1;
        playerCharacter.currentDialogueCallbacks = null;
    }


    private void EndDialogue()
    {
        currentStatus = Status.Done;
    }
}
