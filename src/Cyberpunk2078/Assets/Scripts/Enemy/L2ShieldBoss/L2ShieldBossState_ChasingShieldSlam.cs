using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_ChasingSieldSlam", menuName = "Enemy State/Level 2/Shield Boss/Chasing Shield Slam")]
public class L2ShieldBossState_ChasingShieldSlam : ESChasingAttack<L2ShieldBoss>
{
    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.DisableHitBox(2);
    }


    protected override void OnBeginChasing()
    {
        base.OnBeginChasing();


        enemy.EnableHitBox(2, false);
    }

    protected override void OnEndChasing()
    {
        base.OnEndChasing();


        enemy.DisableHitBox(2);
    }
}
