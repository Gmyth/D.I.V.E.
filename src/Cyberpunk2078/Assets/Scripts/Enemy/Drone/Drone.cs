using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Seeker))]
public class Drone : Enemy, IPatroller
{
    private static readonly string animation_idle = "L2Drone_Idle";
    private static readonly string animation_alert = "L2Drone_Alert";


    [Header("Parts")]
    [SerializeField] private SpriteRenderer gun;
    [SerializeField] private SpriteRenderer thruster;

    public float Health;
    public float HealthCap = 1;

    public bool isGunCharging = false;

    private Animator animator;

    private int animationIndex;
    private float aimDeviation = 0;


    public SpriteRenderer Gun
    {
        get
        {
            return gun;
        }
    }


    public Vector3 GetDeviatedBulletDirection(float minAngle, float maxAngle)
    {
        return ProjectileUtility.GetDeviatedBulletDirection(-gun.transform.right, minAngle, maxAngle, aimDeviation);
    }

    public void AdjustGunDirection(Vector3 targetPosition)
    {
        //if (isGunCharging)
        //    return;


        Vector3 direction = (transform.position - targetPosition).normalized;


        float angle = Vector3.Angle(gun.transform.right, direction);

        aimDeviation = Mathf.Min(1, aimDeviation + 0.01f * angle * angle);


        gun.transform.right = direction;
    }

    public void ResetGunDirection()
    {
        //if (isGunCharging)
        //    return;


        gun.transform.right = Vector3.right;
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
        Boom.gameObject.SetActive(true);
        Boom.transform.localScale = Vector3.one;
        
        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        Player.CurrentPlayer.AddOverLoadEnergy(enemyData.Attributes[AttributeType.OspReward_c0]);

        
        for(int i = 0; i < 5; i++)
        {
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
            obj.Active();
        }
        
        
        //gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        gameObject.SetActive(false);
        //Destroy(gameObject, 0.5f);
        CheckPointManager.Instance.EnterResetPool(gameObject);
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
            

            AdjustGunDirection(currentTarget.transform.position);
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
