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

    private void LevelSelectionClicked()
    {

    }

    private void BK_MainMenuClicked()
    {

    }

    private void QuitClicked()
    {
        GameProcessManager.Singleton.Quit();
    }
}
