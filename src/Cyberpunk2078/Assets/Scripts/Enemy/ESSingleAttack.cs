using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_SingleAttack", menuName = "Enemy State/Single Attack")]
public class ESSingleAttack : EnemyState
{
    [Header("Configuration")]
    [SerializeField] protected int hitID = -1;
    [SerializeField] protected int hitBox = -1;
    [SerializeField] [Min(0)] private float motionTime;
    [SerializeField] private string animation;

    [Header("Connected States")]
    [SerializeField] private string state_afterAttack;

    protected Enemy enemy;
    protected HitData hitData;
    protected Animator animator;

    private float t_motion = 0;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);


        this.enemy = enemy;

        hitData = DataTableManager.singleton.GetHitData(hitID);
        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        t_motion = 0;


        enemy.OnEnableHitBox.AddListener(InitializeHitBox);


        if (hitBox >= 0)
            enemy.EnableHitBox(hitBox);


        animator.Play(animation);
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnEnableHitBox.RemoveListener(InitializeHitBox);


        if (hitBox >= 0)
            enemy.DisableHitBox(hitBox);
    }


    public override string Update()
    {
        t_motion += TimeManager.Instance.ScaledDeltaTime;


        if (t_motion >= motionTime)
            return state_afterAttack;


        return Name;
    }


    protected virtual void InitializeHitBox(HitBox hitBox)
    {
        hitBox.LoadHitData(hitData);
    }
}
