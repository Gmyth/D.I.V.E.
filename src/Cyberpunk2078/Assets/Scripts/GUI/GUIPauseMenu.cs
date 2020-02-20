using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIPauseMenu : GUIWindow
{
    public GameObject resume;
    public GameObject levelSelection;
    public GameObject bk_mainmenu;
    public GameObject quit;

    public GameObject Panel;
    public GameObject Yes;
    public GameObject No;


    private string currentEvent = null;
    // Start is called before the first frame update
    void Start()
    {
        resume.GetComponent<Button>().onClick.AddListener(ResumeClicked);
        levelSelection.GetComponent<Button>().onClick.AddListener(LevelSelectionClicked);
        bk_mainmenu.GetComponent<Button>().onClick.AddListener(BK_MainMenuClicked);
        quit.GetComponent<Button>().onClick.AddListener(QuitClicked);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ResumeClicked()
    {
        GameProcessManager.Singleton.ResumeGame();
    }

    private void ActivePanel( )
    { 
        Panel.SetActive(true);
        Yes.GetComponent<Button>().onClick.AddListener(YesClicked);
        No.GetComponent<Button>().onClick.AddListener(NoClicked);
    }

    private void DeactivePanel()
    {
        Yes.GetComponent<Button>().onClick.RemoveAllListeners();
        No.GetComponent<Button>().onClick.RemoveAllListeners();
        Panel.SetActive(false);
    }

    private void LevelSelectionClicked()
    {
        ActivePanel();

        currentEvent = "LevelSelection";
    }

    private void BK_MainMenuClicked()
    {
        ActivePanel();

        currentEvent = "MainMenu";

    }

    private void QuitClicked()
    {
        ActivePanel();

        currentEvent = "Quit";
    }

    private void YesClicked()
    {
        if(currentEvent != null)
        {
            if(currentEvent == "LevelSelection")
            {
                DeactivePanel();

                //TODO
                GameProcessManager.Singleton.OpenLevelSelection();
            }
            else if(currentEvent == "MainMenu")
            {
                DeactivePanel();
                GameProcessManager.Singleton.BK_MainMenu();
            }
            else if(currentEvent == "Quit")
            {
                DeactivePanel();
                GameProcessManager.Singleton.Quit();
            }
        }
    }

    private void NoClicked()
    {
        DeactivePanel();
    }
}
