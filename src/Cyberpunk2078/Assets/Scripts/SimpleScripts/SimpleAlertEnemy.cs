using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAlertEnemy : MonoBehaviour
{

    private SimpleAlertDoor alertDoor;

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
        alertDoor = door;
    }

    private void OnEnable()
    {
        alertDoor.ApplyEnemyCountChange(1);
    }

    private void OnDisable() 
    {
        alertDoor.ApplyEnemyCountChange(-1);
    }

}
