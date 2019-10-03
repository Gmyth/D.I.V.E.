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
    [SerializeField] protected StatisticSystem statistics;


    public abstract float ApplyDamage(int instanceId, float rawDamage, bool overWrite = false);

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
    [HideInInspector] public float currentAttackDamage = 0;

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
        get { return (Vector2)transform.position; }
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
