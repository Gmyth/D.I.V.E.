using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class MouseIndicator : MonoBehaviour
{
    // Start is called before the first frame 
    private GameObject player;
    private List<Vector2> directionRef;
    private float timeCount = 0.0f;
    private float deadZone = 0.2f;
    private Vector2 controllerDir;
    private float currTimeDead;
    void Start()
    {
        player  = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
        if (Mathf.Abs(Input.GetAxis("HorizontalJoyStick")) > 0 || Mathf.Abs(Input.GetAxis("VerticalJoyStick")) > 0)
        {
            var h = Input.GetAxis("HorizontalJoyStick");
            var v = Input.GetAxis("VerticalJoyStick");
            controllerDir = new Vector3(Input.GetAxis("HorizontalJoyStick"),Input.GetAxis("VerticalJoyStick")).normalized;
//            if(h != 0 || v != 0)
//            {
//                    // If we're coming from a diagonal direction, wait for a bit
//                    if(Mathf.Abs(controllerDir.x) > 0 && Mathf.Abs(controllerDir.y) > 0 && currTimeDead <= 0)
//                        currTimeDead = 0.05f;
//                    if(currTimeDead > 0)
//                        currTimeDead -= Time.deltaTime;
//                    // If we're done waiting, set the direction
//                    if(currTimeDead <= 0)
//                    {
//                        controllerDir.x = h;
//                        controllerDir.y = v;
//                        controllerDir.Normalize();
//                    }
//            }
        }
        mousePosInScreen.z = 10; // select distance = 10 units from the camera
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInScreen);
        Vector2 direction = -(mousePos - (Vector2) transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle,Vector3.forward);


        transform.rotation = rotation;
//            Quaternion.Slerp(transform.rotation,rotation,timeCount);
//        timeCount += Time.deltaTime * 8;
    }


    public Vector2 getAttackDirection()
    {
        if (controllerDir != Vector2.zero)
        {
            return controllerDir;
        }

        transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
        mousePosInScreen.z = 10; // select distance = 10 units from the camera
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInScreen);
        return   (mousePos - (Vector2) transform.position).normalized;
    }
    
//    public Vector2 getTranslatedDirection(Vector2 direction)
//    {
//        // the direction parameter must be normalized
//        foreach (var scaler in directionRef)
//        {
//            if (Mathf.Abs(Vector2.Angle(scaler, direction)) < 23f)
//            {
//                return scaler;
//            }
//        }
//        
//        // should not reach here
//        return Vector2.zero;
//    }
}
