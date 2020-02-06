using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Knockback", menuName = "Player State/Knockback")]
public class PSKnockback : PlayerState
{
    [SerializeField] private float BlinkInterval= 0.02f;

    [HideInInspector] public Vector3 knockback;
    
    private Rigidbody2D rigidbody;

    private float duration;
    private float t_finish;
    private float t_blink;
    private int counter;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
        
        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override string Update()
    {
        float t = Time.time;


        if (t >= t_finish)
            return "Idle";


        while (t >= t_blink)
        {
            playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = (counter & 1) == 0 ? Color.white : Color.grey;


            t_blink += BlinkInterval;
            counter++;
        }


        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        anim.Play("MainCharacter_Hurt", -1, 0f);


        duration = Player.CurrentPlayer.knockBackDuration;
        t_finish = Time.time + duration;
        t_blink = Time.time + BlinkInterval;
        counter = 0;


        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
