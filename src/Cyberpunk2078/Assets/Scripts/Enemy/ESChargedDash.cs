using UnityEngine;


public abstract class ESChargedDash<T> : ESChargedAttack<T> where T : Enemy
{
    [Header("Dash Configuration")]
    [SerializeField] private OrientationType type = OrientationType.Omnidirectional;
    [SerializeField] private float dashForce = 8000;
    [SerializeField] private float minDuration = 0.15f;
    [SerializeField] private float maxDuration = 0;
    [SerializeField] private float stopDistance = 1;
    [SerializeField] private float dashEndTime = 0;
    [SerializeField] private string animation_dash = "";
    [SerializeField] private string animation_dashEnd = "";

    private Rigidbody2D rigidbody;

    private Vector3 direction;
    private bool bDash;
    private bool bStop;
    private float t_dash;
    private float t_dashEnd;
    private float d;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);

        AudioManager.Instance.PlayOnce("RobotDashCharge");

        enemy.OnAttack.AddListener(Stop);


        bDash = true;
        bStop = false;
        t_dash = 0;
        t_dashEnd = 0;
        d = float.MaxValue;
        

        direction = (enemy.currentTarget.transform.position - enemy.transform.position).normalized;

        if ((type == OrientationType.UpwardOnly && direction.y < 0) || type == OrientationType.Horizontal)
        {
            Vector2 groundNormal = GetGroundNormal();
            
            direction = direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized;
        }


        enemy.Turn(direction);
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnAttack.RemoveListener(Stop);


        rigidbody.velocity = Vector2.zero;


        if (hitBox >= 0)
            enemy.DisableHitBox(hitBox);
    }


    protected override string Attack(float currentTime)
    {
        t_dash += TimeManager.Instance.ScaledDeltaTime;


        if (bDash) // The dash has not been performed
        {
            rigidbody.AddForce(direction * dashForce);


            bDash = false;


            t_dash = 0;


            if (hitBox >= 0)
                enemy.EnableHitBox(hitBox);


            animator.Play(animation_dash);

            AudioManager.Instance.PlayOnce("RobotDash");
        }
        else if (!bStop)
        {
            if (enemy.GuardZone.Contains(enemy.transform.position))
            {
                if (type == OrientationType.Horizontal)
                {
                    float dx = enemy.currentTarget.transform.position.x - enemy.transform.position.x;

                    if (Mathf.Abs(dx) <= stopDistance || Mathf.Sign(dx) != Mathf.Sign(direction.x))
                        bStop = true;
                }
                else
                {
                    Vector3 d = enemy.currentTarget.transform.position - enemy.transform.position;

                    if (d.sqrMagnitude <= stopDistance * stopDistance || Vector3.Dot(direction, d.normalized) < -0.5)
                        bStop = true;
                }
            }
            else
            {
                string nextStateName = Stop();

                if (nextStateName != "")
                    return nextStateName;
            }
        }
        else if (t_dash >= minDuration) // The dash has been finished
        {
            string nextStateName = Stop();

            if (nextStateName != "")
                return nextStateName;
        }
        

        return Name;
    }


    protected string Stop()
    {
        if (t_dashEnd == 0)
        {
            if (hitBox >= 0)
                enemy.DisableHitBox(hitBox);


            rigidbody.velocity /= 10;
            rigidbody.drag = 300;

            bStop = true;


            if (animation_dashEnd != "")
                animator.Play(animation_dashEnd);
        }
        

        t_dashEnd += TimeManager.Instance.ScaledDeltaTime;

        if (t_dashEnd >= dashEndTime)
        {
            rigidbody.drag = 0;
            rigidbody.velocity = Vector2.zero;


            return "Alert";
        }


        return "";
    }

    protected void Stop(Hit hit, Collider2D collider)
    {
        Stop();
    }
}
