﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessManager : MonoBehaviour
{
    public static GameProcessManager Singleton { get; private set; }


    [Header("References")]
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject PlayerHolder;

    private Dictionary<int, GameObject> LevelDictionary = new Dictionary<int, GameObject>();
    private int currentLevelIndex = -1;
    private GameObject currentLevel = null;
    private List<GameObject> LoadedLevel = new List<GameObject>();


    public Event<int> OnStartLevel { get; private set; } = new Event<int>();
    public Event<int> OnQuitLevel { get; private set; } = new Event<int>();


    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;

            for(int i = 0; i < levels.Length; i++)
            {
                int index = levels[i].GetComponent<LevelInfo>().LevelIndex;
                LevelDictionary.Add(index, levels[i]);
            }

            // TEMP: Reload the scene will have issue
            DontDestroyOnLoad(gameObject);
        }
        else if (Singleton != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        PlayerCharacter.Singleton.init();
        PlayerHolder.SetActive(false);

        
        GUIManager.Singleton.Open("MainMenu", this);
        //CameraManager.Instance.Release();
    }


    public void Quit()
    {
#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }

    public void StartGame(int index)
    {
        AudioManager.Singleton.StopEvent("Title");

        if (GUIManager.Singleton.IsInViewport("MainMenu"))
            GUIManager.Singleton.Close("MainMenu");
        if (GUIManager.Singleton.IsInViewport("LevelSelection"))
            GUIManager.Singleton.Close("LevelSelection");

        StartCoroutine(LevelStart(index));
        Camera.main.GetComponent<FadeCamera>().RedoFade();
    }

    public IEnumerator LevelStart(int levelIndex)
    {
        if (Player.CurrentPlayer == null)
            Player.CreatePlayer();
        
        InitHUD();
        
        //generate level
        LoadLevel(levelIndex);

        //Configure player, camera, ui
        InitPlayer(currentLevel, true);

        InitCamera();
        
        CheckPointManager.Instance.Initialize();


        yield return null;
    }

    public void InitPlayer(GameObject currentLevel, bool ResetingStats = false)
    {
        if(currentLevel != null)
        {
            var start_pos = currentLevel.GetComponent<LevelInfo>().StartPoint;

            PlayerHolder.transform.position = start_pos.transform.position;
            PlayerHolder.SetActive(true);
            PlayerCharacter.Singleton.GetComponent<SimpleTimer>().GetTimer();

            if(ResetingStats == true)
            {
                PlayerCharacter playerCharacter = PlayerHolder.GetComponentInChildren<PlayerCharacter>();
                playerCharacter.ResetStatistics();
            }
        }
    }

    public void InitHUD()
    {
        GUIManager.Singleton.Open("HUD", PlayerHolder.GetComponentInChildren<PlayerCharacter>());
    }

    public GameObject LoadLevel(int index)
    {
        if (currentLevelIndex == index)
            return null;

        currentLevelIndex = index;
        currentLevel = Instantiate(LevelDictionary[index]);
        LoadedLevel.Add(currentLevel);

        foreach(GameObject obj in currentLevel.GetComponent<LevelInfo>().breakable)
        {
            obj.GetComponent<Explodable>().fragmentInEditor();
        }


        OnStartLevel.Invoke(index);


        return currentLevel;
    }

    public void DestroyLevel(GameObject level)
    {
        LoadedLevel.Remove(level);
        Destroy(level);
    }

    public void InitCamera()
    {
        CameraManager.Instance.Initialize();
        CameraManager.Instance.Release();
        var pos = PlayerHolder.transform.position;
        Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
        CameraManager.Instance.ResetTarget();
        CameraManager.Instance.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GUIManager.Singleton.IsInViewport("PauseMenu") && !GUIManager.Singleton.IsInViewport("MainMenu") && !GUIManager.Singleton.IsInViewport("EndScreen") && currentLevelIndex != -1)
        {
            //Camera.main.GetComponent<FadeCamera>().RedoFade();
            GUIManager.Singleton.Open("PauseMenu");
            GUIManager.Singleton.Close("HUD");
            PlayerHolder.GetComponentInChildren<MouseIndicator>().Hide();
            TimeManager.Instance.Pause();
        }
    }

    public void ResumeGame()
    {

        GUIManager.Singleton.Close("PauseMenu");
        GUIManager.Singleton.Open("HUD", PlayerHolder.GetComponentInChildren<PlayerCharacter>());
        PlayerHolder.GetComponentInChildren<MouseIndicator>().Show();
        TimeManager.Instance.Resume();
    }

    public void BK_MainMenu()
    {
        GUIManager.Singleton.Close("HUD");
        GUIManager.Singleton.Close("PauseMenu");

        ResetGame();

        GUIManager.Singleton.Open("MainMenu");
        Camera.main.GetComponent<FadeCamera>().RedoFade();
    }

    public void MainMenu()
    {
        GUIManager.Singleton.Close("LevelSelection");
        GUIManager.Singleton.Open("MainMenu");
        Camera.main.GetComponent<FadeCamera>().RedoFade();
    }

    private void ResetGame()
    {
        for (int i = 0; i < LoadedLevel.Count; i++)
        {
            Destroy(LoadedLevel[i]);
        }

        LoadedLevel.Clear();


        //Can do other stuff here
        PlayerHolder.GetComponentInChildren<GhostSprites>().KillSwitchEngage();
        ObjectRecycler.Singleton.RecycleAll();

        
        
        PlayerHolder.SetActive(false);


        OnQuitLevel.Invoke(currentLevelIndex);

        currentLevelIndex = -1;


        TimeManager.Instance.Resume();
    }


    public void OpenLevelSelection()
    {
        Camera.main.GetComponent<FadeCamera>().RedoFade();
        if (GUIManager.Singleton.IsInViewport("PauseMenu"))
            GUIManager.Singleton.Close("PauseMenu");

        if(GUIManager.Singleton.IsInViewport("MainMenu"))
            GUIManager.Singleton.Close("MainMenu");

        if (LoadedLevel.Count != 0)
        {
            ResetGame();
        }

        GUIManager.Singleton.Open("LevelSelection");
    }

    public GameObject GetCurrentDummies()
    {
        if(currentLevelIndex != -1)
        {
            return currentLevel.GetComponent<LevelInfo>().DummyHolder;
        }

        return null;
    }
}
