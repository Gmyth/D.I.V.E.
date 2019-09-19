using UnityEngine;


public class L2Cleaner : Enemy, IPatroller
{
    [Header("Patrolling")]
    [SerializeField] private Vector3[] patrolPoints;
    public RangedWeaponConfiguration patrolFiringConfiguration;


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

        gameObject.SetActive(false);
        Destroy(gameObject, 0.5f);
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
