using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Alert", menuName = "Enemy State/Level 2/Shield Boss/Alert")]
public class L2ShieldBossState_Alert : ESAlert<L2ShieldBoss>
{
    [Header("Melee")]
    [SerializeField] private float meleeRange = 2.5f;
    [SerializeField] private RandomSelector meleeBehaviorSelector;

    [Header("Near")]
    [SerializeField] private float nearRange = 5f;
    [SerializeField] private RandomSelector nearBehaviorSelector;

    [Header("Middle")]
    [SerializeField] private float midRange = 8f;
    [SerializeField] private RandomSelector midBehaviorSelector;

    [Header("Far")]
    [SerializeField] private RandomSelector farBehaviorSelector;


    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.DisableHitBox(3);
    }


    protected override string ChooseBehavior()
    {
        Vector3 d = enemy.currentTarget.transform.position - enemy.transform.position;

        float distance = Mathf.Abs(d.x);


        if (distance <= meleeRange)
            return states_attacks[meleeBehaviorSelector.Select()[0]];


        if (distance <= nearRange)
            return states_attacks[nearBehaviorSelector.Select()[0]];


        if (distance <= midRange)
            return states_attacks[midBehaviorSelector.Select()[0]];


        return states_attacks[farBehaviorSelector.Select()[0]];
    }
}
