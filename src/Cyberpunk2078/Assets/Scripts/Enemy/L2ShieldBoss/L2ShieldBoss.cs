using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class L2ShieldBoss : Enemy
{
    public AttributeSet modifiers = new AttributeSet();


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
        if (statistics.Modify(StatisticType.Fatigue, rawFatigue) >= statistics[StatisticType.MaxFatigue])
        {
            statistics[StatisticType.Fatigue] = 0;

            fsm.CurrentStateIndex = 4;
        }


        return rawFatigue;
    }


    protected override void Start()
    {
        base.Start();


        currentTarget = PlayerCharacter.Singleton;
    }
}
