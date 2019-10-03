using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ESChargedDash<T> : ESAttack<T> where T : Enemy
{
    [SerializeField] private float chargeTime = 1;
    [SerializeField] private float dashForce = 8000;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_alert = -1;

    private Rigidbody2D rigidbody;

    private float t;
    private bool b;
    private Vector3 direction;


    public override void Initialize(T enemy)
    {
        base.Initialize(enemy);

        rigidbody = enemy.GetComponent<Rigidbody2D>();
    }

    public override int Update()
    {
        if (Time.time >= t)
        {
            if (b)
            {
                rigidbody.AddForce(direction * dashForce);

                t = Time.time + dashDuration;
                b = false;

                if (hitBox >= 0)
                    enemy.EnableHitBox(hitBox);
            }
            else
            {
                Stop();


                if (hitBox >= 0)
                    enemy.DisableHitBox(hitBox);


                return stateIndex_alert;
            }
        }


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.OnAttack.AddListener(Stop);


        t = Time.time + chargeTime;
        b = true;


        direction = enemy.currentTarget.transform.position - enemy.transform.position;

        Vector2 groundNormal = GetGroundNormal();

        direction = direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized;

        AdjustFacingDirection(direction);
    }

    public override void OnStateQuit(State nextState)
    {
        enemy.OnAttack.RemoveListener(Stop);
    }


    private void Stop()
    {
        t = 0;
        rigidbody.velocity = Vector2.zero;
    }
}
