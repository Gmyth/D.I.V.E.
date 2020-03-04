﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2CleanerState_Alert", menuName = "Enemy State/L2Cleaner/Alert")]
public class L2CleanerState_Alert : ESAlert<L2Cleaner>
{
    [SerializeField] private RandomSelector behaviorSelector;


    public override string Update()
    {
        enemyAnimator.speed = TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;


        if (!enemy.GuardZone.Contains(enemy.currentTarget) || !IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return state_onTargetLoss;


        if (Time.time >= t_wait)
            return states_attacks[behaviorSelector.Select()[0]];


        return Name;
    }
}
