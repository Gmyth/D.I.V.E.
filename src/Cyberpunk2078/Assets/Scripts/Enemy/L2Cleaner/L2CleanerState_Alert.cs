using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2CleanerState_Alert", menuName = "Enemy State/Level 2/Cleaner/Alert")]
public class L2CleanerState_Alert : EnemyState<L2Cleaner>
{
    [SerializeField] private float waitTime;
    [SerializeField] private RandomSelector behaviorSelector;

    [Header("Animation")]
    [SerializeField] private string animation;

    [Header("Connected States")]
    [SerializeField] private int[] stateIndex_attacks;
    [SerializeField] private int stateIndex_targetLoss = -1;

    private Animator animator;

    private float t;


    public override void Initialize(L2Cleaner enemy)
    {
        base.Initialize(enemy);

        animator = enemy.GetComponent<Animator>();
    }

    public override int Update()
    {
        animator.speed = TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;
        if (!enemy.GuardZone.Contains(enemy.currentTarget) || !IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return stateIndex_targetLoss;


        if (Time.time >= t)
            return stateIndex_attacks[behaviorSelector.Select()[0]];


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        t = Time.time + waitTime;

        animator.Play(animation);
    }
}
