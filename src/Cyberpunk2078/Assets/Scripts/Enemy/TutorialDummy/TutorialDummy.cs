using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : Enemy
{

    public float Health;
    public float HealthCap = 1;

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
        Boom.transform.localScale = Vector3.one;
        Boom.gameObject.SetActive(true);


        EnemyData enemyData = DataTableManager.singleton.GetEnemyData(typeID);
        PlayerCharacter.Singleton.AddOverLoadEnergy(enemyData.Attributes[AttributeType.OspReward_c0]);
        PlayerCharacter.Singleton.AddKillCount(1);

        for (int i = 0; i < 5; i++)
        {
            var obj = ObjectRecycler.Singleton.GetObject<SoulBall>(5);
            obj.transform.position = transform.position;
            obj.gameObject.SetActive(true);
        }

        //gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        gameObject.SetActive(false);
        //Destroy(gameObject, 0.5f);
        CheckPointManager.Instance.RegisterEnemey(gameObject);
    }

    protected override void Awake()
    {
        base.Awake();

        Health = HealthCap;
    }
}
