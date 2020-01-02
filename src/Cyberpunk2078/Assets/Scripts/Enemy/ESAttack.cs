using UnityEngine;


public enum OrientationType
{
    Omnidirectional,
    UpwardOnly,
    Horizontal,
}


public abstract class ESAttack<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected int motionID = -1;
    [SerializeField] protected float damage = 1;
    [SerializeField] protected float knockback = 0;
    [SerializeField] protected int hitBox = -1;

    private MotionData motionData;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        if (motionID >= 0)
            motionData = DataTableManager.singleton.GetMotionData(motionID);
    }


    public override void OnStateEnter(State previousState)
    {
        Hit hit = new Hit();
        hit.source = enemy;
        hit.damage = CalculateAttackDamage();
        hit.knockback = CalculateAttackKnowback();

        enemy.currentHit = hit;
    }


    protected virtual float CalculateAttackDamage()
    {
        return damage;
    }


    protected virtual float CalculateAttackKnowback()
    {
        return knockback;
    }
}
