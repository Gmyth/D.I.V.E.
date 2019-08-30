using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIndicator : MonoBehaviour
{
	// Actual range of this indicator can influence
	public float influenceRange;
	
	// the smaller influence level it is, the higher priority it has when overlap happened
	public float influenceLevel;
	[SerializeField]private Color color;
	
	[Header("CameraGeneral")]
	
	public bool changeMoveSpeed;
	// the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	public float smoothSmallestTimeY;

	// the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	public float smoothSmallestTimeX;


	// the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	public float smoothLargestTimeY; 
	
	// the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	public float smoothLargestTimeX;
	
	public float characterWindowBoundaryX;
	public float characterWindowBoundaryY;
	
	public float offsetX;
	public float offsetY;

	public bool changeSize;
	public float targetCameraSize;
	public float smoothZoomTime;
	
	[Header("CameraBounds")]
	public bool bounds;
	public Vector2 maxCameraPos;
	public Vector2 minCameraPos;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position and the radius is the influence range
		Gizmos.color = color; 
		Gizmos.DrawSphere(new Vector3(transform.position.x,transform.position.y,1f),influenceRange);
	}

}
