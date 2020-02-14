using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIMenu : GUIWindow
{
    public GameObject NewGame;
    public GameObject quit;
    // Start is called before the first frame update
    void Start()
    {
        quit.GetComponent<Button>().onClick.AddListener(QuitClicked);
        NewGame.GetComponent<Button>().onClick.AddListener(NewGameClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void test()
    {
        Debug.Log("dawdaw");

    }

    private void QuitClicked()
    {
        GameProcessManager.Singleton.Quit();
    }

    private void NewGameClicked()
    {
        GameProcessManager.Singleton.StartGame();
    }
}
