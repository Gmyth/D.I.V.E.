using UnityEngine;


public abstract class ESAlert<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected float waitTime;

    [Header("Animation")]
    [SerializeField] protected string animation = "";

    [Header("Connected States")]
    [SerializeField] protected string[] states_attacks;
    [SerializeField] protected string state_onTargetLoss = "";

    protected float t_wait;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_wait = 0;


        if (animation != "")
            enemyAnimator.Play(animation);
    }


    public override string Update()
    {
        enemy.Turn();


        if (!enemy.IsTurning)
            t_wait += TimeManager.Instance.ScaledDeltaTime;


        if (t_wait < waitTime)
            return Name;


        return ChooseBehavior();
    }


    protected virtual string ChooseBehavior()
    {
        return states_attacks[0];
    }
}
