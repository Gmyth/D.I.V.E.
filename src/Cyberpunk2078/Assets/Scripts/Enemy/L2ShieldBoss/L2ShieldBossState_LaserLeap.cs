using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_LaserLeap", menuName = "Enemy State/Shield Boss/Leap")]
public class L2ShieldBossState_LaserLeap : ESTimed<L2ShieldBoss>
{
    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        Vector3 enemyPosition = enemy.transform.position;

        enemyRigidbody.velocity = MathUtility.GetInitialVelocityForParabolaMovement(enemyPosition, enemyPosition.x < enemy.GuardZone.center.x ? enemy.LeftThrowPoint : enemy.RightThrowPoint, duration, enemyRigidbody.gravityScale * -Physics2D.gravity.y);
    }
}
