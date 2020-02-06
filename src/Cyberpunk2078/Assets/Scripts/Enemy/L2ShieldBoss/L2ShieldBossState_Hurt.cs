using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Hurt", menuName = "Enemy State/Shield Boss/Hurt")]
public class L2ShieldBossState_Hurt : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] private float duration;
    [SerializeField] private string animation = "L2ShieldBoss_Hurt";

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
        base.OnStateEnter(previousState);


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
