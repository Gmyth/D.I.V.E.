using UnityEngine;


public abstract class ESChargedDash<T> : ESChargedAttack<T> where T : Enemy
{
    [Header("Dash Configuration")]
    [SerializeField] private OrientationType type = OrientationType.Omnidirectional;
    [SerializeField] private float dashForce = 8000;
    [SerializeField] private float minDuration = 0.15f;
    [SerializeField] private float maxDuration = 0;
    [SerializeField] private float stopDistance = 1;
    [SerializeField] private string animation_dash = "";

    private Rigidbody2D rigidbody;

    private Vector3 direction;
    private bool bDash;
    private bool bStop;
    private float t;
    private float d;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.OnAttack.AddListener(Stop);


        bDash = true;
        bStop = false;
        t = float.MaxValue;
        d = float.MaxValue;
        

        direction = (enemy.currentTarget.transform.position - enemy.transform.position).normalized;

        if ((type == OrientationType.UpwardOnly && direction.y < 0) || type == OrientationType.Horizontal)
        {
            Vector2 groundNormal = GetGroundNormal();

            direction = direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized;
        }


        AdjustFacingDirection(direction);
    }

    public override void OnStateQuit(State nextState)
    {
        if (hitBox >= 0)
            enemy.DisableHitBox(hitBox);


        enemy.OnAttack.RemoveListener(Stop);
    }


    protected override int Attack(float currentTime)
    {
        if (bDash) // The dash has not been performed
        {
            rigidbody.AddForce(direction * dashForce);


            bDash = false;


            t = currentTime + minDuration;


            if (hitBox >= 0)
                enemy.EnableHitBox(hitBox);


            animator.Play(animation_dash);
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
                Stop();

                return stateIndex_alert;
            }
        }
        else if (currentTime >= t) // The dash has been finished
        {
            Stop();

            return stateIndex_alert;
        }


        return Index;
    }


    private void Stop()
    {
        rigidbody.velocity = Vector2.zero;


        bStop = true;
        t = 0;
    }
}
