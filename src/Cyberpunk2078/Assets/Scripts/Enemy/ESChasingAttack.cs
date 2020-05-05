using UnityEngine;


public abstract class ESChasingAttack<T> : ESAttack<T> where T : Enemy
{
    [Header("Chasing")]
    [SerializeField] private float chasingSpeed;
    [SerializeField][Min(0)] private float attackRange;
    [SerializeField][Min(0)] private float attackHeight;
    [SerializeField] private float motionTime;
    [SerializeField] private string idleAnimation = "";
    [SerializeField] private string chasingAnimation = "";
    [SerializeField] private string attackAnimation = "";
    [SerializeField] private string state_onTargetLoss = "";

    private EnemyType enemyType;
    private Rigidbody2D rigidbody;

    private bool isChasing = false;
    private float t_attack = float.MaxValue;
    private string previousStateName;


    private bool IsChasing
    {
        get
        {
            return isChasing;
        }

        set
        {
            if (value != isChasing)
            {
                isChasing = value;


                if (value)
                {
                    if (chasingAnimation != "")
                    {
                        if(enemy.GetTypeID() == 67)
                        {
                            AudioManager.Singleton.PlayEvent("Boss_walk");
                        }
                        animator.Play(chasingAnimation);
                        
                    }
                    StartChasing();
                }
                else
                {
                    if (idleAnimation != "")
                    {
                        animator.Play(idleAnimation);
                        AudioManager.Singleton.StopEvent("Boss_walk");
                    }

                    StopChasing();
                }
            }
        }
    }


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        enemyType = enemy.Data.Type;
        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override string Update()
    {
        float dt = Time.time - t_attack;


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
                    IsChasing = false;


                    if (playerPosition.y - enemyPosition.y <= attackHeight) // Check if the target is low enough to get hit
                    {
                        t_attack = Time.time;


                        string nextStateName = Attack(d);

                        if (nextStateName != "")
                            return nextStateName;
                    }
                }
                else
                {
                    IsChasing = true;


                    string nextStateName = Chase(d);

                    if (nextStateName != "")
                        return nextStateName;
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


        isChasing = false;
        t_attack = float.MaxValue;
        previousStateName = previousState.Name;
    }


    protected virtual void StartChasing()
    {
    }

    protected virtual string Chase(Vector3 direction)
    {
        Vector3 d = ((Vector2)direction).normalized;

        if (enemyType == EnemyType.Ground)
            d = d.x > 0 ? Vector2.right : Vector2.left;


        enemy.Turn(d);


        if (enemy.IsTurning)
            rigidbody.velocity = Vector2.zero;
        else
            rigidbody.velocity = d * chasingSpeed * TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;


        return "";
    }

    protected virtual void StopChasing()
    {
        if (enemyType == EnemyType.Floating)
            rigidbody.velocity = Vector2.zero;
        else
        {
            Vector2 v = rigidbody.velocity;
            v.x = 0;

            rigidbody.velocity = v;
        }
    }

    protected virtual string Attack(Vector3 direction)
    {
        enemy.OnEnableHitBox.AddListener(InitializeHitBox);

        if (hitBox >= 0)
            enemy.EnableHitBox(hitBox);


        enemy.Turn(direction);


        if (!enemy.IsTurning)
            animator.Play(attackAnimation, -1, 0f);

        AudioManager.Singleton.PlayOnce("Robot_attack");

        return "";
    }
}
