﻿using UnityEngine;


public abstract class ESChargedAttack<T> : ESAttack<T> where T : Enemy
{
    [Header("Charge")]
    [SerializeField] protected float chargeTime = 1;
    [SerializeField] protected bool stopChargingOnTargetLoss = false;
    [SerializeField] protected string animation_charging = "";

    [Header("Connected States")]
    [SerializeField] protected string stateIndex_afterAttack = "";
    [SerializeField] protected string state_onTargetLoss = "";

    protected Animator animator;

    protected float t_chargeFinish;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override string Update()
    {
        float currentTime = Time.time;

        if (currentTime >= t_chargeFinish)
            return Attack(currentTime);
        else if (!IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return OnTargetLoss();


        return Name;
    }


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_chargeFinish = Time.time + chargeTime;


        animator.Play(animation_charging);
    }


    protected abstract string Attack(float currentTime);

    protected virtual string OnTargetLoss()
    {
        return stopChargingOnTargetLoss ? state_onTargetLoss : Name;
    }
}
