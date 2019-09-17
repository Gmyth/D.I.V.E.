using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SpatialTracking;

public enum CameraState
{
	Idle = 0, // the normal state of camera, in this state, camera will automatically adjust the position and scope for 4 players
	Focusing,// focus at a point.
    Release,
    Reset
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
	
	[SerializeField] private float defaultFieldOfView; 
	[SerializeField] private float smoothTimeZoom; 
	
	[SerializeField]private float offsetX = 0;
	[SerializeField]private float offsetY = 0;
	
	//Tracking list for all gameobject should be in screen at this moment
	private List<GameObject> targetList;
	private GameObject mainTarget;
	private List<CameraIndicator> indicatorList;
	
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
	private CameraState currentState = CameraState.Reset;
	void Start ()
	{
		mainTarget = GameObject.FindGameObjectWithTag("Player");
		targetList = new List<GameObject>();
		indicatorList = new List<CameraIndicator>();
		targetList.Add(mainTarget);
		Instance = this;
		Initialize();
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

	public void Initialize()
	{
		//collect all indicators in level
		indicatorList = GameObject.FindObjectsOfType<CameraIndicator>().ToList();
	}

	void FixedUpdate()
	{
		float shakeX = 0;
		float shakeY = 0;
		if (shake)
		{
			shakeX = Random.Range(-1f, 1f) * shakeMagnitude;
			shakeY = Random.Range(-1f, 1f) * shakeMagnitude;
		}
		var camera = GetComponent<Camera>();
		switch (currentState)
		{
			case CameraState.Idle:
				//Apply indicator config if indicator exists
				var targetIndicator = findCloestIndicator();
				if(targetIndicator) getCameraBoundary (targetIndicator.characterWindowBoundaryX,targetIndicator.characterWindowBoundaryY);
				else getCameraBoundary (characterWindowBoundaryX,characterWindowBoundaryY);
				
				float smallestSTY = targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothSmallestTimeY : smoothSmallestTimeY;
				float smallestSTX = targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothSmallestTimeX : smoothSmallestTimeX;
				
				float largestSTY = targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothLargestTimeY : smoothLargestTimeY;
				float largestSTX = targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothLargestTimeX : smoothLargestTimeX;
				
				float posY = transform.position.y;
				float posX = transform.position.x;

				float oX = targetIndicator ? targetIndicator.offsetX : offsetX;
				float oY = targetIndicator ? targetIndicator.offsetY : offsetY;
				
                Vector2 center = mainTarget.transform.position;
				
				//calculate the responsible smooth time based on the distance between character and Screen center
				float smoothTimeX = largestSTX - (largestSTX - smallestSTX) * getCurrentSmoothTimePer(center.x, currentLargestWindow.x, currentSmallestWindow.x);
				float smoothTimeY = largestSTY - (largestSTY - smallestSTY) * getCurrentSmoothTimePer(center.y, currentLargestWindow.y, currentSmallestWindow.y);
				
								
				if(targetIndicator && targetIndicator.bounds)
				{
//					transform.position = new Vector3(Mathf.Clamp(transform.position.x, targetIndicator.minCameraPos.x, targetIndicator.maxCameraPos.x)+ targetIndicator.offsetX,
//						Mathf.Clamp(transform.position.y, targetIndicator.minCameraPos.y, targetIndicator.maxCameraPos.y)+targetIndicator.offsetY,
//						transform.position.z
//					);
					center.x = Mathf.Min(Mathf.Max(center.x,targetIndicator.minCameraPos.x),targetIndicator.maxCameraPos.x);
					center.y = Mathf.Min(Mathf.Max(center.y,targetIndicator.minCameraPos.y),targetIndicator.maxCameraPos.y);
				}
				
				posX = Mathf.SmoothDamp(transform.position.x,center.x, ref velocity.x, smoothTimeX);
				posY = Mathf.SmoothDamp(transform.position.y,center.y, ref velocity.y, smoothTimeY);
				transform.position = new Vector3(posX + oX + shakeX, posY + oY + shakeY, transform.position.z);
				
				if(targetIndicator && targetIndicator.changeSize) camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetIndicator.targetCameraSize, ref tempVelocity, targetIndicator.smoothZoomTime);
				else camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, defaultFieldOfView, ref tempVelocity, smoothTimeZoom);
				break;
			
			case CameraState.Focusing:
				camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,cameraSizeOnFocusing, ref tempVelocity, smoothSmallestTimeX / 10);
				posX = Mathf.SmoothDamp(transform.position.x,target.position.x, ref velocity.x, smoothSmallestTimeX / 5);
				posY = Mathf.SmoothDamp(transform.position.y,target.position.y, ref velocity.y, smoothSmallestTimeY / 5);
				transform.position = new Vector3(posX + offsetX + shakeX, posY + offsetY + shakeY, transform.position.z);
				break;

