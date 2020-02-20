using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserRobot : Enemy, IPatroller
{
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

    float IPatroller.GetPatrolPointStayTime(int index)
    {
        return patrolRoute.GetStayTime(index);
    }
}
