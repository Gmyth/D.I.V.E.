using UnityEngine;


public enum OrientationType
{
    Omnidirectional,
    UpwardOnly,
    Horizontal,
}


public abstract class ESAttack<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected int hitDataID = -1;
    [SerializeField] protected int hitBox = -1;

    protected HitData hitData;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        hitData = DataTableManager.singleton.GetHitData(hitDataID);
    }

    public override void OnStateEnter(State previousState)
    {
        enemy.OnEnableHitBox.AddListener(InitializeHitBox);


        if (hitBox >= 0)
            enemy.EnableHitBox(hitBox);
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnEnableHitBox.RemoveListener(InitializeHitBox);


        if (hitBox >= 0)
            enemy.DisableHitBox(hitBox);
    }


    protected virtual void InitializeHitBox(HitBox hitBox)
    {
        hitBox.LoadHitData(hitData);
    }
}
