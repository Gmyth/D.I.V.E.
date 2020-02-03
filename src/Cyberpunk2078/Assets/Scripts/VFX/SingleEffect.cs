using UnityEngine;


public class SingleEffect : Recyclable
{
    public Transform target;
    
    
    private void Update()
    {
        if (target)
            transform.position = target.position;


        if (GetComponentInChildren<Animator>())
            GetComponentInChildren<Animator>().speed = TimeManager.Instance.TimeFactor;
    }

    public void setTarget(Transform t)
    {
        target = t;
    }
}
