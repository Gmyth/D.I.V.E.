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

        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        Destroy(gameObject, 0.5f);
    }


    protected override void Start()
    {
        base.Start();

        Health = HealthCap;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (patrolPoints.Length > 1)
        {
            foreach (Vector3 patrolPoint in patrolPoints)
                Gizmos.DrawSphere(patrolPoint, 0.1f);

            for (int i = 0; i < patrolPoints.Length - 1; ++i)
                LogUtility.DrawGizmoArrow(patrolPoints[i], patrolPoints[i + 1]);
        }
    }
#endif
}
