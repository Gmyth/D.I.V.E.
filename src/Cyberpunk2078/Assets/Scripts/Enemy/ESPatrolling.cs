using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Patrolling", menuName = "Enemy State/Patrolling")]
public class ESPatrolling : EnemyState
{
    [Header("Configuration")]
    [SerializeField] private Vector3[] patrolPoints;
    [SerializeField] private float speed = 5;
    [SerializeField] private float delay = 0;

    [Header("Connected States")]
    [SerializeField] private int index_ESIdle = -1;
    [SerializeField] private int index_ESAlert = -1;

    private Rigidbody2D rigidbody;
    private Seeker seeker;

    private int indexTargetPatrolPoint = 0;
    private Path currentPath = null;
    private int indexWayPoint = 0;
    private float t0 = 0;


    public override void Initialize(int index, Dummy dummy)
    {
        base.Initialize(index, dummy);

        rigidbody = dummy.GetComponent<Rigidbody2D>();
        seeker = dummy.GetComponent<Seeker>();
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

        // TODO: Player detection

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
