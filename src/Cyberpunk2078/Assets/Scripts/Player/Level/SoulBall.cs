using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SoulBallState
{
    Move,
    Ready,
    Chase,
    Acquired
}

public class SoulBall : Recyclable
{

    private float MoveTime;
    private float MoveTimeInterval;
    private float MaxSpeed;
    private SoulBallState currentState;
    private float LastFloatingForce;
    private Transform target;
    private Rigidbody2D rb2d;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        var bufferState = currentState;
        switch (currentState)
        {
            case SoulBallState.Move:
                if (Mathf.Abs(rb2d.velocity.sqrMagnitude) < 1)
                {
                    bufferState = SoulBallState.Ready;
                }
                break;
            case SoulBallState.Ready:
                if ((target.position - transform.position).sqrMagnitude < 1f)
                {
                    // TODO  the absorb range could be attribute
                    rb2d.drag = 0;
                    bufferState = SoulBallState.Chase;
                }

                if (LastFloatingForce + 0.8f > Time.time)
                {
                    LastFloatingForce = Time.time;
                    var dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                    rb2d.AddForce(8 * dir);
                }
                break;
            
            case SoulBallState.Chase:
                if ((target.position - transform.position).sqrMagnitude < 0.1f)
                {
                    // TODO  the absorb range could be attribute
                    rb2d.drag = 0;
                    bufferState = SoulBallState.Acquired;
                    Acquire();
                }
                
                var direction = (target.position - transform.position).normalized;
                if (rb2d.velocity.sqrMagnitude < MaxSpeed)
                {
                    rb2d.AddForce(direction * 2f * Time.deltaTime);
                }
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
        rb2d = GetComponent<Rigidbody2D>();
        currentState = SoulBallState.Move;
        var dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        rb2d.AddForce(Random.Range(50,300) * dir);
        MoveTime = Time.time;
        target = GameObject.FindObjectOfType<PlayerCharacter>().transform;
    }

    void Acquire()
    {
        //add 
        
    }
}
