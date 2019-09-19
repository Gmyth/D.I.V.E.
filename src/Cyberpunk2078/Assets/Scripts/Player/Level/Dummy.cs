using System;
using UnityEngine;


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
    public abstract float ApplyDamage(int instanceId, float rawDamage, bool overWrite = false);

    public abstract void Dead();
}


public abstract class Enemy : Dummy
{
    [SerializeField] protected FSMEnemy fsm;
    [SerializeField] protected StatisticSystem statistics;
    [SerializeField] protected Zone guardZone;
    [SerializeField] protected HitBox[] hitBoxes;

    [HideInInspector] public PlayerCharacter currentTarget;
    [HideInInspector] public float currentAttackDamage = 0;

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

    public float SightRange
    {
        get
        {
            return 10f;
        }
    }


    protected void EnableHitBox(int index)
    {
        hitBoxes[index].damage = currentAttackDamage;
        hitBoxes[index].gameObject.SetActive(true);
    }

    protected void DisableHitBox(int index)
    {
        hitBoxes[index].gameObject.SetActive(false);
    }


    protected virtual void Start()
    {
        fsm.Initialize(this);
        fsm.Boot();
    }

    protected virtual void FixedUpdate()
    {
        fsm.Update();
    }
}
