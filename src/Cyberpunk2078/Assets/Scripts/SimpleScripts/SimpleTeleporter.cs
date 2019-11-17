using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleporter : MonoBehaviour
{

    public Transform TargetTeleportPosition;

    public Color GizmoColor;
    
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
        if (other.gameObject.tag == "Player") {
            other.gameObject.transform.position = TargetTeleportPosition.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.DrawCube(TargetTeleportPosition.position, new Vector3(1, 2, 1));
    }

}
