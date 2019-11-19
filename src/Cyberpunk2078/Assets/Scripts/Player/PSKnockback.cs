using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Knockback", menuName = "Player State/Knockback")]
public class PSKnockback : PlayerState
{
    [SerializeField] private float BlinkInterval= 0.02f;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_Idle;
    [SerializeField] private int stateIndex_Moving;
    [SerializeField] private int stateIndex_jumping;
    [SerializeField] private int stateIndex_Dashing;

    [HideInInspector] public Vector3 knockback;
    
    private Rigidbody2D rigidbody;

    private float duration;
    private float t;
    private float t0;
    private int counter;
    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
        
        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override int Update()
    {
        if (Time.time >= t)
            return stateIndex_Idle;
        if (t0 + BlinkInterval < Time.time)
        {
            playerCharacter.GetComponent<SpriteRenderer>().color = counter % 2 ==0 ?Color.gray:Color.white;
            counter++;
            t0 = Time.time;
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        //kill velocity
        var rb2d = playerCharacter.GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;

        anim.Play("MainCharacter_Hurt", -1, 0f);
        duration = Player.CurrentPlayer.knockBackDuration;
        t = Time.time + duration;
        t0 = Time.time;
        playerCharacter.GetComponent<SpriteRenderer>().color = Color.gray;
        counter = 0;
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
}
