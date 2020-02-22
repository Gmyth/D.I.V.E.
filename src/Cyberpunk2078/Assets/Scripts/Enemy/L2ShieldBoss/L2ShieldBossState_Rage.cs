using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Rage", menuName = "Enemy State/Shield Boss/Rage")]
public class L2ShieldBossState_Rage : ESTimed<L2ShieldBoss>
{
    [SerializeField] private float knockbackForce = 20;
    [SerializeField] private float knockbackDuration = 0.5f;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        Vector3 d = enemy.currentTarget.transform.position - enemy.transform.position;
        enemy.currentTarget.Knockback((d.x > 0 ? Vector3.right : Vector3.left) + Vector3.up, knockbackForce, knockbackDuration);


        enemy.IsInvulnerable = true;
    }
}
