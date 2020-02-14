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
    public Transform TargetTeleportPosition;

    public GameObject TargetLevel;

    public Color GizmoColor;

    private GameObject mask;

    public MovingDirection d;

    [SerializeField] private float speed = 5f;

    private CapsuleCollider2D capc;

    private Collider2D c;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mask != null)
        {
            if (mask.GetComponent<RectTransform>().position.x <= 1920f)
            {
                //TargetLevel.SetActive(true);
                CameraManager.Instance.Initialize();

                capc.isTrigger = true;
                GameObject level = GameProcessManager.Singleton.LoadLevel(1);
                var sp = level.GetComponent<LevelInfo>().StartPoint;
                c.gameObject.transform.position = sp.transform.position;
                CameraManager.Instance.ResetTarget();
                CameraManager.Instance.Reset();
                capc.isTrigger = false;

                transform.parent.gameObject.SetActive(false);
                
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
                mask.AddComponent<LinearMovement>().enabled = false;
                LinearMovement lm = mask.GetComponent<LinearMovement>();
                lm.speed = speed;

                if (d == MovingDirection.R2L)
                {                  
                    lm.orientation = Vector3.left;

                    lm.initialPosition = pos;                       
                    
                }
                else
                {
                    lm.orientation = Vector3.right;

                    pos = new Vector3(pos.x - 1920, pos.x, pos.z);

                    lm.initialPosition = pos;
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
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
       // if (TargetTeleportPosition != null)
        //    Gizmos.DrawCube(TargetTeleportPosition.position, new Vector3(1, 2, 1));
    }



}
