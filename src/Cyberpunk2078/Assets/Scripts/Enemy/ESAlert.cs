using UnityEngine;


public abstract class ESAlert<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected float waitTime;

    [Header("Animation")]
    [SerializeField] protected string animation = "";

    [Header("Connected States")]
    [SerializeField] protected string[] states_attacks;
    [SerializeField] protected string state_onTargetLoss = "";

    protected Animator animator;

    protected float t_finishWait;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        animator = enemy.GetComponent<Animator>();
    }


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_finishWait = Time.time + waitTime;


        if (animation != "")
            animator.Play(animation);
    }


    public override string Update()
    {
        if (Time.time >= t_finishWait)
            return states_attacks[0];


        return Name;
    }
}
