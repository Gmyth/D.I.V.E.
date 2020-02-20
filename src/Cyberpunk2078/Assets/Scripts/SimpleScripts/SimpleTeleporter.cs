using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum MovingDirection
{
    R2L,
    L2R
}

public class SimpleTeleporter : MonoBehaviour
{ 
    //public Color GizmoColor;

    private GameObject mask;

    private CapsuleCollider2D capc;

    private Collider2D c;

    [SerializeField] private MovingDirection direction;

    [SerializeField] private float speed = 5f;

    [SerializeField] private int TargetLevelIndex;

    // Update is called once per frame
    void Update()
    {
        if(mask != null)
        {
            if (mask.GetComponent<RectTransform>().localPosition.x <= 0f)
            {

                capc.isTrigger = true;

                GameObject nextLevel = GameProcessManager.Singleton.LoadLevel(TargetLevelIndex);

                GameProcessManager.Singleton.InitPlayer(nextLevel);

                GameProcessManager.Singleton.DestroyLevel(transform.parent.gameObject);

                GameProcessManager.Singleton.InitCamera();

                CheckPointManager.Instance.Initialize();

                capc.isTrigger = false;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {



            CapsuleCollider2D collider = other.GetComponent<CapsuleCollider2D>();

            if (collider)
            {
                mask = GameObject.Find("HUD_Mask");
                Vector3 pos = mask.transform.position;
                Vector3 l_pos = mask.transform.localPosition;
                mask.AddComponent<LinearMovement>().enabled = false;
                LinearMovement lm = mask.GetComponent<LinearMovement>();
                lm.speed = speed;
                lm.isPersistent = false;

                if (direction == MovingDirection.R2L)
                {                  
                    lm.orientation = Vector3.left;

                    lm.initialPosition = pos;                       
                    
                }
                else
                {
                    lm.orientation = Vector3.right;

                    mask.transform.localPosition = new Vector3(l_pos.x - 5760, l_pos.y, l_pos.z);

                    lm.initialPosition = mask.transform.position;
                }

                lm.enabled = true;
                

                AudioManager.Singleton.PlayOnce("Scene_trans");

                capc = collider;

                c = other;
            }

            //gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = GizmoColor;
        //Gizmos.DrawCube(transform.position, transform.localScale);
        //if (TargetTeleportPosition != null)
        //    Gizmos.DrawCube(TargetTeleportPosition.position, new Vector3(1, 2, 1));
    }



}
