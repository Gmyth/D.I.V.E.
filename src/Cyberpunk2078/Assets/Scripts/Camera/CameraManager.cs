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
    Follow,
    Reset
}

public class CameraManager : MonoBehaviour {

	// Use this for initialization
	public static CameraManager Instance;
	private float zoomVelocity;
	private Vector2 velocity; // the speed reference for camera
	private Vector2 focusVelocity;
	[Header("Camera")]

    // the smooth time for camera change the position on Y - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField] private float smoothTimeY;

    // the smooth time for camera change the position on X - axis, the larger number will slow the camera moving speed. 0 will be response instantly 
    [SerializeField] private float smoothTimeX;

    [SerializeField] private float characterWindowFreeBoundaryX;
    [SerializeField] private float characterWindowFreeBoundaryY;
    
    [SerializeField] private float CameraChaseDelay;

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
	private bool shakeRefreshing;
	
	/*	private bool flashIn;*/
	private bool flash;
	private bool flashRefreshing;
	private bool flashOut;
	
	// currently the smallest magnitude it allowed is 0.01f;
	private float shakeMagnitude; 
	
	
	
	//Focusing related
	private Transform target;

	private float defaultSize;
	private float lastZoomIn; // record the last second for zooming in
	private float ZoomDuration; // record the last second for zooming in
	private float targetZoomSize;
	private float smoothTimeQZ;

	//Follow related
	private Vector2 followOffset;
	
	// The helper function return value
	private Vector2 currentSmallestTolerancePos; // position of smallest TolerancePos that allow character to move without Camera Correction
	private Vector2 currentLargestTolerancePos; // position of largest TolerancePos that allow character to move without Camera Correction


