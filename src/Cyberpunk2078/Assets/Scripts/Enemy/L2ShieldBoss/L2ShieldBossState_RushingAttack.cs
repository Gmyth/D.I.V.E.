using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_RushingAttack", menuName = "Enemy State/Shield Boss/Rushing Attack")]
public class L2ShieldBossState_RushingAttack : ESAttack<L2ShieldBoss>
{
    [Header("")]
    [SerializeField] private float rushForce = 1000;

    private Rigidbody2D rigidbody;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        rigidbody.velocity = Vector2.zero;
    }


    protected override string Attack()
    {
        rigidbody.AddForce(rushForce * (enemy.transform.localScale.x > 0 ? Vector2.right : Vector2.left));

        AudioManager.Singleton.PlayOnce("Boss_swing");

        return base.Attack();
    }
}
