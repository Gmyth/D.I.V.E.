﻿using System.Collections.Generic;
using UnityEngine;

public class HitBoxGroup : MonoBehaviour
{
    [HideInInspector] public Dictionary<int, int> objectsHit = new Dictionary<int, int>();

    private int numActivatedHitBoxes = 0;


    public void OnHitBoxEnable(HitBox hitBox)
    {
        ++numActivatedHitBoxes;
    }

    public void OnHitBoxDisable(HitBox hitBox)
    {
        if (--numActivatedHitBoxes == 0)
            objectsHit.Clear();
    }
}
