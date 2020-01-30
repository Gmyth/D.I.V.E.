using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2CleanerState_Alert", menuName = "Enemy State/L2Cleaner/Alert")]
public class L2CleanerState_Alert : EnemyState<L2Cleaner>
{
    [SerializeField] private float waitTime;
    [SerializeField] private RandomSelector behaviorSelector;

    [Header("Animation")]
    [SerializeField] private string animation;

    [Header("Connected States")]
    [SerializeField] private string[] states_attack;
    [SerializeField] private string state_onTargetLoss = "";

    private Animator animator;

    private float t;


    public override void Initialize(L2Cleaner enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override string Update()
    {
        animator.speed = TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;
        if (!enemy.GuardZone.Contains(enemy.currentTarget) || !IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return state_onTargetLoss;


        if (Time.time >= t)
            return states_attack[behaviorSelector.Select()[0]];


        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        t = Time.time + waitTime;

        animator.Play(animation);
    }
}
