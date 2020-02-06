using Pathfinding;
using UnityEngine;


public interface IPatroller
{
    int NumPatrolPoints { get; }
    RangedWeaponConfiguration PatrolFiringConfiguration { get; }

    Vector3 GetPatrolPoint(int index);
    float GetPatrolPointStayTime(int index);
}


public abstract class ESPatrolling<T> : EnemyState<T> where T : Enemy, IPatroller
{
    [SerializeField] private bool useAStar = false;

    [Header("Configuration")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float delay = 0;
    [SerializeField] private string idleAnimation = "";
    [SerializeField] private string patrollingAnimation = "";
    [SerializeField] private bool alwaysFacingTarget = true;

    [Header("Connected States")]
    [SerializeField] private string state_onTargetFound = "";

    private SpriteRenderer renderer;
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Seeker seeker;

    private float t_nextFire = 0;
    private int indexLastPatrolPoint = -1;

    private int indexTargetPatrolPoint = 0;
    private Path currentPath = null;
    private int indexWayPoint = 0;
    private bool isMoving = false;
    private float t_finishWaiting = 0;
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

    public override string Update()
    {
        Vector3 enemyPosition = enemy.transform.position;


        animator.speed = TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;


        if (state_onTargetFound != "")
        {
            PlayerCharacter target = FindAvailableTarget(enemyPosition, enemy[StatisticType.SightRange], enemy.GuardZone);

            if (target)
            {
                enemy.currentTarget = target;


                rigidbody.velocity = Vector2.zero;

                IsMoving = false;


                return state_onTargetFound;
            }
        }


        float t = Time.time;


        if (enemy.NumPatrolPoints > 0)
        {
            if (t >= t_finishWaiting)
            {
                if (useAStar)
                {
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


                            t_finishWaiting = t + enemy.GetPatrolPointStayTime(indexTargetPatrolPoint);


                            indexLastPatrolPoint = indexTargetPatrolPoint;
                            indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % enemy.NumPatrolPoints;


                            seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
                            currentPath = null;
                        }
                        else
                        {
                            Vector2 direction = (currentPath.vectorPath[indexWayPoint] - enemy.transform.position).normalized;

                            if (enemy.Data.Type == EnemyType.Ground)
                                direction = direction.x > 0 ? Vector2.right : Vector2.left;


                            enemy.AdjustFacing(direction);


                            rigidbody.velocity = direction * speed * TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;
                            IsMoving = true;
                        }
                    }
                }
                else
                {
                    Vector3 patrolPoint = enemy.GetPatrolPoint(indexTargetPatrolPoint);

                    switch (enemy.Data.Type)
                    {
                        case EnemyType.Ground:
                            {
                                float dx = patrolPoint.x - enemyPosition.x;

                                if (Mathf.Abs(dx) < 0.5f)
                                {
                                    rigidbody.velocity = Vector2.zero;

                                    IsMoving = false;


                                    t_finishWaiting = t + enemy.GetPatrolPointStayTime(indexTargetPatrolPoint);


                                    indexLastPatrolPoint = indexTargetPatrolPoint;
                                    indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % enemy.NumPatrolPoints;
                                }
                                else
                                {
                                    Vector3 direction = dx > 0 ? Vector2.right : Vector2.left;


                                    enemy.AdjustFacing(direction);


                                    rigidbody.velocity = speed * direction;

                                    IsMoving = true;
                                }
                            }
                            break;


                        case EnemyType.Floating:
                            {
                                Vector3 v = patrolPoint - enemyPosition;

                                if (v.magnitude < 0.5f)
                                {
                                    rigidbody.velocity = Vector2.zero;

                                    IsMoving = false;


                                    t_finishWaiting = t + enemy.GetPatrolPointStayTime(indexTargetPatrolPoint);


                                    indexLastPatrolPoint = indexTargetPatrolPoint;
                                    indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % enemy.NumPatrolPoints;
                                }
                                else
                                {
                                    Vector3 direction = v.normalized;


                                    enemy.AdjustFacing(direction);


                                    rigidbody.velocity = speed * direction;

                                    IsMoving = true;
                                }
                            }
                            break;
                    }
                }
            }
        }


        RangedWeaponConfiguration firingConfiguration = enemy.PatrolFiringConfiguration;

        if (firingConfiguration.FiringInterval > 0) // Check if the firing is enabled
        {
            if (enemy.currentTarget == null || !IsPlayerInSight(enemy.currentTarget, 10))
                enemy.currentTarget = FindAvailableTarget(enemy.transform.position, 10, enemy.GuardZone);


            if (enemy.currentTarget)
            {
                if (alwaysFacingTarget)
                    enemy.AdjustFacing();


                if (t >= t_nextFire)
                {
                    if (t_finishCharging == 0)
                        t_finishCharging = t + firingConfiguration.ChargeTime;


                    if (t >= t_finishCharging)
                    {
                        Fire(firingConfiguration);

                        t_nextFire = t + firingConfiguration.FiringInterval;
                        t_finishCharging = 0;
                    }
                    else
                        Charge();
                }
            }
        }


        return Name;
    }


    public override void OnStateEnter(State previousState)
    {
        Vector3 enemyPosition = enemy.transform.position;


        indexTargetPatrolPoint = 0;


        if (enemy.NumPatrolPoints > 1)
        {
            if (indexLastPatrolPoint >= 0)
                indexTargetPatrolPoint = indexLastPatrolPoint;
            else
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
            }


            if (useAStar)
            {
                seeker.pathCallback = StartPatrolling;

                seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
            }
        }


        currentPath = null;


        t_finishWaiting = 0;
        t_finishCharging = 0;


        isMoving = false;

        if (idleAnimation != "")
            animator.Play(idleAnimation);
    }


    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        rigidbody.velocity = Vector2.zero;


        indexLastPatrolPoint = indexTargetPatrolPoint;


        seeker.CancelCurrentPathRequest();
    }


    public override void OnMachineBoot()
    {
        base.OnMachineBoot();

        t_nextFire = 0;
        indexLastPatrolPoint = -1;
    }


    protected virtual void Charge()
    {
    }

    protected virtual void Fire(RangedWeaponConfiguration firingConfiguration)
    {
        Bullet bullet = ObjectRecycler.Singleton.GetObject<Bullet>(firingConfiguration.BulletID);
        bullet.hit.source = enemy;
        bullet.isFriendly = false;


        LinearMovement bulletMovement = bullet.GetComponent<LinearMovement>();
        bulletMovement.speed = firingConfiguration.BulletSpeed;
        bulletMovement.initialPosition = firingConfiguration.Muzzle ? firingConfiguration.Muzzle.position : enemy.transform.position;
        bulletMovement.orientation = (enemy.currentTarget.transform.position - bulletMovement.initialPosition).normalized;
        bulletMovement.GetComponent<Bullet>().isFriendly = false;
        bulletMovement.transform.right = bulletMovement.orientation;
        bulletMovement.gameObject.SetActive(true);
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
