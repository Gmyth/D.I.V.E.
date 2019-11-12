using UnityEngine;


public abstract class ESChasingAttack<T> : ESAttack<T> where T : Enemy
{
    [Header("Configuration")]
    [SerializeField] private float chasingSpeed;
    [SerializeField][Min(0)] private float attackRange;
    [SerializeField][Min(0)] private float attackHeight;
    [SerializeField] private float motionTime;
    [SerializeField] private string idleAnimation = "";
    [SerializeField] private string chasingAnimation = "";
    [SerializeField] private string attackAnimation = "";

    [Header("Connected States")]
    [SerializeField] private int stateIndex_targetLoss = -1;
    [SerializeField] private int stateIndex_afterAttack = -1;


    private EnemyType enemyType;
    private Rigidbody2D rigidbody;
    private Animator animator;

    private bool isMoving = false;
    private float t = float.MaxValue;
    private int previousStateIndex;


    private bool IsMoving
    {
        get
        {
            return isMoving;
        }

        set
        {
            if (value != isMoving)
            {
                if (value)
                {
                    if (chasingAnimation != "")
                        animator.Play(chasingAnimation);
                }
                else if (idleAnimation != "")
                    animator.Play(idleAnimation);
            }
        }
    }


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        enemyType = enemy.Data.Type;
        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override int Update()
    {
        float dt = Time.time - t;


        if (dt < 0) // Check if the attack has not been made
        {
            PlayerCharacter player = IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]);

            if (player && enemy.GuardZone.Contains(player)) // Check if the target is still available
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 enemyPosition = enemy.transform.position;


                Vector3 d = playerPosition - enemyPosition;

                if (Mathf.Abs(d.x) <= attackRange) // Check if the target is horizontally close enough
                {
                    if (enemyType == EnemyType.Floating)
                        rigidbody.velocity = Vector2.zero;
                    else
                    {
                        Vector2 v = rigidbody.velocity;
                        v.x = 0;

                        rigidbody.velocity = v;
                    }


                    IsMoving = false;


                    if (playerPosition.y - enemyPosition.y <= attackHeight) // Check if the target is low enough to get hit
                    {
                        AdjustFacingDirection(d.x > 0 ? Vector2.right : Vector2.left);


                        t = Time.time;


                        if (attackAnimation != "")
                            animator.Play(attackAnimation, -1, 0f);
                    }
                }
                else
                {
                    Vector2 direction = d.normalized;

                    if (enemyType == EnemyType.Ground)
                        direction = d.x > 0 ? Vector2.right : Vector2.left;


                    rigidbody.velocity = direction * chasingSpeed;


                    IsMoving = true;


                    AdjustFacingDirection(direction);
                }


                return Index;
            }


            return stateIndex_targetLoss < 0 ? previousStateIndex : stateIndex_targetLoss;
        }


        if (dt < motionTime) // Stay in this state if the motion has not been done yet
            return Index;


        return stateIndex_afterAttack < 0 ? previousStateIndex : stateIndex_afterAttack;
    }


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        isMoving = false;
        t = float.MaxValue;
        previousStateIndex = previousState.Index;
    }
}
