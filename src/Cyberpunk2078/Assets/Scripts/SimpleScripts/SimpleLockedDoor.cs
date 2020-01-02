using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLockedDoor : MonoBehaviour
{

    public List<SimpleLocker> lockers;
    public int lockerCount = 0;

    [SerializeField] private List<GameObject> dots;

    public GameObject OpenedDoor;


    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            dots.Add(transform.GetChild(i).gameObject);
        }

        OpenedDoor.SetActive(false);
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
        CameraManager.Instance.Shaking(0.05f,0.1f);
        dots[--lockerCount].SetActive(false);

        if (lockerCount <= 0)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        CameraManager.Instance.Shaking(0.05f,0.7f, true);
        gameObject.SetActive(false);
        OpenedDoor.SetActive(true);

        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText("A door opened somewhere...");
        //CameraManager.Instance.Shaking(0.6f, 1000000f);
    }

}
