using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CameraState
{
	Idle = 0, // the normal state of camera, in this state, camera will automatically adjust the position and scope for 4 players
	Focusing, // focus at a point.
    Overview
}

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	public static CameraManager Instance;
	private float tempVelocity;
	private Vector2 velocity; // the speed reference for camera
	[Header("Camera")]

    // the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField] private float smoothSmallestTimeY;

    // the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField] private float smoothSmallestTimeX;


    // the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField]private float smoothLargestTimeY; 
	
	// the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
	[SerializeField]private float smoothLargestTimeX;

    [SerializeField] private float characterWindowBoundaryX;
    [SerializeField] private float characterWindowBoundaryY;

    // the smooth time for camera to zoom in, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField]private float smoothTimeZoomIn; 
	
	// the smooth time for camera to zoom out , the larger number will slow the camera moving speed. 0 will be response instantly 
	[SerializeField]private float smoothTimeZoomOut; 
	
	// how responsive you wanna the camera to do the zoom 
	[SerializeField]private float zoomSensity; 
	
	//Tracking list for all gameobject should be in screen at this moment
	private List<GameObject> targetList;
	private GameObject mainTarget;
	
	[Header("CameraBounds")]
	[SerializeField]private bool bounds;
	[SerializeField]private Vector3 maxCameraPos;
	[SerializeField]private Vector3 minCameraPos;
	[SerializeField]private float maxCameraSize;
	[SerializeField]private float minCameraSize;
	
	[Header("Focusing")]
	[SerializeField]private float cameraSizeOnFocusing;
	
	/*	private bool shaking;*/
	private bool shake;
	// currently the smallest magnitude it allowed is 0.01f;
	private float shakeMagnitude; 
	
	private bool refeshing;
	
	
	//Focusing related
	private Transform target;
	
	private float lastZoomOut; // record the last second for zooming out
	private float lastZoomIn; // record the last second for zooming in
	
	// The helper function return value
	private Vector2 currentSmallestPlayer; // position of player on lower left corner
	private Vector2 currentLargestPlayer; // position of player on upper upper corner
	private Vector2 currentSmallestWindow; // position of camera on lower left corner
	private Vector2 currentLargestWindow; // position of camera on upper upper corner
	
	private bool zoomInChasing; // the need of chasing character for a while 
	private CameraState currentState = CameraState.Idle;
	void Start ()
	{
		mainTarget = GameObject.FindGameObjectWithTag("Player");
		targetList = new List<GameObject>();
		targetList.Add(mainTarget);
		Instance = this;
	}

    public void resetTarget()
    {
	    mainTarget = GameObject.FindGameObjectWithTag("Player");
		targetList.Clear();
	    targetList.Add(mainTarget);
    }
	
	//change the target, clear current list and add the target to new one
	public void changeTarget(GameObject focusPoint)
	{
		targetList.Clear();
		targetList.Add(focusPoint);
	}
	
	//add the target to list, and can set release aftere serval seconds;
	public void addTempTarget(GameObject newItem, float releaseDlay = 0f)
	{
		targetList.Add(newItem);
		if (releaseDlay > 0 )
		{
			StartCoroutine(releaseDelay(newItem, releaseDlay));
		}
	}

	void FixedUpdate()
	{
		float xOffset = 0;
		float yOffset = 0;
		if (shake)
		{
			xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
			yOffset = Random.Range(-1f, 1f) * shakeMagnitude;
		}
		var camera = GetComponent<Camera>();
		switch (currentState)
		{
			case CameraState.Idle:
				float posy = transform.position.y;
				float posx = transform.position.x;
                getCameraBoundary();
                Vector2 center = mainTarget.transform.position;

				posx = Mathf.SmoothDamp(transform.position.x,center.x, ref velocity.x, smoothSmallestTimeX);
				posy = Mathf.SmoothDamp(transform.position.y,center.y, ref velocity.y, smoothSmallestTimeY);
				transform.position = new Vector3(posx + xOffset, posy + yOffset, transform.position.z);
				if (bounds)
				{
					transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x)+xOffset,
						Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y)+yOffset,
						transform.position.z
					);
				}
				break;
			
			case CameraState.Focusing:
				camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,cameraSizeOnFocusing, ref tempVelocity, smoothSmallestTimeX / 10);
				posx = Mathf.SmoothDamp(transform.position.x,target.position.x, ref velocity.x, smoothSmallestTimeX / 5);
				posy = Mathf.SmoothDamp(transform.position.y,target.position.y, ref velocity.y, smoothSmallestTimeY / 5);
				transform.position = new Vector3(posx + xOffset, posy + yOffset, transform.position.z);
				break;

            case CameraState.Overview:
                camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, 15f, ref tempVelocity, smoothSmallestTimeX * 2);
                posx = Mathf.SmoothDamp(transform.position.x, 0f, ref velocity.x, smoothSmallestTimeX);
                posy = Mathf.SmoothDamp(transform.position.y, 0f, ref velocity.y, smoothSmallestTimeY);
                transform.position = new Vector3(posx + xOffset, posy + yOffset, transform.position.z);
                break;

        }
	}
	
