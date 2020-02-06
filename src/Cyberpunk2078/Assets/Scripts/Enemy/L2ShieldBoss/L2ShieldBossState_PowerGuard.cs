﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_PowerGuard", menuName = "Enemy State/Shield Boss/Power Guard")]
public class L2ShieldBossState_PowerGuard : EnemyState<L2ShieldBoss>
{
    [SerializeField] private int hitBox = 3;
    [SerializeField] private float knockbackDrag = 5f;
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private float knockbackDuration = 1f;
    [SerializeField] private string counterAttack = "ChargedDash";
    [SerializeField][Range(0, 1)] private float counterAttackChance = 30;
    [SerializeField] private float counterAttackStartTime = 0.5f;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private float t_finish = 0;
    private float t_counterAttack = 0;
    private float r = 0;
    private Vector3 backDirection;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        float t = Time.time;

        t_finish = t + knockbackDuration;
        t_counterAttack = t + counterAttackStartTime;
        r = Random.Range(0, 100);
        backDirection = -enemy.transform.localScale.x * enemy.transform.right;


        enemy.TurnImmediately();
        enemy.EnableHitBox(hitBox);


        rigidbody.drag = knockbackDrag;
        rigidbody.velocity = backDirection * knockbackSpeed;


        animator.Play("L2ShieldBoss_Guard");
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.DisableHitBox(hitBox);


        rigidbody.drag = 0;
        rigidbody.velocity = Vector2.zero;
    }

    public override string Update()
    {
        enemy.Turn();


        float t = Time.time;


        if (counterAttack != "" && t >= t_counterAttack && r <= counterAttackChance)
            return counterAttack;


        if (t >= t_finish)
            return "Alert";


        return Name;
    }
}
