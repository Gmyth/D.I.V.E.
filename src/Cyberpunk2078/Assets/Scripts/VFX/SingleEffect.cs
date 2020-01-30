using UnityEngine;


public class SingleEffect : Recyclable
{
    public Transform target;
    
    
    private void Update()
    {
        if (target)
            transform.position = target.position;
    }


    public void setTarget(Transform t)
    {
        target = t;
    }
}