            case CameraState.Reset:
	            
	            Vector2 origin = mainTarget.transform.position;
	            posX = Mathf.SmoothDamp(transform.position.x, origin.x, ref velocity.x, 0.2f);
	            posY = Mathf.SmoothDamp(transform.position.y, origin.y, ref velocity.y, 0.2f);

	            if (((Vector2) transform.position - origin).magnitude < 2)
	            {
		            currentState = CameraState.Idle;
	            }
	            
	            transform.position = new Vector3(posX + offsetX + shakeX, posY + offsetY + shakeY, transform.position.z);
                break;
            case CameraState.Release:
                break;

            
        }
	}
	
//	void OnDrawGizmos()
//	{
//		// Draw a yellow sphere at the transform's position
//		//Vector3 center = new Vector3((currentSmallestPlayer.x+currentLargestPlayer.x)/2,(currentSmallestPlayer.y+currentLargestPlayer.y)/2,1f); // get current center of players
//		Gizmos.color = Color.red;
//
//		Gizmos.DrawSphere(new Vector3(currentSmallestWindow.x,currentSmallestWindow.y,1f),0.5f);
//		
//		Gizmos.color = Color.blue;
//		Gizmos.DrawSphere(new Vector3(currentLargestWindow.x,currentLargestWindow.y,1f),0.5f);
//		
//		//Gizmos.color = Color.yellow;
//		//Gizmos.DrawSphere(center, 1);
//	}

	public void focusAt(Transform _target)
	{
		target = _target;
		currentState = CameraState.Focusing;
	}

    public void release()
    {
        currentState = CameraState.Release;
    }


    public void reset()
	{
		currentState = CameraState.Reset;
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

	private void getCameraBoundary(float boundaryX, float boundaryY)
	{
		// Screens coordinate corner location
		var camera = GetComponent<Camera>();
		//var upperLeftScreen = new Vector3(Screen.width*0.15f, Screen.height*0.75f, 0 );
//		var upperRightScreen = new Vector3(Screen.width*0.90f + camera.orthographicSize*8, Screen.height*0.90f + camera.orthographicSize*4, 0);
//		var lowerLeftScreen = new Vector3(Screen.width*0.10f - camera.orthographicSize*8, Screen.height*0.10f - camera.orthographicSize*4, 0);
		var upperRightScreen = new Vector3(Screen.width * boundaryX, Screen.height * boundaryY , -10);
		var lowerLeftScreen = new Vector3(Screen.width * (1 - boundaryX) , Screen.height * (1 - boundaryY), -10);
		//var lowerRightScreen = new Vector3(Screen.width*0.85f, Screen.height*0.25f, 0);
   
		//Corner locations in world coordinates
		
		Vector2 upperRight = camera.ScreenToWorldPoint(upperRightScreen);
		Vector2 lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);

		currentLargestWindow = lowerLeft;
		currentSmallestWindow = upperRight;
	}

	private float getCurrentSmoothTimePer(float pos, float largest,float smallest)
	{

		if (pos > largest || pos < smallest) return 1;
		
		float interval = (largest - smallest)/2;

		float absPos = pos > smallest + interval ? pos - interval - smallest : pos - smallest;

		float percentage = absPos / interval;
		
		return 1 - percentage;

	}

	private CameraIndicator findCloestIndicator()
	{
		//function for locating closet indicator in range
		if (indicatorList.Count == 0) return null;
		Vector2 pos = transform.position;
		var targetIndicator = indicatorList[0];
		bool found = false;
		foreach (var indicator in indicatorList)
		{
			if (indicator.influenceRange >= (pos - (Vector2) indicator.transform.position).magnitude)
			{

				if (found && targetIndicator.influenceLevel > indicator.influenceLevel)
				{
					//found new indicator with higher priority 

					targetIndicator = indicator;
				}
				else if (!found)
				{
					targetIndicator = indicator;
					found = true;
				}
			}
		}

		return found?targetIndicator:null;
	}




	//release the added target after x seconds
	private IEnumerator  releaseDelay(GameObject item, float delay)
	{
		yield return  new WaitForSeconds(delay);
		targetList.Remove(item);
		if(targetList.Count == 0) resetTarget();
	}
}