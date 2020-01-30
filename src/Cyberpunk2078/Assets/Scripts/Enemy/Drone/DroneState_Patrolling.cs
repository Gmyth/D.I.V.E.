using UnityEngine;


[CreateAssetMenuAttribute(fileName = "DroneState_Patrolling", menuName = "Enemy State/Drone/Patrolling")]
public class DroneState_Patrolling : ESPatrolling<Drone>
{
    [Header("Searching")]
    [SerializeField] private float searchingDuration = 3;

    private Animator droneAnimator;
    private Animator gunAnimator;

    private float t_finishSearching = 0;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        droneAnimator = enemy.GetComponent<Animator>();
        gunAnimator = enemy.Gun.GetComponent<Animator>();
    }

    public override string Update()
    {
        string baseUpdateResult = base.Update();


        if (baseUpdateResult == Name)
        {
            if (enemy.currentTarget)
            {
                float t = Time.time;


                if (t < t_finishSearching)
                    AdjustFacingDirection();
                else
                {
                    enemy.currentTarget = null;

                    droneAnimator.Play(Drone.animation_idle);
                }
            }
        }


        return baseUpdateResult;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_finishSearching = Time.time + searchingDuration;
    }


    protected override void Charge()
    {
        base.Charge();


        enemy.Aim(enemy.currentTarget.transform.position);


        if (!enemy.isGunCharging)
        {
            enemy.isGunCharging = true;

            gunAnimator.Play("L2Drone_Gun_Charging");
        }
    }

    protected override void Fire(RangedWeaponConfiguration firingConfiguration)
    {
        Bullet bullet = ObjectRecycler.Singleton.GetObject<Bullet>(firingConfiguration.BulletID);
        bullet.hit.source = enemy;
        bullet.isFriendly = false;


        LinearMovement bulletMovement = bullet.GetComponent<LinearMovement>();
        bulletMovement.speed = firingConfiguration.BulletSpeed;
        bulletMovement.initialPosition = firingConfiguration.Muzzle ? firingConfiguration.Muzzle.position : enemy.transform.position;
        bulletMovement.orientation = enemy.GetDeviatedBulletDirection(firingConfiguration.MinDeviationAngle, firingConfiguration.MaxDeviationAngle);
        bulletMovement.transform.right = bulletMovement.orientation;
        bulletMovement.gameObject.SetActive(true);


        enemy.isGunCharging = false;


        gunAnimator.Play("L2Drone_Gun_Idle");
    }
}
