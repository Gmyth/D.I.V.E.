using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : Dummy
{
    [SerializeField] private FSMEnemy fsm;

    [Header("Configuration")]
    [SerializeField] private int bulletID = 0;
    [SerializeField] private float fireInterval;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float DamageCD;
    private Dictionary<int, float> damageList = new Dictionary<int, float>();
    
    public float Health;

    public float HealthCap = 1;



    public int BulletID
    {
        get
        {
            return bulletID;
        }
    }

    public float FireInterval
    {
        get
        {
            return fireInterval;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
    }


    private void Start()
    {
        Health = HealthCap;
        fsm.Initialize(this);
        fsm.Boot();
    }

    private void FixedUpdate()
    {
        fsm.Update();
    }


    public override float ApplyDamage(int instanceId,float rawDamage, bool overWrite)
    {
        float value;
        if (damageList.TryGetValue(instanceId,out value) && overWrite)
        {
            // Already hit by this attack but ok for damage again
            if (value + DamageCD < Time.time)
            {
                Debug.Log(LogUtility.MakeLogStringFormat("Enemy", "Take {0} damage.", rawDamage));
                Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0);;
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
            damageList.Add(instanceId,Time.time);
            Debug.Log(LogUtility.MakeLogStringFormat("Enemy", "Take {0} damage.", rawDamage));
            Health = Mathf.Max(Mathf.Min(Health - rawDamage, HealthCap), 0);;
            if (Health == 0)
            {
                // dead
                Dead();
            }
        }


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
}
