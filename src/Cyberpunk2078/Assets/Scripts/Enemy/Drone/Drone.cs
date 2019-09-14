using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Seeker))]
public class Drone : Enemy, IPatroller
{
    [SerializeField] private FSMEnemy fsm;

    [Header("Patrolling")]
    [SerializeField] private Vector3[] patrolPoints;
    public RangedWeaponConfiguration patrolFiringConfiguration;


    int IPatroller.NumPatrolPoints
    {
        get
        {
            return patrolPoints.Length;
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
        return patrolPoints[index];
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(gameObject.name, "Take {0} damage.", rawDamage));
        return rawDamage;
    }


    private void Start()
    {
        fsm.Initialize(this);
        fsm.Boot();
    }

    private void FixedUpdate()
    {
        fsm.Update();
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints.Length > 1)
        {
            foreach (Vector3 patrolPoint in patrolPoints)
                Gizmos.DrawSphere(patrolPoint, 0.1f);

            for (int i = 0; i < patrolPoints.Length - 1; ++i)
                LogUtility.DrawGizmoArrow(patrolPoints[0], patrolPoints[1]);
        }
    }
#endif
}
