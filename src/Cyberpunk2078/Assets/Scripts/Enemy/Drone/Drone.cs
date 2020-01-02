using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Seeker))]
public class Drone : Enemy, IPatroller
{
    public static readonly string animation_idle = "L2Drone_Idle";
    public static readonly string animation_alert = "L2Drone_Alert";


    [SerializeField] private float farRange;
    [SerializeField] private float nearRange;

    [Header("Parts")]
    [SerializeField] private SpriteRenderer gun;
    [SerializeField] private SpriteRenderer thruster;

    public float Health;
    public float HealthCap = 1;

    public bool isGunCharging = false;

    private Animator animator;

    private int animationIndex;
    private float aimDeviation = 0;


    public float FarRange
    {
        get
        {
            return farRange;
        }
    }

    public float NearRange
    {
        get
        {
            return nearRange;
        }
    }

    public SpriteRenderer Gun
    {
        get
        {
            return gun;
        }
    }


    public Vector3 AimingDirection
    {
        get
        {
            return Mathf.Sign(transform.localScale.x) * gun.transform.right;
        }

        private set
        {
            gun.transform.right = value;
        }
    }


    public Vector3 Aim(Vector3 targetPosition)
    {
        //if (isGunCharging)
        //    return;


        Vector3 direction = Mathf.Sign(transform.localScale.x) * (targetPosition - transform.position).normalized;


        float angle = Vector3.Angle(gun.transform.right, direction);

        aimDeviation = Mathf.Min(1, aimDeviation + 0.01f * angle * angle);


        AimingDirection = direction;


        return direction;
    }

    public Vector3 GetDeviatedBulletDirection(float minAngle, float maxAngle)
    {
        return ProjectileUtility.GetDeviatedBulletDirection(AimingDirection, minAngle, maxAngle, aimDeviation);
    }



    public void ResetGunDirection()
    {
        //if (isGunCharging)
        //    return;


        gun.transform.right = Vector3.right;
    }


    public void Fire(bool hasDeviation = true)
    {
        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(patrolFiringConfiguration.BulletID);
        bullet.speed = patrolFiringConfiguration.BulletSpeed;
        bullet.initialPosition = patrolFiringConfiguration.Muzzle ? patrolFiringConfiguration.Muzzle.position : transform.position;
        bullet.orientation = hasDeviation ? GetDeviatedBulletDirection(patrolFiringConfiguration.MinDeviationAngle, patrolFiringConfiguration.MaxDeviationAngle) : AimingDirection;

        bullet.GetComponent<Bullet>().isFriendly = false;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);
    }

    public void Fire(Vector3 position, bool hasDeviation = true)
    {
        Vector3 aimDirection = Aim(position);


        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(patrolFiringConfiguration.BulletID);
        bullet.speed = patrolFiringConfiguration.BulletSpeed;
        bullet.initialPosition = patrolFiringConfiguration.Muzzle ? patrolFiringConfiguration.Muzzle.position : transform.position;
        bullet.orientation = hasDeviation ? GetDeviatedBulletDirection(patrolFiringConfiguration.MinDeviationAngle, patrolFiringConfiguration.MaxDeviationAngle) : aimDirection;

        bullet.GetComponent<Bullet>().isFriendly = false;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);
    }


    int IPatroller.NumPatrolPoints
    {
        get
        {
            return patrolRoute.NumWayPoints;
        }
    }

    RangedWeaponConfiguration IPatroller.PatrolFiringConfiguration
    {
        get
        {
            return patrolFiringConfiguration;
        }
    }

    Vector3 IPatroller.GetPatrolPoint(int index)
    {
        return patrolRoute[index];
    }

    float IPatroller.GetPatrolPointStayTime(int index)
    {
        return patrolRoute.GetStayTime(index);
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Take {0} damage.", rawDamage));


        Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0);

        if (Health <= 0)
            Dead();


        return rawDamage;
    }

    public override void Dead()
    {
        var Boom = ObjectRecycler.Singleton.GetObject<SingleEffect>(3);
        Boom.transform.position = transform.position;
        Boom.transform.localScale = Vector3.one;
        Boom.gameObject.SetActive(true);


        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        PlayerCharacter.Singleton.AddOverLoadEnergy(enemyData.Attributes[AttributeType.OspReward_c0]);
        PlayerCharacter.Singleton.AddKillCount(1);

        for (int i = 0; i < 5; i++)
        {
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
        }

        //gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        gameObject.SetActive(false);
        //Destroy(gameObject, 0.5f);
        CheckPointManager.Instance.Dead(gameObject);
    }


    protected override void Start()
    {
        base.Start();


        Health = HealthCap;
        isGunCharging = false;

        animator = GetComponent<Animator>();

        animationIndex = 0;
        aimDeviation = 0;


        animator.Play(animation_idle);
    }


    private void Update()
    {
        if (currentTarget)
        {
            if (animationIndex != 1)
            {
                animator.Play(animation_alert);
                animationIndex = 1;
            }
            

            Aim(currentTarget.transform.position);
        }
        else
        {
            if (animationIndex != 0)
            {
                animator.Play(animation_idle);
                animationIndex = 0;
            }



            isGunCharging = false;


            ResetGunDirection();
        }

        aimDeviation = Mathf.Max(0, aimDeviation - Time.deltaTime);
    }
}
