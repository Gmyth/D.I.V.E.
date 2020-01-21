using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_KnockedBack", menuName = "Enemy State/Level 2/Shield Boss/Knocked Back")]
public class L2ShieldBossState_KnockedBack : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField][Min(0)] private float duration;

    [Header("Connected States")]
    [SerializeField] private string state_onRecovery;

    private Animator animator;

    private float t_recovery = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }


    public override string Update()
    {
        if (Time.time >= t_recovery)
            return state_onRecovery;


        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        t_recovery = Time.time + duration;


        enemy.statusModifiers.Set(AttributeType.Fatigue_p1, -0.5f);


        animator.Play("L2ShieldBoss_KnockBack");
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.statusModifiers.Set(AttributeType.Fatigue_p1, 0);
    }
}
