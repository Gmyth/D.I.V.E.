using UnityEngine;


public abstract class ESAttack<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected AttributeSet attributes;
}
