using System.Collections;
using UnityEngine;

public enum CharacterState
{
    Idle,
    Moveable,
    Jump,
    Fall,
    Slide,
    Dashing,
    UpAttack,
    RightAttack,
    DownAttack
}

public class Character : MonoBehaviour
{

    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private BoxCollider2D dashAtkBox;
    [SerializeField] private BoxCollider2D upAtkBox;
    [SerializeField] private BoxCollider2D rightAtkBox;
    [SerializeField] private BoxCollider2D downAtkBox;
    [SerializeField] private GameObject atkBoxGroup;
    [SerializeField] private BoxCollider2D hitBox;
    
    
    [SerializeField] private GameObject shield;
    [SerializeField] private BoxCollider2D blockBox;
    private bool Grounded;
    private bool SecondJumpReady;

    private bool NotOkForInput;
    private bool blocking;

    private int attackLevel = 1;
    private CharacterState currentState;
    
    private CharacterState previousState;
    private Energy energy;
    private Vector2 currentPressDirection;
    private Animator anim;
    private Rigidbody2D rb2d;
    private MouseIndicator mouse;
    private bool idle;
    public CharacterState CurrentState
    {
        // this allowed to triggger codes when the state switched
        get
        {
            return currentState;
        }

        private set
        {
            if (value == currentState)
            {
                Debug.Log("Player Reset " + value);
            }
            else
            {
                Debug.Log("Player change to " + value);
                previousState = currentState;
                currentState = value;
                switch (currentState)
                {
                    case CharacterState.Idle:
                        //run when playerState transfered to Run
                        break;
                    
                    case CharacterState.Moveable:
                        //run when playerState transfered to Run
                        if(Mathf.Abs(rb2d.velocity.x) < 0.2f)anim.Play("Character",0);
                        else anim.Play("Character_Run");
                        break;
                    
                    case CharacterState.Jump:
                        if (SecondJumpReady)
                        {
                            anim.Play("Character_Jump", -1, 0f);
                        }
                        else
                        {
                            anim.Play("Character_Roll", -1, 0f);
                        }
                        break;
                    
                    case CharacterState.Fall:
                        anim.Play("Character_Fall", -1, 0f);
                        break;
                    
                    case CharacterState.Slide:
                        anim.Play("Character_Slide", -1, 0f);
                        break;
                    case CharacterState.Dashing:
                        anim.Play("Character_DashAtk", -1, 0f);
                        if (mouse.getAttackDirection().x < 0)
                        {
                            transform.right = -mouse.getAttackDirection();
                            GetComponent<SpriteRenderer>().flipX = true;

                        }
                        else
                        {
                            transform.right = mouse.getAttackDirection(); 
                            GetComponent<SpriteRenderer>().flipX = false;
                        }
                        
                        break;
                    case CharacterState.UpAttack:
                        anim.Play("Character_UpAtk", -1, 0f);
                        break;
                    case CharacterState.RightAttack:
                        anim.Play("Character_RightAtk", -1, 0f);
                        break;
                    case CharacterState.DownAttack:
                        anim.Play("Character_DownAtk", -1, 0f);
                        break;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        energy = GameObject.FindObjectOfType<Energy>();
        SecondJumpReady = true;
        idle = false;
        blocking = false;
        blockBox.enabled = false;
        dashAtkBox.enabled = false;
        upAtkBox.enabled = false;
        rightAtkBox.enabled = false;
        downAtkBox.enabled = false;
        shield.SetActive(false);
        mouse = GetComponentInChildren<MouseIndicator>();
        anim = gameObject.GetComponent<Animator>();
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        setPlayerState(CharacterState.Moveable);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        groundCheck();
        switch (currentState) {
            case CharacterState.Moveable:
                // press direction can be different with  movement
                currentPressDirection = new Vector2(h, v);
                if(!NotOkForInput)physicsInputHelper(h);
                if (Mathf.Abs(h) < 0.3f && !idle)
                {
                    anim.Play("Character");
                    idle = true;
                }else if(Mathf.Abs(h) > 0.3f && idle){
                    anim.Play("Character_Run");
                    idle = false;
                }
                break;
            case CharacterState.Jump:
                if(!NotOkForInput)physicsInputHelper(h);
                if (!Grounded && rb2d.velocity.y < 0)
                {
                    setPlayerState(CharacterState.Fall);
                }else if (Grounded && rb2d.velocity.y < 0)
                {
                    setPlayerState(CharacterState.Moveable);
                }
                break;
                
            case CharacterState.Fall:
                if(!NotOkForInput)physicsInputHelper(h);
                if (Grounded)
                {
                   setPlayerState(CharacterState.Moveable);
                }
                break;
        } 
    }

    void Update()
    {
        if (blocking)
        {
            if (energy.EnergyAmount < 10)
            {
                blocking = false;
            }
            energy.changeEnergyAmount(-40 * Time.deltaTime);
        }else if (energy.EnergyAmount < 200)
        {
            energy.EnergyAmount = Mathf.Min(energy.EnergyAmount + 28* Time.deltaTime,200);
        }
        else{
            energy.EnergyAmount = Mathf.Max(energy.EnergyAmount - 3* Time.deltaTime,200);
        }

        if (!NotOkForInput)
        {
            if (Input.GetButtonDown("Jump")) {
                //boosting                    
                if (Grounded)
                {
                    rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
                    setPlayerState(CharacterState.Jump);
                    rb2d.AddForce(transform.up * jumpForce);
                    Grounded = false;
                }else if(SecondJumpReady)
                {
                    SecondJumpReady = false;
                    rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
                    anim.Play("Character_Jump",0);
                    rb2d.AddForce(transform.up * jumpForce);
                    setPlayerState(CharacterState.Jump);
                    Grounded = false;
                }
            }

            if(Input.GetButtonDown("Dash") && energy.EnergyAmount > 100)
            {
                // Dash
                dashAtkBox.enabled = false;
                attackLevel = (int)energy.CurrentEnergyLevel;
                Vector3 direction = mouse.getAttackDirection();
                rb2d.velocity = new Vector2(0,0);
                if (energy.CurrentEnergyLevel == EnergyLevel.OverCharged)
                {
                    rb2d.AddForce(direction * dashForce * 1.3f);
                    setPlayerState(CharacterState.Dashing);
                    hitBox.enabled = false;
                    SecondJumpReady = true;
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.22f));
                }else if (energy.CurrentEnergyLevel == EnergyLevel.Normal)
                {
                    rb2d.AddForce(direction * dashForce * 1.2f);
                    setPlayerState(CharacterState.Dashing);
                    hitBox.enabled = false;
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.18f));
                }
                else
                {
                    rb2d.AddForce(direction * dashForce);
                    setPlayerState(CharacterState.Dashing);
                    hitBox.enabled = false;
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.15f)); 
                }
                energy.changeEnergyAmount(-100);
            }

            if (Input.GetButtonDown("Attack") && energy.EnergyAmount > 40)
            {
                attackLevel = (int)energy.CurrentEnergyLevel;
                energy.changeEnergyAmount(-40);
                Vector2 dir = mouse.getAttackDirection();
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (angle < 135 && angle >= 45)
                {
                    //up
                    setPlayerState(CharacterState.UpAttack);
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.2f));
                }
                else if(angle < 45 && angle >= -45)
                {
                    setPlayerState(CharacterState.RightAttack);
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.2f));
                }else if (angle < -45 && angle > -135)
                {
                    setPlayerState(CharacterState.DownAttack);
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.2f));
                }
                else
                {
                    atkBoxGroup.transform.localScale = new Vector3(-1, 1, 1);
                    GetComponent<SpriteRenderer>().flipX = true;
                    setPlayerState(CharacterState.RightAttack);
                    NotOkForInput = true;
                    StartCoroutine(DashRelease(0.2f));
                }
            }

            if (Input.GetMouseButton(1) && energy.EnergyAmount > 10)
            {
                if(!blocking)energy.changeEnergyAmount(-10);
                blocking = true;
                hitBox.enabled = false;
                shield.SetActive(true);
                blockBox.enabled = true;
                StartCoroutine(BlockRelease(0.01f));
            }
            else
            {
                blocking = false;
            }
        }
    }

    private void physicsInputHelper(float h)
    {
              
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

        if (rb2d.velocity.x > 0.2f)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            atkBoxGroup.transform.localScale = new Vector3(1, 1, 1);
        }
        else if(rb2d.velocity.x < -0.2f)
        {
            atkBoxGroup.transform.localScale = new Vector3(-1, 1, 1);
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void addEnergy(float amount)
    {
        energy.changeEnergyAmount(amount);
    }

    public void setPlayerState(CharacterState targetState) { CurrentState = targetState; }

    public void groundCheck(bool resetSecond = true)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0f,-0.2f,0f),-transform.up,0.5f);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0.1f,-0.2f,0f),-transform.up,0.5f);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(-0.1f,-0.2f,0f),-transform.up,0.5f);
        Debug.DrawRay(transform.position + new Vector3(0f,-0.2f,0f), -transform.up * 0.5f, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0.1f,-0.2f,0f), -transform.up * 0.5f, Color.yellow);
        Debug.DrawRay(transform.position + new Vector3(-0.1f,-0.2f,0f), -transform.up * 0.5f, Color.green);
        
        if ((hit.collider != null && hit.transform.CompareTag("Ground") )||
            hit1.collider != null && hit1.transform.CompareTag("Ground") ||
            hit2.collider != null && hit2.transform.CompareTag("Ground")
            )
        {
            if (Grounded == false && resetSecond)
            {
                //back to surface
                SecondJumpReady = true;
            }
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
    }

    public void enableDashAtkBox()
    {
        dashAtkBox.enabled = true;
    }

    public void disableDashAtkBox()
    {
        dashAtkBox.enabled = false;
    }
    
    public void enableUpAtkBox()
    {
        upAtkBox.enabled = true;
    }

    public void disableUpAtkBox()
    {
        upAtkBox.enabled = false;
    }
    
    public void enableDownAtkBox()
    {
        downAtkBox.enabled = true;
    }

    public void disableDownAtkBox()
    {
        downAtkBox.enabled = false;
    }
    
    public void enableRightAtkBox()
    {
        rightAtkBox.enabled = true;
    }

    public void disableRightAtkBox()
    {
        rightAtkBox.enabled = false;
    }
    
    private IEnumerator BlockRelease(float time)
    {
        while (blocking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(time);
        shield.SetActive(false);
        blockBox.enabled = false;
        hitBox.enabled = true;
    }

    private IEnumerator DashRelease(float time)
    {
        yield return new WaitForSeconds(time);
        hitBox.enabled = true;
        //dashAtkBox.enabled = false;
        Grounded = false;
        groundCheck(false);
        if (Grounded){
            setPlayerState(CharacterState.Moveable);
        }else{
            setPlayerState(CharacterState.Fall);
        }
        transform.right = Vector3.right;
        rb2d.velocity = rb2d.velocity * 0.4f;
        NotOkForInput = false;
    }
}
