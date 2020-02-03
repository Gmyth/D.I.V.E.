﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_ShieldSlamPreparation", menuName = "Enemy State/Level 2/Shield Boss/Shield Slam Preparation")]
public class L2ShieldBossState_ShieldSlamPreparation : EnemyState<L2ShieldBoss>
{
    [Header("Configuration")]
    [SerializeField] [Min(0)] private float minDuration = 0.2f;
    [SerializeField] [Min(0)] private float maxDuration = 3f;
    [SerializeField] [Min(0)] private float minDistance = 2f;
    [SerializeField] [Min(0)] private float maxDistance = 4f;
    [SerializeField] [Min(0)] private float attackHeight = 3f;
    [SerializeField] private string animation = "L2ShieldBoss_ShieldSlamPreparation";

    [Header("Connected States")]
    [SerializeField] private string state_onFailure = "ShieldSlamChase";

    protected Animator animator;

    private float t_motion = 0;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        t_motion = 0;


        enemy.StopTurning();


        animator.Play(animation);
    }


    public override string Update()
    {
        t_motion += TimeManager.Instance.ScaledDeltaTime;


        if (t_motion >= minDuration)
        {
            Vector3 d = enemy.currentTarget.transform.position - enemy.transform.position;
            float dx = Mathf.Abs(d.x);

            if (dx <= minDistance)
            {
                if (Mathf.Abs(d.y) >= attackHeight)
                    return "Alert";


                return "ShieldSlam";
            }
            else if (dx >= maxDistance || t_motion >= maxDuration)
                return state_onFailure;
        }


        return Name;
    }
}
