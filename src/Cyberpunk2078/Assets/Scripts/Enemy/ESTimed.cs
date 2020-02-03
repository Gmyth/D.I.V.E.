using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Timed", menuName = "Enemy State/Timed")]
public class ESTimed : EnemyState
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] private float duration;
    [SerializeField] private string animation;

    [Header("Connected States")]
    [SerializeField] private string nextState;

    protected Enemy enemy;
    protected Animator animator;

    private float t_finish = 0;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);


        this.enemy = enemy;


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        t_finish = 0;


        animator.Play(animation);
    }


    public override string Update()
    {
        t_finish += TimeManager.Instance.ScaledDeltaTime;


        if (t_finish >= duration)
            return nextState;


        return Name;
    }
}
