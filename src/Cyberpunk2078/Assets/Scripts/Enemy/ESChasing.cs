using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Chasing", menuName = "Enemy State/Chasing")]
public class ESChasing : ESChasing<Enemy>
{
}


public class ESChasing<T> : EnemyState<T> where T : Enemy
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] private float speed;
    [SerializeField] [Min(0)] private float stopDistance;
    [SerializeField] private string animation_idle = "";
    [SerializeField] private string animation_chasing = "";

    [Header("Connected States")]
    [SerializeField] private string state_onTargetLoss = "";
    [SerializeField] private string state_onStop = "";

    private bool isChasing = false;


    protected bool IsChasing
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
                    if (animation_chasing != "")
                        enemyAnimator.Play(animation_chasing);

                    StartChasing();
                }
                else
                {
                    if (animation_idle != "")
                        enemyAnimator.Play(animation_idle);

                    StopChasing();
                }
            }
        }
    }


    public override void OnStateEnter(State previousState)
    {
        enemyAnimator.Play(animation_chasing);


        isChasing = false;
    }


    public override string Update()
    {
        PlayerCharacter player = IsPlayerInSight(enemy.currentTarget, enemy, enemy[StatisticType.SightRange]);

        if (player && enemy.GuardZone.Contains(player)) // Check if the target is still available
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 enemyPosition = enemy.transform.position;


            Vector3 d = playerPosition - enemyPosition;

            if (Mathf.Abs(d.x) <= stopDistance) // Check if the target is horizontally close enough
            {
                IsChasing = false;


                return state_onStop;
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


        return state_onTargetLoss;
    }


    protected virtual void StartChasing()
    {
    }

    protected virtual string Chase(Vector3 direction)
    {
        Vector3 d = ((Vector2)direction).normalized;

        if (enemy.Data.Type == EnemyType.Ground)
            d = d.x > 0 ? Vector2.right : Vector2.left;


        enemy.Turn(d);


        if (enemy.IsTurning)
            enemyRigidbody.velocity = Vector2.zero;
        else
            enemyRigidbody.velocity = d * speed * TimeManager.Instance.TimeFactor * enemy.UnitTimeFactor;


        return "";
    }

    protected virtual void StopChasing()
    {
        if (enemy.Data.Type == EnemyType.Floating)
            enemyRigidbody.velocity = Vector2.zero;
        else
        {
            Vector2 v = enemyRigidbody.velocity;
            v.x = 0;

            enemyRigidbody.velocity = v;
        }
    }
}
