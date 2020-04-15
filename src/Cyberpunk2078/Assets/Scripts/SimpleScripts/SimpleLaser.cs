using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLaser : MonoBehaviour
{

    //public Transform RespawnPoint;

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
                    Vector3 KnockBackDir = transform.rotation.z != 0 ? new Vector3((other.transform.position - transform.position).normalized.x,0,0):
                            new Vector3(0,(other.transform.position - transform.position).normalized.y,0);

                    PlayerCharacter.Singleton.Knockback(KnockBackDir,20f,0.3f);
                    
                    
                    
                    //Respawn at last checkpoint
                    //other.transform.position = RespawnPoint.position;
                }
                else
                {
                    CheckPointManager.Instance.Restore();
                }

            }

        }
    }

}
