using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public struct RangedWeaponConfiguration
{
    [SerializeField] private float firingInterval;
    [SerializeField] private float chargeTime;
    [SerializeField] private Transform muzzle;
    [SerializeField] private int bulletID;
    [SerializeField] private float bulletSpeed;

    [Header("Deviation")]
    [SerializeField] private float minDeviationAngle;
    [SerializeField] private float maxDeviationAngle;


    public float FiringInterval
    {
        get
        {
            return firingInterval;
        }
    }

    public float ChargeTime
    {
        get
        {
            return chargeTime;
        }
    }

    public Transform Muzzle
    {
        get
        {
            return muzzle;
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

    public float MinDeviationAngle
    {
        get
        {
            return minDeviationAngle;
        }
    }

    public float MaxDeviationAngle
    {
        get
        {
            return maxDeviationAngle;
        }
    }
}


[RequireComponent(typeof(Rigidbody2D), typeof(Collision2D))]
public abstract class Dummy : MonoBehaviour, IDamageable
{
    public class HitEvent : UnityEvent<Hit, Collider2D> { }


    [SerializeField] protected StatisticSystem statistics;

    public bool isInvulnerable = false;
    public bool isEvading = false;

    public HitEvent OnHit { get; private set; } = new HitEvent();
    public HitEvent OnAttack { get; private set; } = new HitEvent();


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


    public bool IsOnGround(float xOffset = 0.1f, float yOffset = 0, float length = 0.1f)
    {
        Vector3 position = transform.position;
        Vector3 up = transform.up;


        int layerMask = LayerMask.GetMask("Obstacle");

        RaycastHit2D hitM = Physics2D.Raycast(position + new Vector3(0f, yOffset, 0f), -up, length, layerMask);
        RaycastHit2D hitL = Physics2D.Raycast(position + new Vector3(xOffset, yOffset, 0f), -up, length, layerMask);
        RaycastHit2D hitR = Physics2D.Raycast(position + new Vector3(-xOffset, yOffset, 0f), -up, length, layerMask);


        Debug.DrawRay(position + new Vector3(0f, yOffset, 0f), -up * length, Color.red);
        Debug.DrawRay(position + new Vector3(xOffset, yOffset, 0f), -up * length, Color.green);
        Debug.DrawRay(position + new Vector3(-xOffset, yOffset, 0f), -up * length, Color.yellow);


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

    protected EnemyData data;

    [HideInInspector] public AttributeSet statusModifiers = new AttributeSet();
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

    public EnemyData Data
    {
        get
        {
            return data;
        }
    }

    public Vector2 Pos
    {
        get
        {
            return transform.position;
        }
    }

    public FSMEnemy FSM
    {
        get
        {
            return fsm;
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

    public void DisableAllHitBoxes()
    {
        foreach (HitBox hitBox in hitBoxes)
            hitBox.gameObject.SetActive(false);
    }

    public void SwitchHitBox(int previousIndex, int currentIndex)
    {
        DisableHitBox(previousIndex);
        EnableHitBox(currentIndex);
    }


    protected virtual void Start()
    {
        data = DataTableManager.singleton.GetEnemyData(typeID);


        statistics = new StatisticSystem(data.Attributes, statusModifiers);
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];

        if (fsm)
        {
            fsm = fsm.Initialize(this);
            fsm.Boot();
        }
    }

    protected virtual void FixedUpdate()
    {
        fsm?.Update();
    }

    public void Reset()
    {
        fsm?.Reboot();
    }
}
