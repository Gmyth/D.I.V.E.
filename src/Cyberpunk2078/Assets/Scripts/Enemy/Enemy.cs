using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Dummy
{
    [SerializeField] private FSMEnemy fsm;

    [Header("Configuration")]
    [SerializeField] private int bulletID = 0;
    [SerializeField] private float fireInterval;
    [SerializeField] private float bulletSpeed;


    public int BulletID
    {
        get
        {
            return bulletID;
        }
    }

    public float FireInterval
    {
        get
        {
            return fireInterval;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
    }


    private void Start()
    {
        fsm.Initialize(this);
        fsm.Boot();
    }

    private void FixedUpdate()
    {
        fsm.Update();
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat("Enemy", "Take {0} damage.", rawDamage));
        return rawDamage;
    }
}
