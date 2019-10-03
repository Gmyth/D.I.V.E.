using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    public List<GameObject> checkPoints;
    public GameObject checkPointPrefab;
    public Transform checkPointFolder;

    private Transform playerLastCheckPoint;
    private GameObject player;
    public List<GameObject> enemy;
    public List<GameObject> enemyPool;
    private GameObject[] dummy;
    void Awake()
    {
        Instance = this;
        enemy = new List<GameObject>();
        enemyPool = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dummy = GameObject.FindGameObjectsWithTag("Dummy");
        //Debug.Log("Dummy Length:" + dummy.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCheckPoint()
    {
        var obj = Instantiate(checkPointPrefab, gameObject.transform.position, Quaternion.identity, checkPointFolder);
        checkPoints.Add(obj);
    }

    public void ClearCheckPoint()
    {
        for(int i=0;i<checkPoints.Count;i++)
        {
            DestroyImmediate(checkPoints[i]);
        }
        checkPoints.Clear();
    }

    public void SetLastCheckPointTransform(Transform t)
    {
        playerLastCheckPoint = t;

        enemyPool.Clear();

        for(int i = 0; i < dummy.Length; i ++)
        {
            dummy[i].GetComponent<Enemy>().lastCheckPointTransform = dummy[i].gameObject.transform.position;
            enemy.Add(dummy[i]);
        }
    }

    public void RestoreCheckPoint()
    {
        Debug.Log("WAD");
        var player = PlayerCharacter.Singleton.gameObject;
        player.transform.position = playerLastCheckPoint.position;

        for (int i = 0; i < enemyPool.Count; i++)
        {
            enemyPool[i].SetActive(true);
        }

        for (int i = 0; i < enemy.Count; i++)
        {
            var lastPos = enemy[i].GetComponent<Enemy>().lastCheckPointTransform;
            enemy[i].transform.position = lastPos;
        }
    }

    public void EnterResetPool(GameObject obj)
    {
        Debug.Log("Eneter POol");
        enemyPool.Add(obj);
    }
}
