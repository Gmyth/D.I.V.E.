using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    private List<GameObject> checkPoints;

    private Transform playerLastCheckPoint;

    public GameObject checkPointPrefab;

    public Transform checkPointFolder;

    /// <summary>
    /// Enemies needed to be reset and restore position
    /// </summary>
    public List<GameObject> killedEnemies;

    /// <summary>
    /// Dummy reference
    /// </summary>
    private GameObject[] dummy;

    /// <summary>
    /// //Objects needed to be restored
    /// </summary>
    public List<GameObject> objects;

    /// <summary>
    /// Blackscreen Reference
    /// </summary>
    public GameObject blackScreen;  
   
    void Awake()
    {
        Instance = this;
        killedEnemies = new List<GameObject>();       
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get all dummies
        dummy = GameObject.FindGameObjectsWithTag("Dummy");
    }

    //Editor tool
    public void SetCheckPoint()
    {
        var obj = Instantiate(checkPointPrefab, gameObject.transform.position, Quaternion.identity, checkPointFolder);
        checkPoints.Add(obj);
    }
    //Editor tool
    public void ClearCheckPoint()
    {
        for(int i=0;i<checkPoints.Count;i++)
        {
            DestroyImmediate(checkPoints[i]);
        }
        checkPoints.Clear();
    }

    public void RecordPosition(Transform t)
    {
        playerLastCheckPoint = t;

        //Clear when reach new checkpoint
        killedEnemies.Clear();
        objects.Clear();

        for(int i = 0; i < dummy.Length; i ++)
        {
            //record position
            dummy[i].GetComponent<Enemy>().lastCheckPointTransform = dummy[i].gameObject.transform.position;
        }
    }

    public void RestoreCheckPoint()
    {
        //Restore enemies position
        StartCoroutine("BlackScreen");
    
        //Restore Camera State
        CameraManager.Instance.Idle();
        TimeManager.Instance.endSlowMotion();

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

    IEnumerator BlackScreen()
    {
        Image image = blackScreen.GetComponent<Image>();

        //Black Screen Fade in
        float a = 0;

        while (a < 1)
        {
            a += Time.deltaTime;
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

        //Restore player position
        var player = PlayerCharacter.Singleton.gameObject;
        player.transform.position = playerLastCheckPoint.position;
       
        //Enable kill enemies
        for (int i = 0; i < killedEnemies.Count; i++)
        {
            killedEnemies[i].SetActive(true);
        }

        //Restore all enemies position
        for (int i = 0; i < dummy.Length; i++)
        {
            var lastPos = dummy[i].GetComponent<Enemy>().lastCheckPointTransform;
            dummy[i].transform.position = lastPos;
            dummy[i].GetComponent<Enemy>().Reset();
        }

        //Black Screen Fade out
        while (a > 0)
        {
            a -= Time.deltaTime;
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = 0;
        yield return null;
    }

    public void Dead(GameObject obj)
    {
        if(!killedEnemies.Contains(obj))
            killedEnemies.Add(obj);
    }

    public void RestoreObject(GameObject obj)
    {
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }           
    }
}
