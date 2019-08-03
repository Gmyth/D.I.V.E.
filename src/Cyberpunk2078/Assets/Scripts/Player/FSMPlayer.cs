using UnityEngine;


public abstract class PlayerState : State
{
    protected PlayerCharacter playerCharacter;
    


    public virtual void Initialize(int index, PlayerCharacter playerCharacter)
    {
        Index = index;
        this.playerCharacter = playerCharacter;
    }


    public abstract int Update();


    public virtual void OnStateEnter() { }
    //public virtual void OnStateReset() { }
    public virtual void OnStateQuit() { }

    //Player Ground check
    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,-0.2f,0f),-playerCharacter.transform.up,0.5f);
        RaycastHit2D hit1 = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.1f,-0.2f,0f),-playerCharacter.transform.up,0.5f);
        RaycastHit2D hit2 = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.1f,-0.2f,0f),-playerCharacter.transform.up,0.5f);
     
        if ((hit.collider != null && hit.transform.CompareTag("Ground") )||
            (hit1.collider != null && hit1.transform.CompareTag("Ground") )||
            (hit2.collider != null && hit2.transform.CompareTag("Ground"))
        )
        {
            return true;
        }

        return false;
    }

    // Function : Keyboard Input => Physics Velocity, And Friction Calculation
    public void PhysicsInputHelper(float h, float maxSpeed  = 9)
    {
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();     
        // calculate speed on X axis
        if (Mathf.Abs(h) > 0.1f){
            // has horizontal input 
            if (Mathf.Abs(rb2d.velocity.x) < maxSpeed)
            {
                var direction = Vector3.right * h * 20f;
                if (direction.x * rb2d.velocity.x < 0)
                {
                    direction = direction * 4f;
                }
                rb2d.AddForce(direction);
            }else{
                if (rb2d.velocity.x * h < 0)
                {
                    // not in the same direction
                    // reduce speed,friction
                    var direction = rb2d.velocity.normalized;
                    if (direction.x > 0) direction.x = 1;
                    else direction.x = -1;
                    rb2d.AddForce(new Vector2(-direction.x * 8f, 0f));
                }
            }
        }
        else{
            // does not has input
            // reduce speed,friction
            rb2d.AddForce(new Vector2(-rb2d.velocity.x * 4, 0f));
        }
        
//        if (rb2d.velocity.x > 0.2f)
//        {
//            GetComponent<SpriteRenderer>().flipX = false;
//            atkBoxGroup.transform.localScale = new Vector3(1, 1, 1);
//        }
//        else if(rb2d.velocity.x < -0.2f)
//        {
//            atkBoxGroup.transform.localScale = new Vector3(-1, 1, 1);
//            GetComponent<SpriteRenderer>().flipX = true;
//        }
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
            //    Debug.Log(LogUtility.MakeLogStringFormat("FSMPLayer", "Reset on state {0}", currentStateIndex));

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
