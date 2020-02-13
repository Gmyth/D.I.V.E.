﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_LeapShieldSlam", menuName = "Enemy State/Shield Boss/Leap Shield Slam")]
public class L2ShieldBossState_LeapShieldSlam : ESAttack<L2ShieldBoss>
{
    [Header("Leap")]
    [SerializeField][Min(0)] private float leapChargeTime = 0.5f;
    [SerializeField] private string animation_charge;

    private Rigidbody2D rigidbody;

    private float t_charge;
    private float t_leap;


    public override void Initialize(L2ShieldBoss enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_charge = 0;
        t_leap = 0;
    }


    protected override string BeforeAttack()
    {
        if (t_phase == 0)
            animator.Play(animation_charge);
        

        if (t_charge >= leapChargeTime)
        {
            if (t_leap == 0)
            {
                Vector3 playerPosition = enemy.currentTarget.transform.position;


                Vector3 initialPosition = enemy.transform.position;
                Vector3 targetPosition = playerPosition + new Vector3(Mathf.Sign((initialPosition - playerPosition).x) * 2, 0, 0);
                float g = -Physics2D.gravity.y * rigidbody.gravityScale;

                rigidbody.velocity = new Vector3((targetPosition.x - initialPosition.x) / attackPoint, (targetPosition.y - initialPosition.y) / attackPoint + 0.5f * g * attackPoint, (targetPosition.z - initialPosition.z) / attackPoint);


                animator.Play(animation_beforeAttack);
            }
            else if (t_leap >= attackPoint)
            {
                phase = Phase.Attack;
                t_phase = 0;
            }


            t_leap += TimeManager.Instance.ScaledDeltaTime;
        }


        t_charge += TimeManager.Instance.ScaledDeltaTime;


        return "";
    }

    protected override string Attack()
    {
        Vector2 v = rigidbody.velocity;
        v.x = 0;

        rigidbody.velocity = v;


        return base.Attack();
    }
}
