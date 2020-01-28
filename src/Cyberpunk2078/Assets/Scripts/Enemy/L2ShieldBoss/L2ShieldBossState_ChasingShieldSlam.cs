using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_ChasingSieldSlam", menuName = "Enemy State/Level 2/Shield Boss/Chasing Shield Slam")]
public class L2ShieldBossState_ChasingShieldSlam : ESChasingAttack<L2ShieldBoss>
{
    protected override void OnBeginChasing()
    {
        base.OnBeginChasing();


        Hit hit = new Hit();
        hit.type = Hit.Type.Melee;
        hit.source = enemy;
        hit.damage = 0;
        hit.knockback = 150;

        enemy.currentHit = hit;


        enemy.EnableHitBox(2);
    }

    protected override void OnEndChasing()
    {
        base.OnEndChasing();


        enemy.DisableHitBox(2);
    }
}
