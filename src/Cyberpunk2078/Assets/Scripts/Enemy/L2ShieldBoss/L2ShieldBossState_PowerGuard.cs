using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_PowerGuard", menuName = "Enemy State/Shield Boss/Power Guard")]
public class L2ShieldBossState_PowerGuard : EnemyState<L2ShieldBoss>
{
    [SerializeField] private int hitBox = 3;
    [SerializeField] private float duration = 1f;
    
    private Animator animator;

    private float t_finish = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_finish = Time.time + duration;


        enemy.EnableHitBox(hitBox);

        animator.Play("L2ShieldBoss_Guard");
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.DisableHitBox(hitBox);
    }

    public override string Update()
    {
        AdjustFacingDirection();


        float t = Time.time;


        if (t >= t_finish)
        {
            float r = Random.Range(0, 100);


            if (r <= 15)
                return "ChargedDash";


            Vector3 d = enemy.currentTarget.transform.position - enemy.transform.position;


            if (r <= 50 && Mathf.Abs(d.x) < 2.5f)
                return "CounterAttack";


            return "Alert";
        }


        return Name;
    }
}
