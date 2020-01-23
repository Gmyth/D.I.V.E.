using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_DiveBomb", menuName = "Enemy State/Level 2/Shield Boss/Dive Bomb")]
public class L2ShieldBossState_DiveBomb : ESChargedAttack<L2ShieldBoss>
{
    private enum State : int
    {
        InValid = 0,
        Charging = 1,
        Raising = 2,
        Falling = 3,
        Waiting = 4,
    }


    [Header("Configuration")]
    [SerializeField] [Min(0)] private float jumpForce;
    [SerializeField] [Min(0)] private float waitTime;

    private Rigidbody2D rigidbody;

    State state = 0;
    bool isTired = false;
    float t_finishWait = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(global::State previousState)
    {
        base.OnStateEnter(previousState);


        state = State.Charging;
        isTired = enemy[StatisticType.Fatigue] >= enemy[StatisticType.MaxFatigue];

        t_finishWait = 0;
    }


    protected override string Attack(float currentTime)
    {
        switch (state)
        {
            case State.Charging:
                rigidbody.AddForce(Vector2.up * jumpForce);
                animator.Play("L2ShieldBoss_DiveBomb");
                ++state;
                break;


            case State.Raising:
                if (rigidbody.velocity.y <= 0)
                {
                    rigidbody.AddForce(-Vector2.up * jumpForce);
                    ++state;
                }
                break;


            case State.Falling:
                if (enemy.IsOnGround())
                {
                    PlayerCharacter playerCharacter = PlayerCharacter.Singleton;

                    if (playerCharacter.IsOnGround(0.12f, -0.7f, 0.6f))
                    {
                        Hit hit = new Hit();
                        hit.source = enemy;
                        hit.damage = damage;
                        hit.knockback = knockback;


                        GameUtility.ApplyDamage(hit, playerCharacter);
                    }


                    LinearMovement shockWave = ObjectRecycler.Singleton.GetObject<LinearMovement>(16);
                    shockWave.initialPosition = enemy.transform.position + new Vector3(1, 0, 0);
                    shockWave.orientation = Vector3.right;
                    shockWave.gameObject.SetActive(true);

                    shockWave = ObjectRecycler.Singleton.GetObject<LinearMovement>(16);
                    shockWave.initialPosition = enemy.transform.position + new Vector3(-1, 0, 0);
                    shockWave.orientation = Vector3.left;
                    shockWave.gameObject.SetActive(true);


                    ++state;
                    t_finishWait = Time.time + waitTime;
                }
                break;


            case State.Waiting:
                if (Time.time >= t_finishWait)
                    return isTired ? "Tired" : state_afterAttack;
                break;
        }


        return Name;
    }
}
