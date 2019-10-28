using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class L2ShieldBoss : Enemy
{
    public override float ApplyDamage(float rawDamage)
    {
        throw new System.NotImplementedException();
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
}
