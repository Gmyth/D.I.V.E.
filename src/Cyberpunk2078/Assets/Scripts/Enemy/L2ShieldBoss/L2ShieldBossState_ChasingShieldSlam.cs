﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_ChasingSieldSlam", menuName = "Enemy State/Level 2/Shield Boss/Chasing Shield Slam")]
public class L2ShieldBossState_ChasingShieldSlam : ESChasingAttack<L2ShieldBoss>
{
    [SerializeField] private float maxChaseTime = 3f;

    private float t_chase;


    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.IsInvulnerable = false;


        enemy.DisableHitBox(0);
        enemy.DisableHitBox(2);
    }


    protected override void StartChasing()
    {
        base.StartChasing();


        t_chase = 0;
        enemy.IsInvulnerable = true;


        enemy.EnableHitBox(2);
    }

    protected override string Chase(Vector3 direction)
    {
        base.Chase(direction);


        t_chase += TimeManager.Instance.ScaledDeltaTime;


        if (t_chase >= maxChaseTime)
            return "Alert";


        return "";
    }

    protected override void StopChasing()
    {
        base.StopChasing();


        enemy.DisableHitBox(2);
    }
}
