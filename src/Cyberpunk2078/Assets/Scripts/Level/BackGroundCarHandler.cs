using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCarHandler : MonoBehaviour
{
    public float width;
    
    // Start is called before the first frame update

    // Update is called once per frame
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(transform.position.x,transform.position.y,1f),new Vector3(width,3,0));
    }
}