	private bool zoomInChasing; // the need of chasing character for a while 
	private CameraState currentState = CameraState.Reset;
	void Start ()
	{
		defaultSize = Camera.main.orthographicSize;
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

	void LateUpdate()
	{
		float shakeX = 0;
		float shakeY = 0;
		if (shake)
		{
			shakeX = Random.Range(-1f, 1f) * shakeMagnitude;
			shakeY = Random.Range(-1f, 1f) * shakeMagnitude;
		}
		var camera = GetComponentInChildren<Camera>();
		var targetIndicator = findClosestIndicator();
		switch (currentState)
		{
			case CameraState.Idle:
				//Apply indicator config if indicator exists
				float oX = targetIndicator && targetIndicator.changeOffset ? targetIndicator.offsetX : offsetX;
				float oY = targetIndicator && targetIndicator.changeOffset ? targetIndicator.offsetY : offsetY;
				
				if(targetIndicator && targetIndicator.changeWindow){ 
					getCameraBoundary (targetIndicator.characterWindowFreeBoundaryX, targetIndicator.characterWindowFreeBoundaryY, oX, oY);
					
				}
				else
				{
					getCameraBoundary (characterWindowFreeBoundaryX, characterWindowFreeBoundaryY,oX,oY);
				}
				
				
				float posY = transform.position.y;
				float posX = transform.position.x;

				
                Vector2 center = mainTarget.transform.position;
				
				//calculate the responsible smooth time based on the distance between character and Screen center




				Vector2 targetPos = new Vector2(
					posX + getDstMovement(center.x,currentLargestTolerancePos.x,currentSmallestTolerancePos.x),
					posY + getDstMovement(center.y,currentLargestTolerancePos.y,currentSmallestTolerancePos.y)
					);

                //velocity = Vector2.zero;
                //print(targetPos);
                if (targetIndicator && targetIndicator.bounds)
                {
                    targetPos.x = Mathf.Min(Mathf.Max(targetPos.x, targetIndicator.minCameraPos.x), targetIndicator.maxCameraPos.x);
                    targetPos.y = Mathf.Min(Mathf.Max(targetPos.y, targetIndicator.minCameraPos.y), targetIndicator.maxCameraPos.y);
                }

                
                posX = Mathf.SmoothDamp(transform.position.x,targetPos.x, ref velocity.x, 
					targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothTimeX : smoothTimeX);
				posY = Mathf.SmoothDamp(transform.position.y,targetPos.y, ref velocity.y, 
					targetIndicator && targetIndicator.changeMoveSpeed ? targetIndicator.smoothTimeY : smoothTimeY);
				

				transform.position = new Vector3(posX + shakeX, posY + shakeY, transform.position.z);

				if (targetIndicator && targetIndicator.changeSize)
				{
					defaultSize = targetIndicator.targetCameraSize;
					if (flash)
					{
						if (!flashOut)
						{
							// zoom in to target ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,targetZoomSize, ref zoomVelocity, smoothTimeQZ);
						}
						else
						{
							//zoom out to origin ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,defaultSize, ref zoomVelocity, smoothTimeQZ);
						}
					}
					else
					{
						// back to normal
						camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetIndicator.targetCameraSize, ref zoomVelocity, targetIndicator.smoothZoomTime);
					}
				}
				else
				{
					defaultSize = defaultFieldOfView;
					if (flash)
					{
						if (!flashOut)
						{
							// zoom in to target ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,targetZoomSize, ref zoomVelocity, smoothTimeQZ);
						}
						else
						{
							//zoom out to origin ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,defaultSize, ref zoomVelocity, smoothTimeQZ);
						}
					}
					else
					{
						// back to normal
						camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, defaultFieldOfView, ref zoomVelocity, smoothTimeZoom);
					}
				}
				
				
				break;
			
			case CameraState.Focusing:
				
				// prevent overchasing
				if ((target.position - transform.position).sqrMagnitude > 1000)
				{
					break;
				}

				posX = Mathf.SmoothDamp(transform.position.x,target.position.x, ref focusVelocity.x, 0.001f);
				posY = Mathf.SmoothDamp(transform.position.y,target.position.y, ref focusVelocity.y, 0.001f);
				transform.position = new Vector3(posX, posY, transform.position.z);
				
				if (targetIndicator && targetIndicator.changeSize)
				{
					defaultSize = targetIndicator.targetCameraSize;
					if (flash)
					{
						if (!flashOut)
						{
							// zoom in to target ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,targetZoomSize, ref zoomVelocity, smoothTimeQZ);
						}
						else
						{
							//zoom out to origin ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,defaultSize, ref zoomVelocity, smoothTimeQZ);
						}
					}
					else
					{
						// back to normal
						camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetIndicator.targetCameraSize, ref zoomVelocity, targetIndicator.smoothZoomTime);
					}
				}
				else
				{
					defaultSize = defaultFieldOfView;
					if (flash)
					{
						if (!flashOut)
						{
							// zoom in to target ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,targetZoomSize, ref zoomVelocity, smoothTimeQZ);
						}
						else
						{
							//zoom out to origin ZoomSize
							camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,defaultSize, ref zoomVelocity, smoothTimeQZ);
						}
					}
					else
					{
						// back to normal
						camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize,cameraSizeOnFocusing, ref zoomVelocity, smoothTimeX / 10);
					}
				}
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
            
            
			case CameraState.Follow:
				var indicator = findClosestIndicator();
				Vector2 FollowPos = (Vector2)mainTarget.transform.position + followOffset;
				
				if(indicator && indicator.bounds)
				{
					FollowPos.x = Mathf.Min(Mathf.Max(FollowPos.x,indicator.minCameraPos.x),indicator.maxCameraPos.x);
					FollowPos.y = Mathf.Min(Mathf.Max(FollowPos.y,indicator.minCameraPos.y),indicator.maxCameraPos.y);
				}
				
				posX = Mathf.SmoothDamp(transform.position.x,FollowPos.x, ref velocity.x, 
					0.2f);
				posY = Mathf.SmoothDamp(transform.position.y,FollowPos.y, ref velocity.y, 
					0.2f);
				transform.position = new Vector3(posX + offsetX + shakeX, posY + offsetY + shakeY, transform.position.z);
				break;

            
        }
	}
	
	void OnDrawGizmos()
	{
		// Draw a yellow sphere at the transform's position
		//Vector3 center = new Vector3((currentSmallestPlayer.x+currentLargestPlayer.x)/2,(currentSmallestPlayer.y+currentLargestPlayer.y)/2,1f); // get current center of players
		
		
		Gizmos.color =  new Color (0.5f, 0.8f, 0, .5f);
		Vector3 center = (currentLargestTolerancePos - currentSmallestTolerancePos) / 2 + currentSmallestTolerancePos;
		Gizmos.DrawCube (center, currentLargestTolerancePos - currentSmallestTolerancePos); 

		//Gizmos.DrawSphere(center, 1);
	}

	public void FocusAt(Transform _target, float duration = -1f)
	{
		target = _target;
		currentState = CameraState.Focusing;
		if (duration > -1f) StartCoroutine(resetDelay(duration));
	}

    public void Release()
    {
        currentState = CameraState.Release;
    }


    public void Reset()
	{
		currentState = CameraState.Reset;
	}
    
    public void Idle()
    {
	    currentState = CameraState.Idle;
        flash = false;
        flashOut = false;
        shake = false;
    }

    private IEnumerator resetDelay(float duration)
    {
	    if(!target) Reset();
	    yield return  new WaitForSeconds(duration);
	    if(currentState != CameraState.Reset)Reset();
    }

    public void FlashIn(float targetSize, float smoothTime, float inDuration, float outDuration)
    {
	    if (!flash)
	    {
		    flashRefreshing = false;
		    flash = true;
		    targetZoomSize = targetSize;
		    smoothTimeQZ = smoothTime;
		    StartCoroutine(flashRelease(inDuration,outDuration));
	    }
	    else
	    {
		    targetZoomSize = targetSize;
		    smoothTimeQZ = smoothTime;
		    flashRefreshing = true; // refresh current Coroutine
	    }
    }
    
    IEnumerator flashRelease(float inDuration,float outDuration)
    {
	    int counter = 0;
	    while (counter * 0.01f < inDuration)
	    {
		    // Zoom in Duration
		    
		    if (flashRefreshing)
		    {
			    //get refreshed
			    counter = 0;
			    flashRefreshing = false; 
		    }
		    yield return new WaitForSeconds(0.01f);
		    counter++;
	    }
	    
	    // flash out start
	    flashOut = true;
	    counter = 0;
	    
	    while (counter * 0.01f < outDuration)
	    {
		    // Zoom in Duration
		    if (flashRefreshing)
		    {
			    //get refreshed
			    counter = 0;
			    flashRefreshing = false;
			    break;
		    }
		    yield return new WaitForSeconds(0.01f);
		    counter++;
	    }
	    
	    flash = false;
	    flashOut = false;
	    yield return null;
    }

    public void Follow(float duration = -1)
    {
	    if (currentState!=CameraState.Follow)
	    {
		    var previous = currentState;
		    currentState = CameraState.Follow;
		    followOffset = transform.position - mainTarget.transform.position;
		    if(duration > 0)StartCoroutine(followRelease(previous,duration));
	    }
	    else
	    {
		    // already followed
	    }
    }

    IEnumerator followRelease(CameraState previous, float duration)
    {
	    float start = Time.time;
	    yield return  new WaitForSeconds(duration);
	    currentState = previous;
	    yield return null;
    }

    public void Shaking(float strength, float duration, bool overwrite = false)
	{
		if (!shake)
		{
			shakeRefreshing = false;
			shake = true;
			shakeMagnitude = strength;
			StartCoroutine(shakeRelease(duration));
		}
		else
		{
			if (overwrite)
			{
				shakeMagnitude = strength;
				StartCoroutine(shakeRelease(duration,overwrite));
				shakeRefreshing = true; // refresh current Coroutine
			}
			
		}
	}

    IEnumerator shakeRelease(float duration, bool overwrite = false)
	{
		if (shakeRefreshing)
		{
			shakeRefreshing = false;
		}
		
		bool stop = true;
		
		int counter = 0;
		while (counter * 0.01f < duration)
		{
			if (shakeRefreshing && !overwrite)
			{
				//get refreshed
				stop = false;
				break;
			}
			
			yield return new WaitForSeconds(0.01f);
			counter++;
		}

		if (stop) { shake = false; }
		
		yield return null;
	}

	private void getCameraBoundary(float boundaryX, float boundaryY, float oX, float oY)
	{
		// Screens coordinate corner location
		var camera = GetComponent<Camera>();

		var upperRightScreen = new Vector3(Screen.width * boundaryX, Screen.height * boundaryY , -10);
		var lowerLeftScreen = new Vector3(Screen.width * (1 - boundaryX) , Screen.height * (1 - boundaryY), -10);

		//Corner locations in world coordinates
		Vector2 upperRight = camera.ScreenToWorldPoint(upperRightScreen);
		Vector2 lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);
		Vector2 offset = new Vector2(oX,oY);
		currentSmallestTolerancePos = upperRight + offset;
		currentLargestTolerancePos = lowerLeft + offset;
	}
	
	
	private float getDstMovement(float pos, float larges,float smallest)
	{
		
		if (pos < smallest)
		{
			return pos - smallest;
		}
		
		if (pos > larges)
		{
			return pos - larges;
		}

		return 0f;

	}


	private CameraIndicator findClosestIndicator()
	{
		//function for locating closet indicator in range
		if (indicatorList.Count == 0) return null;
		Vector2 pos = mainTarget.transform.position;
		var targetIndicator = indicatorList[0];
		bool found = false;
		foreach (var indicator in indicatorList)
		{
			if (indicator.inRange(pos))
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