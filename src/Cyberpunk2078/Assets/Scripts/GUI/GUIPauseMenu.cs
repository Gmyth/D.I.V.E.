using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private void SetTMP(string label, string content)
    {
        var tmps = Panel.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI tmp in tmps)
        {
            if(tmp.gameObject.name == "TMP_label")
            {
                tmp.text = label;
            }
            else if(tmp.gameObject.name == "TMP_content")
            {
                tmp.text = content;
            }
        }
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
        SetTMP("Back to level selection", "Are you sure you want to quit the current game and return to the level selection?");
        currentEvent = "LevelSelection";
    }

    private void BK_MainMenuClicked()
    {
        ActivePanel();
        SetTMP("Back to MainMenu", "Are you sure back to menu?");
        currentEvent = "MainMenu";

    }

    private void QuitClicked()
    {
        ActivePanel();
        SetTMP("Quit", "Do you want to quit the game?");
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