//	void OnDrawGizmos()
//	{
//		// Draw a yellow sphere at the transform's position
//		Vector3 center = new Vector3((currentSmallestPlayer.x+currentLargestPlayer.x)/2,(currentSmallestPlayer.y+currentLargestPlayer.y)/2,1f); // get current center of players
//		Gizmos.color = Color.red;
//
//		Gizmos.DrawSphere(new Vector3(currentLargestPlayer.x,currentLargestPlayer.y,1f),0.5f);
//		
//		Gizmos.color = Color.blue;
//		Gizmos.DrawSphere(new Vector3(currentLargestWindow.x,currentLargestWindow.y,1f),0.5f);
//		
//		Gizmos.color = Color.yellow;
//		Gizmos.DrawSphere(center, 1);
//	}

	public void focusAt(Transform _target)
	{
		target = _target;
		currentState = CameraState.Focusing;
	}

    public void Overview()
    {
        currentState = CameraState.Overview;
    }

    public void reset()
	{
		target = null;
		currentState = CameraState.Idle;
	}
	
	
	public void Shaking(float strength,float duration)
	{
		if (!shake)
		{
			refeshing = false;
			shake = true;
			shakeMagnitude = strength;
			StartCoroutine(shakeRelease(duration));
		}
		else
		{
			shakeMagnitude = strength;
			refeshing = true; // refresh current Coroutine
		}
	}

	IEnumerator shakeRelease(float duration)
	{
		int counter = 0;
		while (counter * 0.01f < duration)
		{
			if (refeshing)
			{
				//get refreshed
				counter = 0;
				refeshing = false; 
			}
			yield return new WaitForSeconds(0.01f);
			counter++;
		}
		shake = false;
		yield return null;
	}

	// helper function
	private void getPlayerBoundary()
	{
		float smallestX = 9999;
		float smallestY = 9999;
		float largestX = -9999;
		float largestY = -9999;
		foreach (var player in targetList)
		{
			var temp = player.transform.position;
			if (temp.x < smallestX)
			{
				smallestX = temp.x;
			}
			if (temp.x > largestX)
			{
				largestX = temp.x;
			}
			
			if (temp.y < smallestY)
			{
				smallestY = temp.y;
			}
			if (temp.y > largestY)
			{
				largestY = temp.y;
			}
		}
		currentLargestPlayer = new Vector2(largestX,largestY);
		currentSmallestPlayer = new Vector2(smallestX,smallestY);
	}

	private void getCameraBoundary()
	{
		// Screens coordinate corner location
		var camera = GetComponent<Camera>();
		//var upperLeftScreen = new Vector3(Screen.width*0.15f, Screen.height*0.75f, 0 );
//		var upperRightScreen = new Vector3(Screen.width*0.90f + camera.orthographicSize*8, Screen.height*0.90f + camera.orthographicSize*4, 0);
//		var lowerLeftScreen = new Vector3(Screen.width*0.10f - camera.orthographicSize*8, Screen.height*0.10f - camera.orthographicSize*4, 0);
		var upperRightScreen = new Vector3(Screen.width * characterWindowBoundaryX, Screen.height * (1 - characterWindowBoundaryY) , 0);
		var lowerLeftScreen = new Vector3(Screen.width * (1- characterWindowBoundaryX) , Screen.height * characterWindowBoundaryY, 0);
		//var lowerRightScreen = new Vector3(Screen.width*0.85f, Screen.height*0.25f, 0);
   
		//Corner locations in world coordinates
		
		var upperRight = camera.ScreenToWorldPoint(upperRightScreen);
		var lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);

		currentLargestWindow = upperRight;
		currentSmallestWindow = lowerLeft;
        print("UPPER RIGHT: " + upperRight);
        print("LOWERLEFT:" + lowerLeft);
	}
	
	//release the added target after x seconds
	private IEnumerator  releaseDelay(GameObject item, float delay)
	{
		yield return  new WaitForSeconds(delay);
		targetList.Remove(item);
		if(targetList.Count == 0) resetTarget();
	}
}