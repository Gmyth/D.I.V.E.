using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Timed", menuName = "Enemy State/Timed")]
public class ESTimed : ESTimed<Enemy>
{
}


public class ESTimed<T> : EnemyState<T> where T : Enemy
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] protected float duration;
    [SerializeField] protected string animation;

    [Header("Connected States")]
    [SerializeField] protected string nextState;

    protected float t_finish = 0;


    public override void OnStateEnter(State previousState)
    {
        t_finish = 0;


        enemyAnimator.Play(animation);
        AudioManager.Singleton.PlayOnce("Boss_jump");
    }


    public override string Update()
    {
        t_finish += TimeManager.Instance.ScaledDeltaTime;


        if (t_finish >= duration)
            return nextState;


        return Name;
    }
}
