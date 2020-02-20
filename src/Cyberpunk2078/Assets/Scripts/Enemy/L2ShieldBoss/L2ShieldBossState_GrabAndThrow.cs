using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_GrabAndThrow", menuName = "Enemy State/Shield Boss/Grab and Throw")]
public class L2ShieldBossState_GrabAndThrow : ESAttack<L2ShieldBoss>
{
    [Header("Wait")]
    [SerializeField] private string animation_waitting = "L2ShieldBoss_Idle";

    [Header("Grab")]
    [SerializeField] private float chargeSpeed = 4f;
    [SerializeField] private float extraStunTime = 0.1f;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.OnAttack.AddListener(Grab);

        enemy.EnableHitBox(hitBox);
    }


    protected override string BeforeAttack()
    {
        PlayerCharacter player = enemy.currentTarget;
        Vector3 d = player.transform.position - enemy.transform.position;


        if (Mathf.Abs(d.x) < 0.5f || player.IsOnGround())
        {
            enemyAnimator.Play(animation_waitting);

            enemyRigidbody.velocity = Vector2.zero;
        }
        else
        {
            enemyAnimator.Play(animation_beforeAttack);

            enemyRigidbody.velocity = chargeSpeed * (d.x > 0 ? Vector2.right : Vector2.left);
        }


        return "";
    }

    protected override string Attack()
    {
        PlayerCharacter player = enemy.currentTarget;
        Vector3 enemyPosition = enemy.transform.position;
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();


        if (t_phase == 0)
        {
            enemy.Turn((enemyPosition.x < enemy.GuardZone.center.x ? enemy.RightThrowPoint : enemy.LeftThrowPoint) - enemyPosition);


            player.Knockback(Vector3.zero, 0, attackDuration + extraStunTime);

            player.transform.parent.parent = enemy.HandAnchor;
            player.transform.parent.localPosition = Vector3.zero;

            playerRigidbody.gravityScale = 0;


            enemyAnimator.Play(animation_attack);
        }


        if (t_phase >= attackDuration)
        {
            player.transform.parent.parent = null;

            playerRigidbody.gravityScale = player.DefaultGravity;
            playerRigidbody.velocity = MathUtility.GetInitialVelocityForParabolaMovement(player.transform.position, enemyPosition.x < enemy.GuardZone.center.x ? enemy.RightThrowPoint : enemy.LeftThrowPoint, attackPoint, -Physics2D.gravity.y * playerRigidbody.gravityScale);


            ++phase;
            t_phase = 0;
        }


        return "";
    }


    private void Grab(Hit hit, Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            PlayerCharacter player = collider.GetComponent<PlayerCharacter>();

            if (player == enemy.currentTarget)
            {
                ++phase;
                t_phase = 0;


                enemyRigidbody.velocity = Vector2.zero;

                enemy.OnAttack.RemoveListener(Grab);

                enemy.DisableHitBox(hitBox);
            }
        }
    }
}
