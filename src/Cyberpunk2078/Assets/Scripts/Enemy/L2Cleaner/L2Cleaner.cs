using UnityEngine;


public class L2Cleaner : Enemy, IPatroller
{
//    [Header("Patrolling")]
//    [SerializeField] private Vector3[] patrolPoints;
//    public RangedWeaponConfiguration patrolFiringConfiguration;


    public override float ApplyDamage(int instanceId, float rawDamage, bool overWrite = false)
    {
        float damage = rawDamage;

        statistics[StatisticType.Hp] -= damage;

        if (statistics[StatisticType.Hp] <= 0)
            Dead();


        return damage;
    }

    public override void Dead()
    {
        var Boom = ObjectRecycler.Singleton.GetObject<SingleEffect>(3);
        Boom.transform.position = transform.position;
        Boom.gameObject.SetActive(true);
        Boom.transform.localScale = Vector3.one;

        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        Player.CurrentPlayer.AddOverLoadEnergy(enemyData.Attributes[AttributeType.Osp_c0]);

        for(int i = 0; i < 5; i++){
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
            obj.Active();
        }
        
        gameObject.SetActive(false);
        CheckPointManager.Instance.EnterResetPool(gameObject);
        //Destroy(gameObject);
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
}
