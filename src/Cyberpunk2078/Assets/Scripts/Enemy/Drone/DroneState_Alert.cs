using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2DroneState_Alert", menuName = "Enemy State/Drone/Alert")]
public class DroneState_Alert : ESAlert<Drone>
{
    [SerializeField] private float chasingSpeed = 3f;
    [SerializeField] private float chasingDuration = 0.5f;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private float t_chasing;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_chasing = 0;


        animator.Play("L2Drone_Alert");
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        rigidbody.velocity = Vector2.zero;
    }

    public override int Update()
    {
        if (!IsPlayerInSight(enemy.currentTarget, enemy[StatisticType.SightRange]))
            return stateIndex_targetLoss;


        if (!enemy.GuardZone.Contains(enemy.currentTarget))
            return stateIndex_attacks[0];


        AdjustFacingDirection((enemy.currentTarget.transform.position - enemy.transform.position).x > 0 ? Vector3.right : Vector3.left);


        Vector3 v = enemy.currentTarget.transform.position - enemy.transform.position;
        float t = Time.time;


        if (t > t_chasing)
        {
            float d = v.magnitude;


            if (d < enemy.NearRange || (d < enemy.FarRange && Random.Range(0f, 100f) < 50))
                return stateIndex_attacks[0];


            rigidbody.velocity = chasingSpeed * v.normalized;
            t_chasing = t + chasingDuration;
        }


        return Index;
    }
}
