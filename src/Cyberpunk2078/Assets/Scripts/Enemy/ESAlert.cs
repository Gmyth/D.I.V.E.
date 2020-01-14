using UnityEngine;


public abstract class ESAlert<T> : EnemyState<T> where T : Enemy
{
    [SerializeField] protected float waitTime;

    [Header("Animation")]
    [SerializeField] protected string animation;

    [Header("Connected States")]
    [SerializeField] protected string[] states_attacks;
    [SerializeField] protected string state_onTargetLoss = "";
}
