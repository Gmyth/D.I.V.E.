﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Attack_GD", menuName = "Player State/Attack GD")]
public class PSAttackGD : PlayerState
{
    [Header("Configuration")]
    [SerializeField] private float recoveryTime = 0.5f;

    [Header("Connected States")]
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;

    private float t0 = 0;


    public override int Update()
    {
        if (Time.time - t0 > recoveryTime)
        {
            if (Input.GetAxis("Horizontal") == 0)
                return index_PSIdle;

            return index_PSMoving;
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        t0 = Time.time;

        playerCharacter.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
    }
}
