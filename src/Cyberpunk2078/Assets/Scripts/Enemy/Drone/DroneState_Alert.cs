using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2DroneState_Alert", menuName = "Enemy State/Drone/Alert")]
public class DroneState_Alert : ESAlert<Drone>
{
    [SerializeField] private RandomSelector behaviorSelector;
    [SerializeField] private float stayDuration = 0.5f;
    [SerializeField] private float chasingSpeed = 3f;
    [SerializeField] private float chasingDuration = 0.5f;
    [SerializeField] private float backingSpeed = 1f;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private float t_ready;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override string Update()
    {
        if (!IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return state_onTargetLoss;


        if (!enemy.GuardZone.Contains(enemy.currentTarget))
            return states_attacks[0];


        Vector3 v = enemy.currentTarget.transform.position - enemy.transform.position;
        float d = v.magnitude;
        float t = Time.time;


        if (d < enemy.NearRange) // Prioritize on keeping a good distance
        {
            Vector3 u = (Vector3)enemy.GuardZone.center - enemy.transform.position;

            rigidbody.velocity = backingSpeed * ((-v.normalized + u.normalized) / 2).normalized;
        }
        else
        {
            enemy.Turn();


            if (d < enemy.NearRange + 0.5f) // Always fire before getting to close or far
                return states_attacks[0];
            else if (d <= enemy.FarRange)
            {
                if (t >= t_ready)
                {
                    switch (behaviorSelector.Select()[0])
                    {
                        case 0: // Fire
                            return states_attacks[0];


                        case 1: // Chase
                            rigidbody.velocity = chasingSpeed * v.normalized;
                            t_ready = t + chasingDuration;
                            break;


                        case 2: // Stay
                            rigidbody.velocity = Vector2.zero;
                            t_ready = t + stayDuration;
                            break;
                    }
                }
            }
            else
                rigidbody.velocity = chasingSpeed * v.normalized;
        }
        

        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_ready = 0;


        animator.Play(Drone.animation_alert);
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        rigidbody.velocity = Vector2.zero;
    }
}
