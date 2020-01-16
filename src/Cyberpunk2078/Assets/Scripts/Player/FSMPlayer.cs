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
    protected float lastGroundedSec;

    public virtual void Initialize(int index, PlayerCharacter playerCharacter)
    {
        Index = index;
        this.playerCharacter = playerCharacter;

        anim = playerCharacter.SpriteHolder.GetComponent<Animator>();
    }


    // Player Ground check
    // @return 0 - airborne   1 - ground   2 - enemy
    public int GetGroundType()
    {
        Vector2 slideCheck = playerCharacter.GetComponent<Rigidbody2D>().velocity.x > 0
            ? new Vector2(0.5f, -0.5f)
            : new Vector2(-0.5f, -0.5f);
        
        // this variable will reposition the ray start point 
        float centerOffset = -0.7f;

        float DistanceToTheGround = playerCharacter.GetComponent<CapsuleCollider2D>().bounds.extents.y + centerOffset;
        //float DistanceToTheGround = centerOffset;
        RaycastHit2D hitM = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,centerOffset,0f),-playerCharacter.transform.up, DistanceToTheGround + 0.4f  );
        RaycastHit2D hitL = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.3f,centerOffset,0f),-playerCharacter.transform.up, DistanceToTheGround + 0.4f );
        RaycastHit2D hitR = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.3f,centerOffset,0f),-playerCharacter.transform.up, DistanceToTheGround + 0.4f);
        RaycastHit2D hitSlide = Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0f,0f,0f),slideCheck, 2f );

        
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0f, centerOffset, 0f), -playerCharacter.transform.up * (DistanceToTheGround + 0.4f), Color.red);
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(0.3f, centerOffset, 0f), -playerCharacter.transform.up * (DistanceToTheGround + 0.4f), Color.green);
        Debug.DrawRay(playerCharacter.transform.position + new Vector3(-0.3f, centerOffset, 0f), -playerCharacter.transform.up * (DistanceToTheGround + 0.4f), Color.yellow);
        Debug.DrawRay(playerCharacter.transform.position, slideCheck * 2f, Color.green);

        bool margin = !(hitSlide.collider != null && (hitSlide.transform.CompareTag("Ground") || hitSlide.transform.CompareTag("Platform") && hitSlide.normal.x > 0.1f));

        Player player = Player.CurrentPlayer;


        if ((hitL.collider != null && (hitL.transform.CompareTag("Ground") || hitL.transform.CompareTag("Platform"))) ||
            (hitR.collider != null && (hitR.transform.CompareTag("Ground") || hitR.transform.CompareTag("Platform"))) ||
            (hitM.collider != null && (hitM.transform.CompareTag("Ground") || hitM.transform.CompareTag("Platform"))))
        {
            //Character Margin
            if (margin && !grounded)
            {

                //if (name == "Airborne")
                //{
                //   playerCharacter.transform.Translate(Vector2.up * (hitM.distance - DistanceToTheGround));
                //}
                //else
                //{
                //   playerCharacter.transform.Translate(Vector2.down* (hitM.distance - DistanceToTheGround));
                //}
            }

            player.ChainWallJumpReady = false;
            player.secondJumpReady = true;

            grounded = true;

            return 1;
        } 
        else if ((hitL.collider != null && hitL.transform.CompareTag("Dummy")) ||
                 (hitR.collider != null && hitR.transform.CompareTag("Dummy")) ||
                 (hitM.collider != null && hitM.transform.CompareTag("Dummy")))
        {
            return 2;
        }


        player.jumpForceGate = false;

        grounded = false;


        return 0;
    }

    //Player Ground Normal Vector, Used for Dash direction correction
    public Vector2 GroundNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCharacter.transform.position,-playerCharacter.transform.up,1.2f);

        if (hit.collider != null && hit.transform.CompareTag("Ground") ) {
            return hit.normal;
        }

        return Vector2.zero;
    }

    //Check player is close to wall
    public Direction isCloseTo(string tag, string layerMask = "")
    {
        RaycastHit2D hit = layerMask.Length > 0 ? 
                Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.3f,0f,0f),playerCharacter.transform.right,0.8f,1 << LayerMask.NameToLayer(layerMask))
                : Physics2D.Raycast(playerCharacter.transform.position + new Vector3(-0.3f,0f,0f),playerCharacter.transform.right,0.8f);
        RaycastHit2D hit1 = layerMask.Length > 0 ? 
                Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.3f,0f,0f),-playerCharacter.transform.right,0.8f, 1 << LayerMask.NameToLayer(layerMask))
                : Physics2D.Raycast(playerCharacter.transform.position + new Vector3(0.3f,0f,0f),-playerCharacter.transform.right,0.8f);

        Debug.DrawRay(playerCharacter.transform.position, playerCharacter.transform.right * 0.5f, Color.green);
        Debug.DrawRay(playerCharacter.transform.position, -playerCharacter.transform.right * 0.5f, Color.green);
        
        if (hit.collider != null && hit.transform.CompareTag(tag))
        {
            return Direction.Right;
            
        }else if (hit1.collider != null && hit1.transform.CompareTag(tag))
        {
            return Direction.Left;
        }
        return Direction.None;
    }
    
    public Vector2 LadderMargin(string tag = "Ladder", string layerMask = "")
    {
        RaycastHit2D hit = layerMask.Length > 0 ? 
            Physics2D.Raycast(playerCharacter.transform.position +  new Vector3(-0.3f,0f,0f),playerCharacter.transform.right,1f,1 << LayerMask.NameToLayer(layerMask))
            : Physics2D.Raycast(playerCharacter.transform.position +  new Vector3(-0.3f,0f,0f),playerCharacter.transform.right,1f);
        RaycastHit2D hit1 = layerMask.Length > 0 ? 
            Physics2D.Raycast(playerCharacter.transform.position +  new Vector3(0.3f,0f,0f),-playerCharacter.transform.right,1f, 1 << LayerMask.NameToLayer(layerMask))
            : Physics2D.Raycast(playerCharacter.transform.position +  new Vector3(0.3f,0f,0f),-playerCharacter.transform.right,1f);

        Debug.DrawRay(playerCharacter.transform.position, playerCharacter.transform.right * 0.8f, Color.red);
        Debug.DrawRay(playerCharacter.transform.position, -playerCharacter.transform.right * 0.8f, Color.yellow);

        float height = 0;
        float center = 0;
        if (hit.collider != null && hit.collider.transform.CompareTag(tag))
        {
            playerCharacter.transform.position = new Vector3(hit.transform.position.x,  playerCharacter.transform.position.y,  playerCharacter.transform.position.z);
            height = hit.collider.bounds.extents.y;
            center = hit.transform.position.y;
        }else if (hit1.collider != null && hit1.collider.transform.CompareTag(tag))
        {
            playerCharacter.transform.position = new Vector3(hit1.transform.position.x,  playerCharacter.transform.position.y,  playerCharacter.transform.position.z);
            height = hit1.collider.bounds.extents.y;
            center = hit1.transform.position.y;
        }

        if (height > 0)
        {
            return new Vector2(center - height,center + height);
        }
        return Vector2.zero;
    }

    protected void Fire()
    {
        LinearMovement bullet = ObjectRecycler.Singleton.GetObject<LinearMovement>(0);
        bullet.speed = 20;
        bullet.initialPosition = playerCharacter.transform.position;
        bullet.orientation = playerCharacter.transform.parent.GetComponentInChildren<MouseIndicator>().GetAttackDirection();
        
        bullet.GetComponent<Bullet>().speed = 20;
        bullet.GetComponent<Bullet>().isFriendly = true;
        bullet.transform.right = bullet.orientation;

        bullet.gameObject.SetActive(true);
    }
    
    //if player is grounded, the direction to down left/right & down side are not allowed



    // Function : Keyboard Input => Physics Velocity, And Friction Calculation
    public void PhysicsInputHelper(float h, float maxSpeed  = 9,  float Acceleration  = 25)
    {
        float feverFactor = playerCharacter.InFever ? Player.CurrentPlayer.FeverFactor : 1;
        Rigidbody2D rb2d = playerCharacter.GetComponent<Rigidbody2D>();
       
        // calculate speed on X axis
        if (Mathf.Abs(h) > 0.1f)
        {
            // has horizontal input
            if (Mathf.Abs(rb2d.velocity.x) < maxSpeed * feverFactor)
            {
                var direction = Vector3.right * h * Acceleration * feverFactor;
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
    }

    public void EarlyUpdate()
    {

        if (Input.GetAxis("Trigger") <= 0)
        {
             Player.CurrentPlayer.triggerReady = true;
        }
        
        if (Input.GetAxis("VerticalJoyStick") <= 0)
        {
            Player.CurrentPlayer.climbReady = true;
        }
        
        if (Input.GetAxis("Vertical") <= 0)
        {
            Player.CurrentPlayer.climbReady = true;
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

        foreach (PlayerState state in states)
            state.OnMachineBoot();


        currentStateIndex = startingStateIndex;

        CurrentState.OnStateEnter(null);

        OnCurrentStateChange.Invoke(currentStateIndex, -1);
    }

    public override void Reboot()
    {
        ShutDown();
        Boot();
    }

    public override void ShutDown()
    {
        int previousStateIndex = currentStateIndex;


        CurrentState.OnStateQuit(null);

        currentStateIndex = -1;

        OnCurrentStateChange.Invoke(-1, previousStateIndex);


        foreach (PlayerState state in states)
            state.OnMachineShutDown();

        OnMachineShutDown();
    }

    public void Update()
    {
        if (currentStateIndex >= 0)
        {
            CurrentState.EarlyUpdate();
            CurrentStateIndex = CurrentState.Update();
        }
    }
}
