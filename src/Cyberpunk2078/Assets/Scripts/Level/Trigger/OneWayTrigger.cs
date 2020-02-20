using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayTrigger : TriggerObject
{
    // Start is called before the first frame update
    public override void Enable()
    {
        gameObject.SetActive(false);
    }

    public override void Disable()
    {
    }
}
