using UnityEngine;


public class L2Cleaner : Enemy, IPatroller
{
//    [Header("Patrolling")]
//    [SerializeField] private Vector3[] patrolPoints;
//    public RangedWeaponConfiguration patrolFiringConfiguration;

    
    public override void Dead()
    {
        var Boom = ObjectRecycler.Singleton.GetObject<SingleEffect>(3);
        Boom.transform.position = transform.position;
        Boom.gameObject.SetActive(true);
        Boom.transform.localScale = Vector3.one;

        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        
        PlayerCharacter.Singleton.AddOverLoadEnergy(enemyData.Attributes[AttributeType.OspReward_c0]);
        PlayerCharacter.Singleton.AddKillCount(1);
        
        for (int i = 0; i < 3; i++)
        {
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
        }
        
        gameObject.SetActive(false);
        CheckPointManager.Instance?.Dead(gameObject);

        AudioManager.Singleton.PlayOnce("KillRobot");
    }


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
