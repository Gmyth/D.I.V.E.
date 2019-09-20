using System.Collections;
using System.Collections.Generic;
using MyBox;
using  UnityEditor;
using UnityEngine;
public enum IndicatorType{
	Circle = 1,
	Square,
}

public class CameraIndicator : MonoBehaviour
{
	// Actual range of this indicator can influence
	
	[Header("Indicator General")]
	
	[SerializeField]private Color color;
	
	[Header("Indicator Config")]
	// the smaller influence level it is, the higher priority it has when overlap happened
	public float influenceLevel;
	public IndicatorType type;
	[ConditionalField(nameof(type), false, IndicatorType.Circle)] public float radius;
	[ConditionalField(nameof(type), false, IndicatorType.Square)] public float width;
	[ConditionalField(nameof(type), false, IndicatorType.Square)] public float length;
	 

	
	[Header("Camera Move Speed")]
	
	public bool changeMoveSpeed;
	// the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	[ConditionalField(nameof(changeMoveSpeed), false, true)] public float smoothTimeY;

	// the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	[ConditionalField(nameof(changeMoveSpeed), false, true)] public float smoothTimeX;
	
	[Header("Camera Window")]
	public bool changeWindow;
	[ConditionalField(nameof(changeWindow), false, true)] public float characterWindowFreeBoundaryX;
	[ConditionalField(nameof(changeWindow), false, true)] public float characterWindowFreeBoundaryY;
	
	[Header("Camera Offset")]
	public bool changeOffset;
	[ConditionalField(nameof(changeOffset), false, true)] public float offsetX;
	[ConditionalField(nameof(changeOffset), false, true)] public float offsetY;

	[Header("Camera Size")]
	public bool changeSize;
	[ConditionalField(nameof(changeSize), false, true)] public float targetCameraSize;
	[ConditionalField(nameof(changeSize), false, true)] public float smoothZoomTime;
	
	[Header("Camera Bounds")]
	public bool bounds;
	[ConditionalField(nameof(bounds), false, true)] public Vector2 maxCameraPos;
	[ConditionalField(nameof(bounds), false, true)] public Vector2 minCameraPos;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position and the radius is the influence range
		Gizmos.color = color;
		switch (type)
		{
			case IndicatorType.Circle:
				Gizmos.DrawSphere(new Vector3(transform.position.x,transform.position.y,1f),radius);
				break;
			case IndicatorType.Square:
				Gizmos.DrawCube(new Vector3(transform.position.x,transform.position.y,1f),new Vector3(width,length,0));
				break;
		}
		
	}


	public bool inRange(Vector2 target)
	{
		// check if target is in range
		if (type == IndicatorType.Circle)
		{
			if ((target - (Vector2) transform.position).magnitude <= radius)
			{
				// in range
				return true;
			}

			return false;
		}
		
		if (type == IndicatorType.Square)
		{
			Vector2 smallest = new Vector2(transform.position.x - width/2,transform.position.y - length/2);
			Vector2 largest = new Vector2(transform.position.x + width/2,transform.position.y + length/2);
			if (target.x <= largest.x 
			    && target.x >= smallest.x 
			    && target.y <= largest.y 
			    && target.y >= smallest.y)
			{
				// in range
				return true;
			}
			return false;
		}

		return false;
	}

}
