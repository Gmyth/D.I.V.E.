using UnityEngine;


public abstract class ESAlert<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected float waitTime;

    [Header("Animation")]
    [SerializeField] protected string animation;

    [Header("Connected States")]
    [SerializeField] protected int[] stateIndex_attacks;
    [SerializeField] protected int stateIndex_targetLoss = -1;
}
