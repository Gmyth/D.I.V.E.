﻿using UnityEngine;


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
    
    
    [SerializeField] private int indexPSIdle = 0;
    
    private Rigidbody2D rb2d;

    private Status currentStatus = Status.None;
    
    
    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
        
        
        rb2d = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override int Update()
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
                        return indexPSIdle;
                    
                    hud.ShowDialogue(DataTableManager.singleton.GetDialogueData(playerCharacter.currentDialogueID),
                        delegate
                        {
                            currentStatus = Status.Done;
                            hud.HideDialogue();
                        });
                
                    currentStatus = Status.InDialogue;
                    
                    break;
            }
        }


        if (currentStatus == Status.Done)
            return indexPSIdle;
        
        
        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        currentStatus = Status.Waiting;


        Vector2 v = rb2d.velocity;
        v.y = 0;
        rb2d.velocity = v;
        
        
        switch (GetGroundType())
        {
            case 0:
                rb2d.gravityScale = 3;
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
                    hud.ShowDialogue(DataTableManager.singleton.GetDialogueData(playerCharacter.currentDialogueID),
                        delegate
                        {
                            currentStatus = Status.Done;
                            hud.HideDialogue();
                        });

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

        
        currentStatus = Status.Done;

        
        playerCharacter.currentDialogueID = -1;
    }
}
