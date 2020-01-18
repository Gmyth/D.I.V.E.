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


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -rawDamage, 0, statistics[StatisticType.MaxHp]);

        if (result.currentValue <= 0)
            Dead();


        return result.previousValue - result.currentValue;
    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public float ApplyFatigue(float rawFatigue)
    {
        if (fsm.CurrentStateName == "Tired" || rawFatigue <= 0)
            return 0;


        float fatigue = rawFatigue * (1 + statistics.Sum(AttributeType.Fatigue_p0)) * (1 + statistics.Sum(AttributeType.Fatigue_p1));


        StatisticModificationResult result = statistics.Modify(StatisticType.Fatigue, fatigue, 0, statistics[StatisticType.MaxFatigue]);

        if (result.currentValue >= statistics[StatisticType.MaxFatigue])
        {
            statistics[StatisticType.Fatigue] = 0;

            fsm.CurrentStateName = "Tired";
        }


        Debug.LogWarningFormat("[L2ShieldBoss] Fatigue: {0} / {1}", result.currentValue, statistics[StatisticType.MaxFatigue]);


        return result.currentValue - result.previousValue;
    }


    protected override void Start()
    {
        base.Start();


        currentTarget = PlayerCharacter.Singleton;

        isInvulnerable = true;
    }
}
