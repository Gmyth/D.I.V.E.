using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static CheckPointManager Instance { get; private set; } = null;

    public List<GameObject> checkPoints;
    public GameObject checkPointPrefab;
    public Transform checkPointFolder;

    private Transform lastSavePoint;
    private GameObject player;

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
        lastSavePoint = t;
    }

    public void RestoreCheckPoint()
    {
        var player = PlayerCharacter.Singleton.gameObject;
        player.transform.position = lastSavePoint.position;
    }
}
