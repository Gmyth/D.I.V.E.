﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    //Dummy reference
    private GameObject[] dummy;

    //Objects needed to be restored
    public List<GameObject> objects;

    public GameObject blackScreen;  
   
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
        //triggers = GameObject.FindGameObjectsWithTag("Triggers");
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

        objects.Clear();

        for(int i = 0; i < dummy.Length; i ++)
        {
            //put dummy into enemy
            if (!enemy.Contains(dummy[i]))
            {
                enemy.Add(dummy[i]);
            }

            //record position
            dummy[i].GetComponent<Enemy>().lastCheckPointTransform = dummy[i].gameObject.transform.position;

        }
    }

    public void RestoreCheckPoint()
    {
        StartCoroutine("BlackScreen");

        for(int i = 0; i < objects.Count; ++i)
        {
            if (objects[i].GetComponent<SimpleEventTrigger>() != null)
            {
                objects[i].GetComponent<SimpleEventTrigger>().gameObject.SetActive(true);
            }
        }
    }

    IEnumerator BlackScreen()
    {
        Image image = blackScreen.GetComponent<Image>();


        float a = 0;

        while (a < 1)
        {
            a += Time.deltaTime;
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }

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
            enemy[i].GetComponent<Enemy>().Reset();
        }

        while (a > 0)
        {
            a -= Time.deltaTime;
            image.color = new Color(0, 0, 0, a);
            yield return null;
        }
        PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 0;
        yield return null;
    }
    public void EnterResetPool(GameObject obj)
    {
        enemyPool.Add(obj);
    }

    public void RestoreObject(GameObject obj)
    {
        objects.Add(obj);
    }
}
