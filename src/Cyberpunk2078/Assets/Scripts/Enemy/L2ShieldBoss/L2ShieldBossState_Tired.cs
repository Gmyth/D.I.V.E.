using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Tired", menuName = "Enemy State/Level 2/Shield Boss/Tired")]
public class L2ShieldBossState_Tired : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField][Min(0)] private float duration;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_recovery = -1;
    [SerializeField] private int stateIndex_onHit = -1;

    private float t = 0;
    private bool hasHit = false;


    public override int Update()
    {
        if (hasHit)
            return stateIndex_onHit;


        if (Time.time >= t)
            return stateIndex_recovery;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        enemy.OnHit.AddListener(OnHit);

        t = Time.time + duration;
        hasHit = false;
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnHit.RemoveListener(OnHit);
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
