using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Patrolling", menuName = "Enemy State/Patrolling")]
public class ESPatrolling : DroneState
{
    [Header("Configuration")]
    [SerializeField] private Vector3[] patrolPoints;
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


    public override void Initialize(int index, Enemy dummy)
    {
        base.Initialize(index, dummy);

        rigidbody = dummy.GetComponent<Rigidbody2D>();
        seeker = dummy.GetComponent<Seeker>();

        t0 = 0;
        tf = 0;
    }

    public override int Update()
    {
        if (currentPath != null)
        {
            float distance = Vector2.Distance(currentPath.vectorPath[indexWayPoint], dummy.transform.position);

            if (distance < 0.2f)
                ++indexWayPoint;
            

            if (indexWayPoint >= currentPath.vectorPath.Count)
            {
                rigidbody.velocity = Vector2.zero;

                indexTargetPatrolPoint = (indexTargetPatrolPoint + 1) % patrolPoints.Length;

                seeker.StartPath(dummy.transform.position, patrolPoints[indexTargetPatrolPoint]);
                currentPath = null;
            }
            else
            {
                Vector2 direction = (currentPath.vectorPath[indexWayPoint] - dummy.transform.position).normalized;

                rigidbody.AddForce(direction * speed * Time.deltaTime);
            }
        }

        
        PlayerCharacter player = IsPlayerInSight(10);
        
        if (player && Time.time - tf >= dummy.FireInterval)
        {
            LinearMovement bullet = ObjectRecycler.Singleton.GetObject<Bullet>(dummy.BulletID).GetComponent<LinearMovement>();
            bullet.speed = dummy.BulletSpeed;
            bullet.initialPosition = dummy.transform.position;
            bullet.orientation = (player.transform.position - bullet.initialPosition).normalized;

            bullet.gameObject.SetActive(true);

            tf = Time.time;
        }


        return Index;
    }


    public override void OnStateEnter()
    {
        seeker.pathCallback = StartPatrolling;

        seeker.StartPath(dummy.transform.position, patrolPoints[indexTargetPatrolPoint]);
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
