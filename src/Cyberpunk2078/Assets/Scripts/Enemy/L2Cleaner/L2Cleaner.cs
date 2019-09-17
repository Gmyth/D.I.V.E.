using UnityEngine;


public class L2Cleaner : Enemy, IPatroller
{
    [Header("Patrolling")]
    [SerializeField] private Vector3[] patrolPoints;
    public RangedWeaponConfiguration patrolFiringConfiguration;


    public override float ApplyDamage(int instanceId, float rawDamage, bool overWrite = false)
    {
        throw new System.NotImplementedException();
    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }


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
}
