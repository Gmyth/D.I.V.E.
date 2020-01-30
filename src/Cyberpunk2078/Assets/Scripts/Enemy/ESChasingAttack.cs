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
    [SerializeField] private string state_onTargetLoss = "";
    [SerializeField] private string state_afterAttack = "";

    private EnemyType enemyType;
    private Rigidbody2D rigidbody;
    private Animator animator;

    private bool isMoving = false;
    private float t = float.MaxValue;
    private string previousStateName;


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
                isMoving = value;


                if (value)
                {
                    if (chasingAnimation != "")
                        animator.Play(chasingAnimation);

                    OnBeginChasing();
                }
                else
                {
                    if (idleAnimation != "")
                        animator.Play(idleAnimation);

                    OnEndChasing();
                }
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

    public override string Update()
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
                        AdjustFacingDirection(d);


                        t = Time.time;


                        if (attackAnimation != "")
                            animator.Play(attackAnimation, -1, 0f);


                        Attack();
                    }
                }
                else
                {
                    Vector2 direction = d.normalized;

                    if (enemyType == EnemyType.Ground)
                        direction = d.x > 0 ? Vector2.right : Vector2.left;


                    rigidbody.velocity = direction * chasingSpeed * TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;


                    IsMoving = true;


                    AdjustFacingDirection(direction);
                }


                return Name;
            }


            return state_onTargetLoss == "" ? previousStateName : state_onTargetLoss;
        }


        if (dt < motionTime) // Stay in this state if the motion has not been done yet
            return Name;


        return state_afterAttack == "" ? previousStateName : state_afterAttack;
    }


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);
        //        PlayerCharacter player = GameObject.FindObjectOfType<PlayerCharacter>();
        //        var dir = (player.transform.position - enemy.transform.position ).normalized;
        //        
        //        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position + new Vector3(0,0.5f,0),dir.x < 0? Vector3.left : Vector3.right, 5f);
        //        if (hit.collider != null && hit.transform.CompareTag("Player"))
        //        {
        //            //hit! Hunch Trigger
        //            PlayerCharacter playerCharacter = hit.collider.GetComponent<PlayerCharacter>();
        //            if (playerCharacter.IsInFeverMode)
        //            {
        //                playerCharacter.ConsumeFever(30);
        //                TimeManager.Instance.startSlowMotion(0.5f);
        //                CameraManager.Instance.FocusAt(playerCharacter.transform,0.2f);
        //                CameraManager.Instance.FlashIn(7f,0.05f,0.15f,0.01f);
        //            }
        //        }


        isMoving = false;
        t = float.MaxValue;
        previousStateName = previousState.Name;
    }


    protected virtual void OnBeginChasing()
    {
    }

    protected virtual void OnEndChasing()
    {
    }

    protected virtual void Attack()
    {
    }
}
