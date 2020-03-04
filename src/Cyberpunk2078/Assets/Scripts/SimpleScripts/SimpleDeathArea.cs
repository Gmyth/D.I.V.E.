using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDeathArea : MonoBehaviour
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
            PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();

            if (playerCharacter[StatisticType.Hp] > 0)
            {
                //Take damage
                PlayerCharacter.Singleton.ApplyDamage(100);

                //Respawn at last checkpoint
                //other.transform.position = RespawnPoint.position;
            }
            else
            {
                CheckPointManager.Instance.Restore();
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
        //Gizmos.DrawCube(RespawnPoint.position, new Vector3(1, 2, 1));
    }

}
