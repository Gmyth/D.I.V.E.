using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Idle", menuName = "Enemy State/Idle")]
public class ESIdle : EnemyState
{
    [Header("Connected States")]
    [SerializeField] private int index_ESAlert;

    private Enemy enemy;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);

        this.enemy = enemy;
    }


    public override int Update()
    {
        enemy.currentTarget = FindAvailableTarget(enemy.transform.position, enemy.SightRange, enemy.GuardZone);

        if (enemy.currentTarget)
            return index_ESAlert;


        return Index;
    }
}
