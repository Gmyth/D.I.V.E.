using UnityEngine;


public abstract class ESGuard<T> : EnemyState<T> where T : Enemy
{
    [Header("Configuration")]
    [SerializeField] private bool useSightRange = true;
    [SerializeField] private bool useGuardZone = true;
    [SerializeField] private float duration = 3;

    [Header("Connected States")]
    [SerializeField] private string state_onEnd = "";
    [SerializeField] private string state_onTargetFound = "";

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

    public override string Update()
    {
        float t = Time.time;

        if (t >= t_stop)
            return state_onEnd;


        PlayerCharacter target = FindAvailableTarget(useSightRange, useGuardZone);

        if (target)
            return state_onTargetFound;


        return Name;
    }
}
