﻿using UnityEngine;


public abstract class PlayerState : State
{
    protected PlayerCharacter playerCharacter;
    protected Animator anim;

    protected bool flip;


    public virtual void Initialize(int index, PlayerCharacter playerCharacter)
    {
        Index = index;
        this.playerCharacter = playerCharacter;
        anim = playerCharacter.GetComponent<Animator>();
    }


    public abstract int Update();


    public virtual void OnStateEnter() { }
    //public virtual void OnStateReset() { }
    public virtual void OnStateQuit() { }

    //Player Ground check
    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,-0.2f,0f),-playerCharacter.transform.up,1f);
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.1f,-0.2f,0f),-playerCharacter.transform.up,1f);
        RaycastHit2D hit2 = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.1f,-0.2f,0f),-playerCharacter.transform.up,1f);

        return hitM.collider != null && hitM.transform.CompareTag("Ground") || hitR.collider != null && hitR.transform.CompareTag("Ground") || hitL.collider != null && hitL.transform.CompareTag("Ground");
    }

    //Player Ground Normal Vector, Used for Dash direction correction
    public Vector2 GroundNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,-0.2f,0f),-playerCharacter.transform.up,1f);

        if (hit.collider != null && hit.transform.CompareTag("Ground") ) {
            return hit.normal;
        }

        return Vector2.zero;
    }

    //Check the Wall is on left or right
    public bool RightSideTest(string tag)
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(0.1f,0f,0f),playerCharacter.transform.right,0.4f);

        if (hit.collider != null && hit.transform.CompareTag(tag) )
        {
            return true;
        }
        return false;
    }

    //Check player is close to wall
    public bool isCloseTo(string tag)
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(0.1f,0f,0f),playerCharacter.transform.right,0.35f);
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(-0.1f,0f,0f),-playerCharacter.transform.right,0.35f);
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0f,-0f,0f), playerCharacter.transform.right * 0.35f, Color.red);
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0.1f,-0f,0f), -playerCharacter.transform.right * 0.35f, Color.yellow);

        if ((hit.collider != null && hit.transform.CompareTag(tag) )||
            (hit1.collider != null && hit1.transform.CompareTag(tag) ))
        {
            return true;
        }

        return false;
    }


    //if player is grounded, the direction to down left/right & down side are not allowed
    public Vector2 getDirectionCorrection(Vector2 _direction, Vector2 _norm)
    {
        if (Vector2.Angle(_direction, _norm) < 55)
        {
            // Allowed for free dash
            return _direction;
        }
        // need to be on the axis of perpendicular to norm
        Vector2 corDir  = Vector2.Perpendicular(_norm);
        if (_direction.x * corDir.x > 0) return corDir;

        return -corDir;
    }



    // Function : Keyboard Input => Physics Velocity, And Friction Calculation
    public void PhysicsInputHelper(float h, float maxSpeed  = 9,  float Acceleration  = 20)
    {
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();

        // calculate speed on X axis
        if (Mathf.Abs(h) > 0.1f)
        {
            // has horizontal input
            if (Mathf.Abs(rb2d.velocity.x) < maxSpeed)
            {
                var direction = Vector3.right * h * Acceleration;
                if (direction.x * rb2d.velocity.x < 0)
                    direction = direction * 4f;

                rb2d.AddForce(direction);
            }
            else
            {
                if (rb2d.velocity.x * h < 0)
                {
                    // not in the same direction
                    // reduce speed,friction
                    Vector2 direction = rb2d.velocity.normalized;

                    if (direction.x > 0)
                        direction.x = 1;
                    else
                        direction.x = -1;

                    rb2d.AddForce(new Vector2(-direction.x * 8f, 0f));
                }
            }
        }
        else
        {
            // does not has input
            // reduce speed, friction
            rb2d.AddForce(new Vector2(-rb2d.velocity.x * 4, 0f));
        }
    }
}


[CreateAssetMenuAttribute(fileName = "FSM_Player", menuName = "State Machine/Player")]
public class FSMPlayer : FiniteStateMachine<PlayerState>
{
    public override int CurrentStateIndex
    {
        get
        {
            return currentStateIndex;
        }

        set
        {
            if (value != currentStateIndex)
            //{
               // Debug.Log(LogUtility.MakeLogStringFormat("FSMPLayer", "Reset on state {0}", currentStateIndex));
            //    CurrentState.OnStateReset();
            //}
            //else
            {
                Debug.Log(LogUtility.MakeLogStringFormat("FSMPLayer", "Make transition from state {0} to {1}", currentStateIndex, value));

                int previousStateIndex = currentStateIndex;

                CurrentState.OnStateQuit();

                currentStateIndex = value;

                CurrentState.OnStateEnter();

                OnCurrentStateChange.Invoke(currentStateIndex, previousStateIndex);
            }
        }
    }


    public void Initialize(PlayerCharacter player)
    {
        for (int i = 0; i < states.Length; ++i)
            states[i].Initialize(i, player);

        currentStateIndex = -1;
    }

    public override void Boot()
    {
        OnMachineBoot();

        currentStateIndex = startingStateIndex;

        CurrentState.OnStateEnter();

        OnCurrentStateChange.Invoke(currentStateIndex, -1);
    }

    public override void ShutDown()
    {
        int previousStateIndex = currentStateIndex;

        CurrentState.OnStateQuit();

        currentStateIndex = -1;

        OnMachineShutDown();

        OnCurrentStateChange.Invoke(-1, previousStateIndex);
    }

    public void Update()
    {

        if (currentStateIndex >= 0)
            CurrentStateIndex = CurrentState.Update();
    }
}
