using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Knockback", menuName = "Player State/Knockback")]
public class PSKnockback : PlayerState
{
    [SerializeField] private float BlinkInterval= 0.02f;

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

    public override string Update()
    {
        if (Time.time >= t)
            return "Idle";
        if (t0 + BlinkInterval < Time.time)
        {
            playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = counter % 2 ==0 ?Color.gray:Color.white;
            counter++;
            t0 = Time.time;
        }

        return Name;
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
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.gray;
        counter = 0;
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.white;
    }
    
}
