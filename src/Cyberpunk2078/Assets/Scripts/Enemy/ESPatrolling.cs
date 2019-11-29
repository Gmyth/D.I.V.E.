using Pathfinding;
using UnityEngine;


public interface IPatroller
{
    int NumPatrolPoints { get; }
    RangedWeaponConfiguration PatrolFiringConfiguration { get; }

    Vector3 GetPatrolPoint(int index);
}


public abstract class ESPatrolling<T> : EnemyState<T> where T : Enemy, IPatroller
{
    [Header("Configuration")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float delay = 0;
    [SerializeField] private string idleAnimation = "";
    [SerializeField] private string patrollingAnimation = "";
    [SerializeField] private bool alwaysFacingTarget = true;

    [Header("Connected States")]
    [SerializeField] private int index_ESIdle = -1;
    [SerializeField] private int index_ESAlert = -1;

    private SpriteRenderer renderer;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Seeker seeker;

    private float t_nextFire = 0;

    private int indexTargetPatrolPoint = 0;
    private Path currentPath = null;
    private int indexWayPoint = 0;
    private bool isMoving = false;
    private float t_finishCharging = 0;


    private bool IsMoving
    {
        get
        {
            return isMoving;
        }

        set
        {
            if (value != isMoving)
            {
                if (value)
                {
                    if (patrollingAnimation != "")
                        animator.Play(patrollingAnimation);
                }
                else if (idleAnimation != "")
                    animator.Play(idleAnimation);
            }
        }
    }


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        renderer = enemy.GetComponent<SpriteRenderer>();
        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
        seeker = enemy.GetComponent<Seeker>();
    }

    public override int Update()
    {
        if(!enemy)
            return Index;


        Vector3 enemyPosition = enemy.transform.position;

        if (index_ESAlert >= 0)
        {
            enemy.currentTarget = FindAvailableTarget(enemyPosition, enemy[StatisticType.SightRange], enemy.GuardZone);

            if (enemy.currentTarget)
            {
                rigidbody.velocity = Vector2.zero;
                IsMoving = false;

                return index_ESAlert;
            }
        }


        if (currentPath != null)
        {
            Vector3 wayPoint = currentPath.vectorPath[indexWayPoint];

            float distance = enemy.Data.Type == EnemyType.Ground ? Mathf.Abs(wayPoint.x - enemyPosition.x) : Vector2.Distance(wayPoint, enemyPosition);

            if (distance < 0.5f)
                ++indexWayPoint;


            if (indexWayPoint >= currentPath.vectorPath.Count)
            {
                rigidbody.velocity = Vector2.zero;
                IsMoving = false;

                indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % enemy.NumPatrolPoints;

                seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
                currentPath = null;
            }
            else
            {
                Vector2 direction = (currentPath.vectorPath[indexWayPoint] - enemy.transform.position).normalized;

                if (enemy.Data.Type == EnemyType.Ground)
                    direction = direction.x > 0 ? Vector2.right : Vector2.left;


                AdjustFacingDirection(direction);


                rigidbody.velocity = direction * speed;


                IsMoving = true;
            }
        }


        float now = Time.time;


        RangedWeaponConfiguration firingConfiguration = enemy.PatrolFiringConfiguration;

        if (firingConfiguration.FiringInterval > 0) // Check if the firing is enabled
        {
            if (enemy.currentTarget == null || !IsPlayerInSight(enemy.currentTarget, 10))
                enemy.currentTarget = FindAvailableTarget(enemy.transform.position, 10, enemy.GuardZone);


            if (enemy.currentTarget)
            {
                if (alwaysFacingTarget)
                    AdjustFacingDirection((enemy.currentTarget.transform.position - enemy.transform.position).x > 0 ? Vector3.left : Vector3.right);


                if (now >= t_nextFire)
                {
                    if (t_finishCharging == 0)
                        t_finishCharging = now + firingConfiguration.ChargeTime;


                    if (now >= t_finishCharging)
                    {
                        Fire(firingConfiguration);

                        t_nextFire = now + firingConfiguration.FiringInterval;
                        t_finishCharging = 0;
                    }
                    else
                        Charge();
                }
            }
        }


        return Index;
    }


    public override void OnStateEnter(State previousState)
    {
        Vector3 enemyPosition = enemy.transform.position;


        indexTargetPatrolPoint = 0;


        if (enemy.NumPatrolPoints > 1)
        {
            float m = Vector3.Distance(enemyPosition, enemy.GetPatrolPoint(indexTargetPatrolPoint));

            for (int i = 1; i < enemy.NumPatrolPoints; ++i)
            {
                float d = Vector3.Distance(enemyPosition, enemy.GetPatrolPoint(i));

                if (d < m)
                {
                    indexTargetPatrolPoint = i;
                    m = d;
                }
            }


            seeker.pathCallback = StartPatrolling;

            seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
        }


        currentPath = null;


        t_finishCharging = 0;


        isMoving = false;

        if (idleAnimation != "")
            animator.Play(idleAnimation);
    }


    public override void OnStateQuit(State nextState)
    {
        seeker.CancelCurrentPathRequest();
    }


    public override void OnMachineBoot()
    {
        base.OnMachineBoot();

        t_nextFire = 0;
    }


    protected virtual void Charge()
    {
    }

    protected virtual void Fire(RangedWeaponConfiguration firingConfiguration)
    {
        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(firingConfiguration.BulletID);
        bullet.speed = firingConfiguration.BulletSpeed;
        bullet.initialPosition = firingConfiguration.Muzzle ? firingConfiguration.Muzzle.position : enemy.transform.position;
        bullet.orientation = (enemy.currentTarget.transform.position - bullet.initialPosition).normalized;

        bullet.GetComponent<Bullet>().isFriendly = false;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);
    }


    private void StartPatrolling(Path path)
    {
        if (!path.error)
        {
            currentPath = path;
            indexWayPoint = 0;
        }
        else
            currentPath = null;
    }
}
