using UnityEngine;


public enum OrientationType
{
    Omnidirectional,
    UpwardOnly,
    Horizontal,
}


public abstract class ESAttack<T> : EnemyState<T> where T : Enemy
{
    public enum Phase
    {
        BeforeAttack,
        Attack,
        AfterAttack,
    }


    [Header("Before Attack")]
    [SerializeField] [Min(0)] protected float attackPoint = 0;
    [SerializeField] protected string animation_beforeAttack = "";

    [Header("Attack")]
    [SerializeField] protected int hitDataID = -1;
    [SerializeField] protected int hitBox = -1;
    [SerializeField] [Min(0)] protected float attackDuration = 0;
    [SerializeField] protected string animation_attack = "";

    [Header("After Attack")]
    [SerializeField] [Min(0)] protected float attackBackswing = 0;
    [SerializeField] protected string animation_afterAttack = "";
    [SerializeField] protected string state_afterAttack = "Alert";

    protected HitData hitData;
    protected Animator animator;

    protected Phase phase;
    protected float t_phase;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        if (hitDataID >= 0)
            hitData = DataTableManager.singleton.GetHitData(hitDataID);

        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        phase = attackPoint > 0 ? Phase.BeforeAttack : Phase.Attack;
        t_phase = 0;
    }

    public override string Update()
    {
        if (phase == Phase.BeforeAttack)
        {
            string nextStateName = BeforeAttack();

            if (nextStateName != "")
                return nextStateName;
        }

        if (phase == Phase.Attack)
        {
            string nextStateName = Attack();

            if (nextStateName != "")
                return nextStateName;
        }

        if (phase == Phase.AfterAttack)
        {
            string nextStateName = AfterAttack();

            if (nextStateName != "")
                return nextStateName;
        }


        t_phase += TimeManager.Instance.ScaledDeltaTime;


        return Name;
    }

    public override void OnStateQuit(State nextState)
    {
        if (hitBox >= 0)
            enemy.DisableHitBox(hitBox);
    }


    protected virtual void InitializeHitBox(HitBox hitBox)
    {
        if (hitDataID >= 0)
            hitBox.LoadHitData(hitData);
    }


    protected virtual string BeforeAttack()
    {
        if (t_phase == 0)
        {
            if (animation_beforeAttack != "")
                animator.Play(animation_beforeAttack);
        }

        if (t_phase >= attackPoint)
        {
            phase = Phase.Attack;

            t_phase = 0;
        }


        return "";
    }

    protected virtual string Attack()
    {
        if (t_phase == 0)
        {
            enemy.OnEnableHitBox.AddListener(InitializeHitBox);

            if (hitBox >= 0)
                enemy.EnableHitBox(hitBox);


            if (animation_attack != "")
                animator.Play(animation_attack);
        }

        if (t_phase >= attackDuration)
        {
            phase = Phase.AfterAttack;

            t_phase = 0;
        }


        return "";
    }

    protected virtual string AfterAttack()
    {
        if (t_phase == 0)
        {
            enemy.OnEnableHitBox.RemoveListener(InitializeHitBox);

            if (hitBox >= 0)
                enemy.DisableHitBox(hitBox);


            if (animation_afterAttack != "")
                animator.Play(animation_afterAttack);
        }

        if (t_phase >= attackBackswing)
            return state_afterAttack;


        return "";
    }
}
