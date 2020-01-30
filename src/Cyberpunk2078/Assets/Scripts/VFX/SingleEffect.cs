using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEffect : Recyclable
{

    private Transform target;
    
    
    private void Update()
    {
        if (target)
        {
            transform.position = target.position;
        }

    }
    
    public void setTarget(Transform _target)
    {
        target = _target;
    }
    
}
