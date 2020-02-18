using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessManager : MonoBehaviour
{  
    public static GameProcessManager Singleton { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject PlayerHolder;

    private int currentLevelIndex = -1;
    private GameObject currentLevel = null;
    private List<GameObject> LoadedLevel = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GUIManager.Singleton.Open("MainMenu", this);
        
    }
    
    private void Awake()
    {
        if (!Singleton)
        {
            Singleton = this;
            PlayerHolder.SetActive(false);
            // TEMP: Reload the scene will have issue
            DontDestroyOnLoad(gameObject);
        }
        else if (Singleton != this)
            Destroy(gameObject);
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
        GUIManager.Singleton.Close("MainMenu");
        StartCoroutine(LevelStart(index));
        Camera.main.GetComponent<FadeCamera>().RedoFade();
    }

    public IEnumerator LevelStart(int levelIndex)
    {
        //generate level
        LoadLevel(levelIndex);

        //Configure player, camera, ui
        InitPlayer(currentLevel, true);

        InitCamera();

        InitHUD();

        yield return null;
    }

    public void InitPlayer(GameObject currentLevel, bool ResetingStats = false)
    {
        if(currentLevel != null)
        {
            var start_pos = currentLevel.GetComponent<LevelInfo>().StartPoint;
            
            PlayerHolder.transform.position = start_pos.transform.position;
            PlayerHolder.SetActive(true);
            
            if(ResetingStats == true)
            {
                PlayerCharacter playerCharacter = PlayerHolder.GetComponentInChildren<PlayerCharacter>();
                playerCharacter.init();
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
        currentLevel = Instantiate(levels[index]);
        LoadedLevel.Add(currentLevel);

        return currentLevel;
    }

    public void InitCamera()
    {
        CameraManager.Instance.ResetTarget();
        CameraManager.Instance.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

        for (int i = 0; i < LoadedLevel.Count; i++)
        {
            Destroy(LoadedLevel[i]);
        }

        LoadedLevel.Clear();
        
        currentLevelIndex = - 1;

        PlayerHolder.SetActive(false);

        //CameraManager.Instance.Release();
        TimeManager.Instance.Resume();

        GUIManager.Singleton.Open("MainMenu");
    }
}
