using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlertEnemy : MonoBehaviour
{

    private List<SimpleAlertDoor> alertDoors = new List<SimpleAlertDoor>();

    private bool reviveFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BindAlertDoor(SimpleAlertDoor door)
    {
        alertDoors.Add(door);
    }

    private void OnEnable()
    {
        if (reviveFlag)
        {
            for (int i = 0; i < alertDoors.Count; i++)
                alertDoors[i].ApplyEnemyCountChange(1);
            reviveFlag = false;
        }
    }

    private void OnDisable() 
    {
        for (int i = 0; i < alertDoors.Count; i++)
            alertDoors[i].ApplyEnemyCountChange(-1);
        reviveFlag = true;
    }

}
