using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockedDoor : MonoBehaviour
{

    public List<SimpleLocker> lockers;
    public int lockerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
