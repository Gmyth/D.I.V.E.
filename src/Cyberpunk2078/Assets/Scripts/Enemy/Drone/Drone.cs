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


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Take {0} damage.", rawDamage));


        Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0);

        if (Health <= 0)
            Dead();


        return rawDamage;
    }

    public override void Dead()
    {
        var Boom = ObjectRecycler.Singleton.GetObject<SingleEffect>(3);
        Boom.transform.position = transform.position;
        Boom.gameObject.SetActive(true);
        Boom.transform.localScale = Vector3.one;
        
        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        Player.CurrentPlayer.AddOverLoadEnergy(enemyData.Attributes[AttributeType.OspReward_c0]);

        
        for(int i = 0; i < 5; i++){
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
            obj.Active();
        }
        
        
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
