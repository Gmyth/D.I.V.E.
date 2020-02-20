using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    [SerializeField] private GameObject checkPointPrefab;

    /// <summary>
    /// Enemies needed to be reset and restore position
    /// </summary>
    [SerializeField] private List<GameObject> savedEnemies = new List<GameObject>();

    /// <summary>
    /// Dummy reference
    /// </summary>
    [SerializeField] private GameObject[] dummies;

    /// <summary>
    /// //Objects needed to be restored
    /// </summary>
    [SerializeField] private List<GameObject> objects;

    /// <summary>
    /// Blackscreen Reference
    /// </summary>
    [SerializeField] private GameObject blackScreen;

    private Transform playerLastCheckPoint;

    void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        dummies = GameObject.FindGameObjectsWithTag("Dummy");
    }

    public void Save(Transform _playertransform)
    {
        playerLastCheckPoint = _playertransform;

        //Clear when reach new checkpoint
        savedEnemies.Clear();
        objects.Clear();
        
        //save enemies position
        for(int i = 0; i < dummies.Length; i++)
        {
            //record position
            dummies[i].GetComponent<Enemy>().lastCheckPointTransform = dummies[i].gameObject.transform.position;
        }
    }

    public void Restore()
    {
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "NoInput";

        StartCoroutine(Restoring());
    
        //Restore Camera State
        CameraManager.Instance.Idle();
        //TimeManager.Instance.endSlowMotion();

        //Restore objects
        for(int i = 0; i < objects.Count; ++i)
        {
            if (objects[i].GetComponent<SimpleEventTrigger>() != null)
            {
                objects[i].GetComponent<SimpleEventTrigger>().gameObject.SetActive(true);
            }
        }
        objects.Clear();
    }

    private void RestoreEnemy()
    {       
        //Enable kill enemies
        for (int i = 0; i < savedEnemies.Count; i++)
        {
            savedEnemies[i].SetActive(true);
        }

        //Restore all enemies position
        for (int i = 0; i < dummies.Length; i++)
        {
            var lastPos = dummies[i].GetComponent<Enemy>().lastCheckPointTransform;
            dummies[i].transform.position = lastPos;
            dummies[i].GetComponent<Enemy>().Reset();
        }
    }

    IEnumerator Restoring()
    {
        Image image = blackScreen.GetComponent<Image>();
        //Black Screen Fade in
        float a = 0;
        while (a < 1)
        {
            a += (Time.deltaTime/2);
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

        //Restore player position
        var player = PlayerCharacter.Singleton.gameObject;
        player.transform.position = playerLastCheckPoint.position;

        RestoreEnemy();

        

        //Black Screen Fade out
        while (a > 0)
        {
            a -= (Time.deltaTime/2);
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

       
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "Idle";


        yield return null;
    }

    public void Dead(GameObject obj)
    {
        if(!savedEnemies.Contains(obj))
            savedEnemies.Add(obj);
    }

    public void RestoreObject(GameObject obj)
    {
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }           
    }
}
