using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleporter : MonoBehaviour
{

    public Transform TargetTeleportPosition;
    public GameObject TargetLevel;

    public Color GizmoColor;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {



            CapsuleCollider2D collider = other.GetComponent<CapsuleCollider2D>();

            if (collider)
            {
                TargetLevel.SetActive(true);
                CameraManager.Instance.Initialize();

                collider.isTrigger = true;
                other.gameObject.transform.position = TargetTeleportPosition.position;
                collider.isTrigger = false;

                transform.parent.gameObject.SetActive(false);
            }

            //gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
        if (TargetTeleportPosition != null)
            Gizmos.DrawCube(TargetTeleportPosition.position, new Vector3(1, 2, 1));
    }

}
