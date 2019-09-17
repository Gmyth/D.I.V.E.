using UnityEngine;


public interface IGuardian
{
    float MinTurnTime { get; }
    float MaxTurnTime { get; }
}


public abstract class ESGuard<T> : EnemyState<T> where T : Enemy, IGuardian
{
    [Header("Connected States")]
    [SerializeField] private int index_ESIdle = -1;
    [SerializeField] private int index_ESAlert = -1;

    private float t0 = 0;


    public override int Update()
    {
        if (Time.time >= t0)
        {
            // TODO: Turn

            t0 += Random.Range(enemy.MinTurnTime, enemy.MaxTurnTime);
        }

        // TODO: Player detection

        return Index;
    }

    public override void OnStateEnter()
    {
        t0 = Time.time + Random.Range(enemy.MinTurnTime, enemy.MaxTurnTime);
    }
}
