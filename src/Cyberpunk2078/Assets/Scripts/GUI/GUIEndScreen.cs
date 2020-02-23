using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIEndScreen : GUIWindow
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GUIManager.Singleton.Open("MainMenu");
            Close();
        }
    }
}
