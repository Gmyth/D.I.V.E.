using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Idle", menuName = "Enemy State/Idle")]
public class ESIdle : ESIdle<Enemy>
{
}



public class ESIdle<T> : EnemyState<T> where T : Enemy
{
    [Header("Detection")]
    [SerializeField] private bool enableDetection = true;


    public override string Update()
    {
        if (enableDetection)
        {
            enemy.currentTarget = FindAvailableTarget(enemy.transform.position, enemy[StatisticType.SightRange], enemy.GuardZone);

            if (enemy.currentTarget)
                return "Alert";
        }


        return Name;
    }
}
