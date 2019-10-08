using UnityEngine;


public abstract class ESChargedDash<T> : ESAttack<T> where T : Enemy
{
    [SerializeField] private float chargeTime = 1;
    [SerializeField] private float dashForce = 8000;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float stopDistance = 0;

    [Header("Animation")]
    [SerializeField] private string chargeAnimation = "";
    [SerializeField] private string dashAnimation = "";

    [Header("Connected States")]
    [SerializeField] private int stateIndex_alert = -1;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private float t;
    private bool bDash;
    private bool bStop;
    private Vector3 direction;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override int Update()
    {
        if (Time.time >= t)
        {
            if (bDash)
            {
                rigidbody.AddForce(direction * dashForce);

                if (stopDistance == 0)
                    t = Time.time + dashDuration;

                bDash = false;

                if (hitBox >= 0)
                    enemy.EnableHitBox(hitBox);


                animator.Play(dashAnimation);
            }
            else if (enemy.GuardZone.Contains(enemy.transform.position) && bStop)
            {
                float dx = enemy.currentTarget.transform.position.x - enemy.transform.position.x;

                if (Mathf.Abs(dx) <= stopDistance || dx * direction.x < 0)
                {
                    t = Time.time + dashDuration;

                    bStop = false;
                }
            }
            else
            {
                Stop();


                if (hitBox >= 0)
                    enemy.DisableHitBox(hitBox);


                return stateIndex_alert;
            }
        }


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.OnAttack.AddListener(Stop);


        t = Time.time + chargeTime;
        bDash = true;
        bStop = stopDistance != 0;


        direction = enemy.currentTarget.transform.position - enemy.transform.position;

        Vector2 groundNormal = GetGroundNormal();

        direction = direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized;

        AdjustFacingDirection(direction);


        animator.Play(chargeAnimation);
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnAttack.RemoveListener(Stop);
    }


    private void Stop()
    {
        t = 0;
        rigidbody.velocity = Vector2.zero;
    }
}
