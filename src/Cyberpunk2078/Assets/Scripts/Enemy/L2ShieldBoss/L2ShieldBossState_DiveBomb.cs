using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_DiveBomb", menuName = "Enemy State/Level 2/Shield Boss/Dive Bomb")]
public class L2ShieldBossState_DiveBomb : ESChargedAttack<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] private float jumpForce;

    private Rigidbody2D rigidbody;
    bool bJump;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();


        bJump = true;
    }


    protected override string Attack(float currentTime)
    {
        if (bJump)
        {
            rigidbody.AddForce(Vector2.up * jumpForce);

            bJump = false;
        }
        else if (enemy.IsOnGround())
        {
            // TODO: Deal damages

            return "Alert";
        }


        return Name;
    }
}
