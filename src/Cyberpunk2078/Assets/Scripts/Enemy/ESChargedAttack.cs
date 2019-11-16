using UnityEngine;


public abstract class ESChargedAttack<T> : ESAttack<T> where T : Enemy
{
    [SerializeField] protected float chargeTime = 1;
    [SerializeField] protected bool stopChargingOnTargetLoss = false;

    [Header("Animation")]
    [SerializeField] protected string chargeAnimation = "";

    [Header("Connected States")]
    [SerializeField] protected int stateIndex_alert = -1;

    protected Animator animator;

    protected float t;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override int Update()
    {
        float currentTime = Time.time;

        if (currentTime >= t)
            return Attack(currentTime);
        else if (stopChargingOnTargetLoss && IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return stateIndex_alert;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t = Time.time + chargeTime;


        animator.Play(chargeAnimation);
    }


    protected abstract int Attack(float currentTime);
}
