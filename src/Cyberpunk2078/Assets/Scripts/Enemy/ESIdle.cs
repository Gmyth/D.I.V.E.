using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Idle", menuName = "Enemy State/Idle")]
public class ESIdle : EnemyState
{
    private Enemy enemy;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);

        this.enemy = enemy;
    }


    public override string Update()
    {
        enemy.currentTarget = FindAvailableTarget(enemy.transform.position, enemy[StatisticType.SightRange], enemy.GuardZone);

        if (enemy.currentTarget)
            return "Idle";


        return Name;
    }
}
