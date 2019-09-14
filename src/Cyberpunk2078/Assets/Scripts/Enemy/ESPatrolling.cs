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

    [Header("Connected States")]
    [SerializeField] private int index_ESIdle = -1;
    [SerializeField] private int index_ESAlert = -1;
    [SerializeField] private int index_ESChasing = -1;

    private Rigidbody2D rigidbody;
    private Seeker seeker;

    private int indexTargetPatrolPoint = 0;
    private Path currentPath = null;
    private int indexWayPoint = 0;
    private float t0 = 0;
    private float tf = 0;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
        seeker = enemy.GetComponent<Seeker>();

        t0 = 0;
        tf = 0;
    }

    public override int Update()
    {
        if (currentPath != null)
        {
            float distance = Vector2.Distance(currentPath.vectorPath[indexWayPoint], enemy.transform.position);

            if (distance < 0.2f)
                ++indexWayPoint;
            

            if (indexWayPoint >= currentPath.vectorPath.Count)
            {
                rigidbody.velocity = Vector2.zero;

                indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % enemy.NumPatrolPoints;

                seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
                currentPath = null;
            }
            else
            {
                Vector2 direction = (currentPath.vectorPath[indexWayPoint] - enemy.transform.position).normalized;

                rigidbody.AddForce(direction * speed * Time.deltaTime);
            }
        }


        RangedWeaponConfiguration firingConfiguration = enemy.PatrolFiringConfiguration;

        if (firingConfiguration.FiringInterval > 0)
        {
            PlayerCharacter player = IsPlayerInSight(10);

            if (player && Time.time - tf >= firingConfiguration.FiringInterval)
            {
                LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(firingConfiguration.BulletID);
                bullet.speed = firingConfiguration.BulletSpeed;
                bullet.initialPosition = enemy.transform.position;
                bullet.orientation = (player.transform.position - bullet.initialPosition).normalized;

                bullet.gameObject.SetActive(true);

                tf = Time.time;
            }
        }
        

        return Index;
    }


    public override void OnStateEnter()
    {
        seeker.pathCallback = StartPatrolling;

        seeker.StartPath(enemy.transform.position, enemy.GetPatrolPoint(indexTargetPatrolPoint));
        currentPath = null;
    }

    public override void OnStateQuit()
    {
        seeker.CancelCurrentPathRequest();
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
