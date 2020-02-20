using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundCar : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private Vector2 Bound;
    private Vector2 bound;
    
    [SerializeField] private GameObject children;

    private BackGroundCarHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        handler = GetComponentInParent<BackGroundCarHandler>();
        if(handler) bound = new Vector2(-handler.width/2,handler.width/2);
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

        if ((direction > 0 && transform.localPosition.x > bound.y) ||(direction < 0 && transform.localPosition.x < bound.y))
        {
            //reached
            bound = new Vector2(bound.y,bound.x);
        }
    }
}
