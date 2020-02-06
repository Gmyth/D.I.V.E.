using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Tired", menuName = "Enemy State/Level 2/Shield Boss/Tired")]
public class L2ShieldBossState_Tired : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField][Min(0)] private float duration;
    [SerializeField] private string animation = "L2ShieldBoss_Tired";

    [Header("Connected States")]
    [SerializeField] private string nextState = "Alert";

    private Animator animator;

    private float t = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        t = 0;


        animator.Play(animation);
    }


    public override string Update()
    {
        t += TimeManager.Instance.ScaledDeltaTime;


        if (t >= duration)
            return nextState;


        return Name;
    }
}
