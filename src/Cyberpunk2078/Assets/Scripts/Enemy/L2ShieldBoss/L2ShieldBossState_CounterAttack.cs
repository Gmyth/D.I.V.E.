using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_CounterAttack", menuName = "Enemy State/Shield Boss/Counter Attack")]
public class L2ShieldBossState_CounterAttack : ESAttack<L2ShieldBoss>
{
    [SerializeField] private float motionTime;
    [SerializeField] private string animation;
    [SerializeField] private string state_afterAttack;

    private Animator animator;

    private float t_finish = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_finish = Time.time + motionTime;


        enemy.AdjustFacingImmediately();


        animator.Play(animation);
    }

    public override string Update()
    {
        float t = Time.time;


        if (t >= t_finish)
            return state_afterAttack;


        return Name;
    }
}
