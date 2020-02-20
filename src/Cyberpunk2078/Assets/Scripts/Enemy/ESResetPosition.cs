using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_ResetPosition", menuName = "Enemy State/Reset Position")]
public class ESResetPosition : ESResetPosition<Enemy>
{
}


public class ESResetPosition<T> : EnemyState<T> where T : Enemy
{
    [Header("Movement")]
    [SerializeField][Min(0)] private float speed = 2f;
    [SerializeField][Min(0)] private float stopDistance = 0.1f;

    [Header("")]
    [SerializeField] private string connectedState = "Idle";

    private Vector3 originalPosition;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);


        originalPosition = enemy.transform.position;
    }

    public override string Update()
    {
        Vector3 d = enemy.transform.position - originalPosition;


        float distance = 0;
        Vector3 direction = Vector3.zero;

        switch (enemy.Data.Type)
        {
            case EnemyType.Floating:
                distance = d.magnitude;
                direction = d.normalized;
                break;


            case EnemyType.Ground:
                distance = d.magnitude;
                direction = d.x > 0 ? Vector3.right : Vector3.left;
                break;
        }


        if (distance <= stopDistance)
            return connectedState;


        enemyRigidbody.velocity = speed * direction;


        return Name;
    }
}
