using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    public List<GameObject> checkPoints;
    public GameObject checkPointPrefab;
    public Transform checkPointFolder;

    private Transform playerLastCheckPoint;
    private GameObject player;
    public Dictionary<GameObject, Transform> dummyLastCheckPoint;
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        GameObject [] dummy = GameObject.FindGameObjectsWithTag("Dummy");
        Debug.Log("Dummy Length:" + dummy.Length);
        for(int i = 0; i < dummy.Length; i ++)
        {
            dummyLastCheckPoint.Add(dummy[i], dummy[i].transform);
        }
    }

    public void RestoreCheckPoint()
    {
        Debug.Log("WAD");
        var player = PlayerCharacter.Singleton.gameObject;
        player.transform.position = playerLastCheckPoint.position;

        GameObject[] dummy = GameObject.FindGameObjectsWithTag("Dummy");
        for (int i = 0; i < dummy.Length; i++)
        {
            dummy[i].transform.position = dummyLastCheckPoint[dummy[i]].position;
        }
    }
}
