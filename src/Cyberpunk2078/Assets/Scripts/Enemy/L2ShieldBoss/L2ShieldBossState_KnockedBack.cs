using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_KnockedBack", menuName = "Enemy State/Level 2/Shield Boss/Knocked Back")]
public class L2ShieldBossState_KnockedBack : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField][Min(0)] private float duration;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_recovery;

    private float t = 0;


    public override int Update()
    {
        if (Time.time >= t)
            return stateIndex_recovery;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        enemy.OnHit.AddListener(OnHit);

        t = Time.time + duration;
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnHit.RemoveListener(OnHit);
    }


    private void OnHit(Hit hit)
    {
        enemy.ApplyFatigue(hit.damage);
    }
}
