using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Throw", menuName = "Enemy State/Shield Boss/Throw")]
public class L2ShieldBossState_Throw : ESAttack<L2ShieldBoss>
{
    [Header("Projectile")]
    [SerializeField] private int projectileObjectID = 19;


    protected override string Attack()
    {
        base.Attack();


        enemy.DisableHitBox(2);

        enemy.Throw(projectileObjectID);

        AudioManager.Singleton.PlayOnce("Boss_toss");
        return "";
    }
}
