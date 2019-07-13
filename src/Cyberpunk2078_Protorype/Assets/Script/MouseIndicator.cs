using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
