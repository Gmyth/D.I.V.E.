using UnityEngine;

public enum Direction
{
    None,
    Left,
    Right
}

public abstract class PlayerState : State
{
    public string Name;
    protected PlayerCharacter playerCharacter;
    protected Animator anim;
    protected bool flip;
    protected bool grounded;

    public virtual void Initialize(int index, PlayerCharacter playerCharacter)
    {
        Index = index;
        this.playerCharacter = playerCharacter;
        Player.CreatePlayer();
        anim = playerCharacter.GetComponent<Animator>();
    }


    //Player Ground check
    public bool isGrounded()
    {
        RaycastHit2D hitM = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,-0.7f,0f),-playerCharacter.transform.up, 0.6f);
        RaycastHit2D hitL = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.12f,-0.7f,0f),-playerCharacter.transform.up, 0.6f);
        RaycastHit2D hitR = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.12f,-0.7f,0f),-playerCharacter.transform.up, 0.6f);

        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0f, -0.7f, 0f), -playerCharacter.transform.up * 0.6f, Color.red);

        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0.12f, -0.7f, 0f), -playerCharacter.transform.up * 0.6f, Color.green);

        Debug.DrawRay(playerCharacter.transform.position + new Vector3(-0.12f, -0.7f, 0f), -playerCharacter.transform.up * 0.6f, Color.yellow);

        if (hitM.collider != null && hitM.transform.CompareTag("Ground") || hitR.collider != null && hitR.transform.CompareTag("Ground") || hitL.collider != null && hitL.transform.CompareTag("Ground"))
        {
            
            Player.CurrentPlayer.secondJumpReady = true;
            grounded = true;
            return true;
        }else if(hitM.collider != null && hitM.transform.CompareTag("Platform") || hitR.collider != null && hitR.transform.CompareTag("Platform") || hitL.collider != null && hitL.transform.CompareTag("Platform"))
        {
            
            Player.CurrentPlayer.secondJumpReady = true;
            grounded = true;
            return true;
        }
        Player.CurrentPlayer.jumpForceGate = false;
        grounded = false;
        return false;
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
    public Direction isCloseTo(string tag, string layerMask = "")
    {
        RaycastHit2D hit = layerMask.Length > 0 ? 
                Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(0.1f,0f,0f),playerCharacter.transform.right,0.35f,1 << LayerMask.NameToLayer(layerMask))
                : Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(0.1f,0f,0f),playerCharacter.transform.right,0.35f);
        RaycastHit2D hit1 = layerMask.Length > 0 ? 
                Physics2D.Raycast(playerCharacter.transform.position+ new Vector3(-0.1f,0f,0f),-playerCharacter.transform.right,0.35f, 1 << LayerMask.NameToLayer(layerMask))
                : Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.1f,0f,0f),-playerCharacter.transform.right,0.35f);

        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0f,-0f,0f), playerCharacter.transform.right * 0.35f, Color.red);
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0.1f,-0f,0f), -playerCharacter.transform.right * 0.35f, Color.yellow);

        if (hit.collider != null && hit.transform.CompareTag(tag))
        {
            return Direction.Right;
            
        }else if (hit1.collider != null && hit1.transform.CompareTag(tag))
        {
            
            return Direction.Left;
        }

        return Direction.None;
    }


    //if player is grounded, the direction to down left/right & down side are not allowed
    public Vector2 getDirectionCorrection(Vector2 _direction, Vector2 _norm)
    {
        if (Vector2.Angle(_direction, _norm) < 85)
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
                if (rb2d.velocity.x * h < 0 && grounded)
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
            //if (grounded) rb2d.AddForce(new Vector2(-rb2d.velocity.x * 4, 0f));
            if (grounded) rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
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
                if (value < 0)
                {
                    currentStateIndex = -1;
                    return;
                }
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat("FSMPLayer", "Make transition from {0} to {1}", states[currentStateIndex].name, states[value].name));
#endif

                int previousStateIndex = currentStateIndex;

                CurrentState.OnStateQuit(states[value]);

                currentStateIndex = value;

                CurrentState.OnStateEnter(states[previousStateIndex]);

                OnCurrentStateChange.Invoke(currentStateIndex, previousStateIndex);
            }
        }
    }


    public FSMPlayer Initialize(PlayerCharacter player)
    {
        FSMPlayer fsmCopy = Instantiate(this);

        for (int i = 0; i < states.Length; ++i)
        {
            PlayerState stateCopy = Instantiate(states[i]);
            stateCopy.Initialize(i, player);

            fsmCopy.states[i] = stateCopy;
        }

        fsmCopy.currentStateIndex = -1;


        return fsmCopy;
    }

    public override void Boot()
    {
        OnMachineBoot();

        currentStateIndex = startingStateIndex;

        CurrentState.OnStateEnter(CurrentState);

        OnCurrentStateChange.Invoke(currentStateIndex, -1);
    }

    public override void ShutDown()
    {
        int previousStateIndex = currentStateIndex;

        CurrentState.OnStateQuit(null);

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
