using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleItem : MonoBehaviour
{
    private float defaultDrag;
    private float defaultMass;
    
    private void OnEnable()
    {
        var rb2d = GetComponent<Rigidbody2D>();
        defaultDrag = rb2d.drag;
        defaultMass = rb2d.mass;
    }

    // Update is called once per frame
    private void Update()
    {
        var rb2d = GetComponent<Rigidbody2D>();
        rb2d.drag = defaultDrag / TimeManager.Instance.TimeFactor;
        rb2d.mass = defaultMass / TimeManager.Instance.TimeFactor;
    }
    
}
