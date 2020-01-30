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
                    AdjustFacingDirection((enemy.currentTarget.transform.position - enemy.transform.position).x > 0 ? Vector3.right : Vector3.left);
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
        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(firingConfiguration.BulletID);
        bullet.speed = firingConfiguration.BulletSpeed;
        bullet.initialPosition = firingConfiguration.Muzzle ? firingConfiguration.Muzzle.position : enemy.transform.position;
        bullet.orientation = enemy.GetDeviatedBulletDirection(firingConfiguration.MinDeviationAngle, firingConfiguration.MaxDeviationAngle);

        bullet.GetComponent<Bullet>().isFriendly = false;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);


        enemy.isGunCharging = false;


        gunAnimator.Play("L2Drone_Gun_Idle");

        AudioManager.Instance.PlayOnce("LaserBullet");
    }
}
