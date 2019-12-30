using UnityEngine;


public abstract class ESGuard<T> : EnemyState<T> where T : Enemy
{
    [Header("Configuration")]
    [SerializeField] private bool useSightRange = true;
    [SerializeField] private bool useGuardZone = true;
    [SerializeField] private float duration = 3;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_idle = -1;
    [SerializeField] private int stateIndex_alert = -1;

    protected Rigidbody2D rigidbody;

    private float t_stop;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        rigidbody.velocity = Vector2.zero;


        t_stop = duration == 0 ? float.MaxValue : Time.time + duration;
    }

    public override int Update()
    {
        float t = Time.time;

        if (t >= t_stop)
            return stateIndex_idle;


        PlayerCharacter target = FindAvailableTarget(useSightRange, useGuardZone);

        if (target)
            return stateIndex_alert;


        return Index;
    }
}
