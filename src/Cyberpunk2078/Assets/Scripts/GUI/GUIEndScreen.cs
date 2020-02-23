using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIEndScreen : GUIWindow
{
    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);


        GUIManager.Singleton.Close("PauseMenu");


        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "NoInput";
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<Animator>().Play("MainCharacter_Idle", 0, 0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GUIManager.Singleton.Open("MainMenu");
            GUIManager.Singleton.Close("EndScreen");
        }
    }
}
