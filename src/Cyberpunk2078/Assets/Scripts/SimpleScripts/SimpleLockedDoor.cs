using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockedDoor : MonoBehaviour
{

    public List<SimpleLocker> lockers;
    public int lockerCount = 0;

    [SerializeField] private List<GameObject> dots;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            dots.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLocker(SimpleLocker targetLocker)
    {
        lockers.Add(targetLocker);
        lockerCount++;
    }

    public void DeleteLocker()
    {
        lockerCount--;
        dots[lockerCount].SetActive(false);
        if (lockerCount <= 0)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        gameObject.SetActive(false);
    }

}
