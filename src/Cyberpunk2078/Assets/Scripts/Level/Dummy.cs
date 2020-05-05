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

    [SerializeField] private bool isInvulnerable = false;
    public bool isEvading = false;
    public float UnitTimeFactor = 1;


    public HitEvent OnHit { get; private set; } = new HitEvent();
    public HitEvent OnAttack { get; private set; } = new HitEvent();

    public EventOnStatisticChange OnStatisticChange => statistics.onStatisticChange;

    public bool IsInvulnerable
    {
        get
        {
            return isInvulnerable;
        }

        set
        {
            isInvulnerable = value;
        }
    }

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


    public virtual float ApplyDamage(float rawDamage)
    {
        float damage = rawDamage;


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -damage, 0);

        if (result.currentValue <= 0)
            Dead();


        return result.previousValue - result.currentValue;
    }

    public abstract void Dead();
}


public abstract class Enemy : Dummy
{
    [SerializeField] protected int typeID;
    protected EnemyData data;

    [SerializeField] protected FSMEnemy fsm;
    [SerializeField] protected Zone guardZone;
    [SerializeField] protected HitBox[] hitBoxes;

    [Header("Turning")]
    [SerializeField] protected bool enableTurn = true;
    [SerializeField] protected float turnTime = 0;

    [Header("Patrolling")]
    [SerializeField][Path(true)] protected Route patrolRoute;
    [SerializeField] protected RangedWeaponConfiguration patrolFiringConfiguration;
    
    public AttributeSet statusModifiers = new AttributeSet();

    [HideInInspector] public PlayerCharacter currentTarget;
    private int numEnabledHitBoxes = 0;

    protected bool isTurning = false;

    public Vector3 lastCheckPointTransform;
    public bool isDashing = false;
    
    //Physics related --- Slow motion implementation
    protected Rigidbody2D rb2d;
    private float defaultDrag;
    private float defaultMass;

    private Vector3 turnDirection = Vector3.zero;
    private float t_turn = 0;


    public Event<HitBox> OnEnableHitBox { get; } = new Event<HitBox>();

    public int GetTypeID()
    {
        return typeID;
    }

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

    public bool IsTurning
    {
        get
        {
            return isTurning;
        }
    }


    public void Turn()
    {
        if (currentTarget)
            Turn(currentTarget.transform.position - transform.position);
    }

    public void Turn(Vector2 direction)
    {
        Turn((Vector3)direction);
    }

    public virtual void Turn(Vector3 direction)
    {
        if (enableTurn)
        {
            if (turnTime <= 0)
                TurnImmediately(direction);
            else
            {
                bool isInSameDirection = direction.x * transform.localScale.x > 0;

                if (isInSameDirection)
                    StopTurning();
                else if (!isTurning)
                {
                    turnDirection = direction;
                    isTurning = true;
                    t_turn = turnTime;
                }
            }
        }
    }

    public void TurnImmediately()
    {
        TurnImmediately(currentTarget.transform.position - transform.position);
    }

    public void TurnImmediately(Vector2 direction)
    {
        TurnImmediately((Vector3)direction);
    }

    public virtual void TurnImmediately(Vector3 direction)
    {
        if (enableTurn)
        {
            StopTurning();


            GameUtility.Turn(this, direction);
        }
    }

    public void StopTurning()
    {
        isTurning = false;
        t_turn = 0;
    }


    public void EnableHitBox(int index)
    {
        HitBox hitBox = hitBoxes[index];


        OnEnableHitBox.Invoke(hitBox);


        hitBox.gameObject.SetActive(true);


        ++numEnabledHitBoxes;
    }

    public void DisableHitBox(int index)
    {
        hitBoxes[index].gameObject.SetActive(false);
    }

    public void DisableAllHitBoxes()
    {
        for (int i = 0; i < hitBoxes.Length; ++i)
            DisableHitBox(i);
    }

    public void SwitchHitBox(int previousIndex, int currentIndex)
    {
        DisableHitBox(previousIndex);
        EnableHitBox(currentIndex);
    }


    public virtual void Reset()
    {
        IsInvulnerable = false;
        isEvading = false;
        isTurning = false;
        
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];
        
        fsm?.Reboot();

        DisableAllHitBoxes();
    }


    public override void Dead()
    {
        PlayerCharacter.Singleton.AddOverLoadEnergy(data.Attributes[AttributeType.OspReward_c0]);
        PlayerCharacter.Singleton.AddKillCount(1);


        gameObject.SetActive(false);
    }


    protected virtual void Awake()
    {
        data = DataTableManager.singleton.GetEnemyData(typeID);
        rb2d = GetComponent<Rigidbody2D>();

        defaultDrag = rb2d.drag;
        defaultMass = rb2d.mass;


        statistics = new StatisticSystem(data.Attributes, statusModifiers);
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];

        
        if (fsm) 
            fsm = fsm.Initialize(this);
    }

    protected virtual void OnEnable()
    {
        if (fsm)
            fsm.Reboot();
    }

    protected virtual void Update()
    {
        float scaledDt = Time.deltaTime * TimeManager.Instance.TimeFactor;

        
        if (isTurning)
        {
            if (turnDirection.x * transform.localScale.x > 0)
                StopTurning();


            if (t_turn <= 0)
                TurnImmediately(turnDirection);
            else
                t_turn -= scaledDt;
        }
    }

    protected virtual void FixedUpdate()
    {
        rb2d.drag = defaultDrag / TimeManager.Instance.TimeFactor;
        rb2d.mass = defaultMass / TimeManager.Instance.TimeFactor;


        if (GetComponent<Animator>())
            GetComponent<Animator>().speed = TimeManager.Instance.TimeFactor;


        fsm?.Update();
    }


}
