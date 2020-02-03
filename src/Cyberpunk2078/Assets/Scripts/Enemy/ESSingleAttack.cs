using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_SingleAttack", menuName = "Enemy State/Single Attack")]
public class ESSingleAttack : EnemyState
{
    [Header("Configuration")]
    [SerializeField] protected int motionID = -1;
    [SerializeField] protected float damage = 1;
    [SerializeField] protected float knockback = 0;
    [SerializeField] protected int hitBox = -1;
    [SerializeField] [Min(0)] private float motionTime;
    [SerializeField] private string animation;

    [Header("Connected States")]
    [SerializeField] private string state_afterAttack;

    protected Enemy enemy;
    protected Animator animator;

    private float t_motion = 0;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);


        this.enemy = enemy;


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        t_motion = 0;


        Hit hit = new Hit();
        hit.type = Hit.Type.Melee;
        hit.source = enemy;
        hit.damage = damage;
        hit.knockback = knockback;

        enemy.currentHit = hit;


        if (hitBox >= 0)
            enemy.EnableHitBox(hitBox);


        animator.Play(animation);
    }

    public override void OnStateQuit(State nextState)
    {
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
}
