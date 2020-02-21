using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIMenu : GUIWindow
{
    public GameObject NewGame;
    public GameObject LoadGame;
    public GameObject Options;
    public GameObject quit;

    public float SpeedFactor = 1;

    private UnityAction Action;

    // Start is called before the first frame update
    void Start()
    {
        quit.GetComponent<Button>().onClick.AddListener(()=> ButtonClicked(quit));
        LoadGame.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(LoadGame));
        Options.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(Options));
        NewGame.GetComponent<Button>().onClick.AddListener(() => ButtonClicked(NewGame));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ButtonClicked(GameObject obj)
    {    
        if (obj.name == "New Game")
            Action += UI_NewGame;
        else if (obj.name == "Load Game")
            Action += UI_LoadGame;
        else if (obj.name == "Options")
            Action += UI_Options;
        else if (obj.name == "Quit")
            Action += UI_Quit;

        StartCoroutine(Animation());       
    }

    void UI_NewGame()
    {
        GameProcessManager.Singleton.StartGame(1);
    }

    void UI_LoadGame()
    {
        GameProcessManager.Singleton.OpenLevelSelection();       
    }

    void UI_Options()
    {

    }
    void UI_Quit()
    {
        GameProcessManager.Singleton.Quit();
    }
    private IEnumerator Animation()
    {
        //Black Screen Fade in
        //float a = 0;
        //while (a < 1)
        //{
        //    a += (Time.deltaTime * SpeedFactor);
        //    image.color = new Color(0, 0, 0, a);
        //    yield return null;
        //}

        //while (a > 0)
        //{
        //    a -= (Time.deltaTime * SpeedFactor);
        //    image.color = new Color(0, 0, 0, a);
        //    yield return null;
        //}

        EventSystem.current.SetSelectedGameObject(null);

        yield return new WaitForSeconds(0.05f);

        Action.Invoke();

        yield return null;
    }
}
