using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIMenu : GUIWindow
{
    public GameObject NewGame;
    public GameObject LoadGame;
    public GameObject Options;
    public GameObject quit;
    // Start is called before the first frame update
    void Start()
    {
        quit.GetComponent<Button>().onClick.AddListener(QuitClicked);
        LoadGame.GetComponent<Button>().onClick.AddListener(LoadGameClicked);
        Options.GetComponent<Button>().onClick.AddListener(OptionsClicked);
        NewGame.GetComponent<Button>().onClick.AddListener(NewGameClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void QuitClicked()
    {
        GameProcessManager.Singleton.Quit();
    }

    private void NewGameClicked()
    {
       // Camera.main.GetComponent<FadeCamera>().RedoFade();

        GameProcessManager.Singleton.StartGame(0);
    }

    private void LoadGameClicked()
    {

        Camera.main.GetComponent<FadeCamera>().RedoFade();
    }

    private void OptionsClicked()
    {

      
    }
}
