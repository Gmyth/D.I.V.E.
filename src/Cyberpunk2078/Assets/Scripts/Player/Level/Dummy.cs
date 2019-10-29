﻿using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public struct RangedWeaponConfiguration
{
    [SerializeField] private float firingInterval;
    [SerializeField] private int bulletID;
    [SerializeField] private float bulletSpeed;


    public float FiringInterval
    {
        get
        {
            return firingInterval;
        }
    }

    public int BulletID
    {
        get
        {
            return bulletID;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
    }
}


[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collision2D))]
public abstract class Dummy : MonoBehaviour, IDamageable
{
    [SerializeField] protected StatisticSystem statistics;

    public bool isInvulnerable = false;
    public bool isEvading = false;

    public Event<Hit> OnHit { get; private set; } = new Event<Hit>();
    public UnityEvent OnAttack { get; private set; } = new UnityEvent();


    public Vector2 GroundNormal
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 3f);

            if (hit.collider && hit.transform.CompareTag("Ground"))
                return hit.normal;


            return Vector2.zero;
        }
    }


    public bool IsOnGround()
    {
        Vector3 position = transform.position;
        Vector3 up = transform.up;


        int layerMask = LayerMask.GetMask("Obstacle");

        RaycastHit2D hitM = Physics2D.Raycast(position + new Vector3(0f, -0.7f, 0f), -up, 0.6f, layerMask);
        RaycastHit2D hitL = Physics2D.Raycast(position + new Vector3(0.12f, -0.7f, 0f), -up, 0.6f, layerMask);
        RaycastHit2D hitR = Physics2D.Raycast(position + new Vector3(-0.12f, -0.7f, 0f), -up, 0.6f, layerMask);


        Debug.DrawRay(position + new Vector3(0f, -0.7f, 0f), -up * 0.6f, Color.red);
        Debug.DrawRay(position + new Vector3(0.12f, -0.7f, 0f), -up * 0.6f, Color.green);
        Debug.DrawRay(position + new Vector3(-0.12f, -0.7f, 0f), -up * 0.6f, Color.yellow);


        return hitM.collider || hitL.collider || hitR.collider;
    }


    public abstract float ApplyDamage(float rawDamage);

    public abstract void Dead();
}


public abstract class Enemy : Dummy
{
    [SerializeField] protected int typeID;
    [SerializeField] protected FSMEnemy fsm;
    [SerializeField] protected Zone guardZone;
    [SerializeField] protected HitBox[] hitBoxes;

    [Header("Patrolling")]
    [SerializeField][Path(true)] protected Route patrolRoute;
    [SerializeField] protected RangedWeaponConfiguration patrolFiringConfiguration;

    private EnemyData data;

    [HideInInspector] public PlayerCharacter currentTarget;
    [HideInInspector] public Hit currentHit;

    public Vector3 lastCheckPointTransform;


    public float this[StatisticType type]
    {
        get
        {
            return statistics[type];
        }
    }

    public Zone GuardZone
    {
        get
        {
            return guardZone;
        }
    }

    public Vector2 Pos
    {
        get
        {
            return transform.position;
        }
    }


    public void EnableHitBox(int index)
    {
        hitBoxes[index].hit = currentHit;
        hitBoxes[index].gameObject.SetActive(true);
    }

    public void DisableHitBox(int index)
    {
        hitBoxes[index].gameObject.SetActive(false);
    }


    protected virtual void Start()
    {
        data = DataTableManager.singleton.GetEnemyData(typeID);


        statistics = new StatisticSystem(data.Attributes);
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];


        fsm = fsm.Initialize(this);
        fsm.Boot();
    }

    protected virtual void FixedUpdate()
    {
        fsm.Update();
    }
}
