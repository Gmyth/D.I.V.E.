using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[RequireComponent(typeof(Seeker))]
public class Drone : Enemy, IPatroller
{
    [Header("")]
    [SerializeField] private float DamageCD;
    private Dictionary<int, float> damageList = new Dictionary<int, float>();

    public float Health;
    public float HealthCap = 1;


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


    public override float ApplyDamage(int instanceId, float rawDamage, bool overWrite)
    {
        float value;
        if (damageList.TryGetValue(instanceId, out value) && overWrite)
        {
            // Already hit by this attack but ok for damage again
            if (value + DamageCD < Time.time)
            {
                Debug.Log(LogUtility.MakeLogStringFormat("Enemy", "Take {0} damage.", rawDamage));
                Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0); ;
                if (Health == 0)
                {
                    // dead
                    Dead();
                }
            }
        }
        else
        {
            // new attack coming
            Debug.Log(LogUtility.MakeLogStringFormat("Enemy", "Take {0} damage.", rawDamage));
            Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0); ;
            if (Health == 0)
            {
                // dead
                Dead();
            }
        }
        
        if (!damageList.ContainsKey(instanceId)) damageList.Add(instanceId, Time.time);
        else damageList[instanceId] = Time.time;


        return rawDamage;
    }

    public override void Dead()
    {
        var Boom = ObjectRecycler.Singleton.GetObject<SingleEffect>(3);
        Boom.transform.position = transform.position;
        Boom.gameObject.SetActive(true);
        Boom.transform.localScale = Vector3.one;
        
        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        Player.CurrentPlayer.AddOverLoadEnergy(enemyData.Attributes[AttributeType.Osp_c0]);

        //gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        gameObject.SetActive(false);
        //Destroy(gameObject, 0.5f);
        CheckPointManager.Instance.EnterResetPool(gameObject);
    }


    protected override void Start()
    {
        base.Start();

        Health = HealthCap;
    }
}
