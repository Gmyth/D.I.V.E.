using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCar : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private Vector2 Bound;
    private Vector2 bound;
    
    [SerializeField] private GameObject children;
    
    
    // Start is called before the first frame update
    void Start()
    {

        bound = Bound;
        if (Random.Range(0, 1) == 0)bound = new Vector2(bound.y,bound.x);
        speed = Random.Range(3, 5);
        //transform.localPosition = new Vector3(bound.x, transform.localPosition.y, transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        float direction = (bound.y - bound.x) > 0 ? 1 : -1;
        transform.Translate(Time.fixedDeltaTime * TimeManager.Instance.TimeFactor * transform.right * speed * direction);

        if ((transform.localPosition - new Vector3(bound.y, transform.localPosition.y, transform.localPosition.z)).magnitude < 0.1f)
        {
            //reached
            bound = new Vector2(bound.y,bound.x);
        }
    }
}
