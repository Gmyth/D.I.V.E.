using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGizmos : MonoBehaviour
{
    
    void OnDrawGizmosSelected()
    {
        /////////////////Draw grounded detect lines in *BLUE*////////////////////////
        Gizmos.color = Color.blue;

        Vector3 centerOffset = new Vector3(0f, 0.4f, 0f);
        Vector3 leftOffset = new Vector3(0.4f, 0.4f, 0f);
        Vector3 rightOffset = new Vector3(-0.4f, 0.4f, 0f);

        float length = 0.5f;

        Vector3 centerPoint = transform.position + centerOffset;
        Vector3 leftPoint = transform.position + leftOffset;
        Vector3 rightPoint = transform.position + rightOffset;

        Gizmos.DrawLine(centerPoint, centerPoint - new Vector3(0f, length, 0f));
        Gizmos.DrawLine(leftPoint, leftPoint - new Vector3(0f, length, 0f));
        Gizmos.DrawLine(rightPoint, rightPoint - new Vector3(0f, length, 0f));

        /////////////////
    }

}
