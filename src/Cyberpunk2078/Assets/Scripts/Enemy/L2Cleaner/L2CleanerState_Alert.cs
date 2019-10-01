using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2CleanerState_Alert", menuName = "Enemy State/Level 2/Cleaner/Alert")]
public class L2CleanerState_Alert : EnemyState<L2Cleaner>
{
    [SerializeField] private float waitTime;

    [Header("Connected States")]
    [SerializeField] private int[] stateIndex_attacks;
    [SerializeField] private int stateIndex_targetLoss = -1;

    private float t;


    public override int Update()
    {
        if (!IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return stateIndex_targetLoss;


        if (Time.time >= t)
        {
            return stateIndex_attacks[0];
        }


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        t = Time.time + waitTime;
    }
}
