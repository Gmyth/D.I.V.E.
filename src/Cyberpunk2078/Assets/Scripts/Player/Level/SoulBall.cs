using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SoulBallState
{
    Move,
    Chase,
    Acquired
}

public class SoulBall : Recyclable
{
    private float MaxSpeed = 16f;
    private SoulBallState currentState;
    private float LastFloatingForceTime;
    private Transform target;
    private Rigidbody2D rb2d;

    private float t0;
    private float timeIntervals = 0.6f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        var bufferState = currentState;

        switch (currentState)
        {
            case SoulBallState.Move:
                if (t0 + timeIntervals<Time.time)
                {
                    bufferState = SoulBallState.Chase;
                    rb2d.velocity = Vector2.zero;
                }
                break;
            
            case SoulBallState.Chase:
                
                if ((target.position - transform.position).sqrMagnitude < 0.1f)
                {
                    // TODO  the absorb range could be attribute
                    rb2d.drag = 0;
                    bufferState = SoulBallState.Acquired;

                    PlayerCharacter.Singleton.AddFever(2f);

                    Die();
                }
                
                var direction = (target.position - transform.position).normalized;
                transform.Translate(direction * 9.5f * Time.deltaTime);
                
                break;
            
            case SoulBallState.Acquired:
                // do nothing
                
                break;
            
        }

        if (bufferState != currentState)
        {
            currentState = bufferState;
        }

    }

    public void Active()
    {
        t0 = Time.time;
        rb2d = GetComponent<Rigidbody2D>();
        currentState = SoulBallState.Move;
        var dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        rb2d.AddForce(Random.Range(15,40) * dir);
        target = GameObject.FindObjectOfType<PlayerCharacter>().transform;
        timeIntervals += Random.Range(-0.2f, 0.2f);
    }
}
