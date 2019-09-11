using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class MouseIndicator : MonoBehaviour
{
    // Start is called before the first frame 
    private GameObject player;
    private List<Vector2> directionRef;
    private float timeCount = 0.0f;
    void Start()
    {
        directionRef = new List<Vector2>();
        directionRef.Add(new Vector2(0f,1f)); //up
        directionRef.Add(new Vector2(-1f,0f)); //left
        directionRef.Add(new Vector2(1f,0f)); //right
        directionRef.Add(new Vector2(0f,-1f)); //down
        directionRef.Add(new Vector2(-0.5f,0.5f)); //upper left
        directionRef.Add(new Vector2(0.5f,0.5f)); //upper right
        directionRef.Add(new Vector2(-0.5f,-0.5f)); //down left
        directionRef.Add(new Vector2(0.5f,-0.5f)); //down right
        player  = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
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
