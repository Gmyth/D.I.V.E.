using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class L2ShieldBoss : Enemy
{
    public override float ApplyDamage(float rawDamage)
    {
        if (isInvulnerable)
        {
            ApplyFatigue(rawDamage);
            return 0;
        }


        if (statistics.Modify(StatisticType.Hp, -rawDamage) <= 0)
            Dead();


        return rawDamage;
    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public float ApplyFatigue(float rawFatigue)
    {
        if (fsm.CurrentStateIndex == 4)
            return 0;


        float fatigue = rawFatigue * (1 + statistics.Sum(AttributeType.Fatigue_p0)) * (1 + statistics.Sum(AttributeType.Fatigue_p1));


        if (statistics.Modify(StatisticType.Fatigue, fatigue) >= statistics[StatisticType.MaxFatigue])
        {
            statistics[StatisticType.Fatigue] = 0;

            fsm.CurrentStateIndex = 4;
        }


        Debug.LogWarningFormat("[L2ShieldBoss] Fatigue: {0} / {1}", statistics[StatisticType.Fatigue], statistics[StatisticType.MaxFatigue]);


        return fatigue;
    }


    protected override void Start()
    {
        base.Start();


        currentTarget = PlayerCharacter.Singleton;

        isInvulnerable = true;
    }
}
