using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Reposition", menuName = "Enemy State/Reposition")]
public class ESReposition : EnemyState
{
    [SerializeField] protected bool useRelativePosition = false;
    [SerializeField] protected Vector2 targetPosition;
    [SerializeField] protected float speed;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected string animation;
    [SerializeField] protected string nextState;

    protected Enemy enemy;
    protected Rigidbody2D rigidbody;
    protected Animator animator;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);


        this.enemy = enemy;


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        animator.Play(animation);
    }

    public override void OnStateQuit(State nextState)
    {
        rigidbody.velocity = Vector2.zero;
    }

    public override string Update()
    {
        Vector2 d = targetPosition - (Vector2)enemy.transform.position;


        bool hasArrived = false;
        Vector2 v = Vector2.zero;

        switch (enemy.Data.Type)
        {
            case EnemyType.Ground:
                hasArrived = Mathf.Abs(d.x) <= stopDistance;
                v = (d.x > 0 ? Vector2.right : Vector2.left) * speed * TimeManager.Instance.TimeFactor;
                break;


            case EnemyType.Floating:
                hasArrived = d.magnitude <= stopDistance;
                v = d.normalized * speed * TimeManager.Instance.TimeFactor;
                break;
        }


        if (hasArrived)
            return nextState;


        enemy.AdjustFacing(d);

        rigidbody.velocity = v;


        return Name;
    }
}
