using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProcessManager : MonoBehaviour
{
    public static GameProcessManager Singleton { get; private set; }

    [SerializeField] private GameObject levels;
    [SerializeField] private GameObject TimeManager;
    [SerializeField] private GameObject PlayerHolder;

    private PlayerCharacter playerCharacter;
    // Start is called before the first frame update
    void Start()
    {
        GUIManager.Singleton.Open("MainMenu", this);
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void StartGame()
    {
        GUIManager.Singleton.Close("MainMenu");
        StartCoroutine(LevelStart(1));
    }

    public IEnumerator LevelStart(int levelIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Initialize();
    }

    private void Initialize()
    {
        
        //Instantiate(TimeManager);
        ResetPlayerPosition();
        GUIManager.Singleton.Open("HUD", PlayerHolder.GetComponentInChildren<PlayerCharacter>());
        CameraManager.Instance.ResetTarget();
        CameraManager.Instance.Reset();
    }

    private void ResetPlayerPosition()
    {
        GameObject start_pos = GameObject.FindGameObjectWithTag("Respawn");
        PlayerHolder.transform.position = start_pos.transform.position;
        PlayerHolder.SetActive(true);
        playerCharacter = PlayerHolder.GetComponentInChildren<PlayerCharacter>();
        playerCharacter.init();
    }
}
