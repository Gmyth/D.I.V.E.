using UnityEngine;


public abstract class ESChargedDash<T> : ESChargedAttack<T> where T : Enemy
{
    [Header("Dash Configuration")]
    [SerializeField] private float dashForce = 8000;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float stopDistance = 0;
    [SerializeField] private string dashAnimation = "";

    private Rigidbody2D rigidbody;

    private float t1;
    private bool bDash;
    private bool bStop;
    private Vector3 direction;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.OnAttack.AddListener(Stop);


        t1 = float.MaxValue;
        bDash = true;
        bStop = stopDistance != 0;


        direction = enemy.currentTarget.transform.position - enemy.transform.position;

        Vector2 groundNormal = GetGroundNormal();

        direction = direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized;

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
        if (currentTime >= t1) // The dash is finished
        {
            Stop();

            return stateIndex_alert;
        }
        else if (bDash) // Start dashing
        {
            rigidbody.AddForce(direction * dashForce);


            bDash = false;

            if (!bStop)
                t1 = Time.time + dashDuration;


            if (hitBox >= 0)
                enemy.EnableHitBox(hitBox);


            animator.Play(dashAnimation);
        }
        else if (bStop && enemy.GuardZone.Contains(enemy.transform.position)) // During dashing
        {
            float dx = enemy.currentTarget.transform.position.x - enemy.transform.position.x;

            if (Mathf.Abs(dx) <= stopDistance || dx * direction.x <= 0)
            {
                t1 = Time.time + dashDuration;

                bStop = false;
            }
        }


        return Index;
    }


    private void Stop()
    {
        t1 = 0;
        rigidbody.velocity = Vector2.zero;
    }
}
