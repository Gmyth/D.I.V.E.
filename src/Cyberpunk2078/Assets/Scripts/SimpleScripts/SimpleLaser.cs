using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLaser : MonoBehaviour
{

    public Transform RespawnPoint;

    public Color GizmoColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (PlayerCharacter.Singleton.GetFSM().CurrentStateName != "Dashing")
            {

                if (PlayerCharacter.Singleton[StatisticType.Hp] > 0)
                {
                    //Take damage
                    PlayerCharacter.Singleton.ApplyDamage(1);

                    //Respawn at last checkpoint
                    other.transform.position = RespawnPoint.position;
                }
                else
                {
                    CheckPointManager.Instance.Restore();
                }

            }

        }
    }

}
