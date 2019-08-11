using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    // Start is called before the first frame 
    private GameObject player;
    void Start()
    {
        player  = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        var mousePosInScreen = Input.mousePosition;
        mousePosInScreen.z = -10; // select distance = 10 units from the camera
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(mousePosInScreen);
        Vector2 direction = (mousePos - (Vector2) transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle,Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,8*Time.deltaTime);
    }


    public Vector2 getAttackDirection()
    { 
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return   (mousePos - (Vector2) transform.position).normalized;
    }
}
