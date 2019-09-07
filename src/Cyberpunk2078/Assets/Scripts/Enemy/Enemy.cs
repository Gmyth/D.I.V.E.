using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Dummy
{
    [SerializeField] private FSMEnemy fsm;


    private void Start()
    {
        fsm.Initialize(this);
        fsm.Boot();
    }

    private void FixedUpdate()
    {
        fsm.Update();
    }
}
