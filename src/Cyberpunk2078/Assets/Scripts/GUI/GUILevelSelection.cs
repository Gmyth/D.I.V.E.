using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUILevelSelection : GUIWindow
{
    [Header("Reference")]
    public GameObject[] UI_levelReference;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < UI_levelReference.Length; i++)
        {
            if (Int32.TryParse(UI_levelReference[i].name, out int index) == true)
                UI_levelReference[i].GetComponentInChildren<Button>().onClick.AddListener(() => LevelSelectionClicked(index));
            else
                throw new ArgumentNullException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LevelSelectionClicked(int i)
    {       
        GameProcessManager.Singleton.StartGame(i);
    }
}
