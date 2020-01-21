using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Tired", menuName = "Enemy State/Level 2/Shield Boss/Tired")]
public class L2ShieldBossState_Tired : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField][Min(0)] private float duration;

    [Header("Connected States")]
    [SerializeField] private string state_onRecovery = "";
    [SerializeField] private string state_onHit = "";

    private Animator animator;

    private float t = 0;
    private bool hasHit = false;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        enemy.OnHit.AddListener(OnHit);


        t = Time.time + duration;
        hasHit = false;


        animator.Play("L2ShieldBoss_Tired");
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnHit.RemoveListener(OnHit);
    }

    public override string Update()
    {
        if (hasHit)
            return state_onHit;


        if (Time.time >= t)
            return state_onRecovery;


        return Name;
    }


    private void OnHit(Hit hit)
    {
        enemy.isInvulnerable = false;

        enemy.ApplyDamage(1);
        enemy.statusModifiers.Modify(AttributeType.MaxFatigue_m0, 1);

        enemy.isInvulnerable = true;


        hasHit = true;
    }
}
