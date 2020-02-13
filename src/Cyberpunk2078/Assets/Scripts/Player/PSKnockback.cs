using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Knockback", menuName = "Player State/Knockback")]
public class PSKnockback : PlayerState
{
    [SerializeField] private float BlinkInterval= 0.02f;

    [HideInInspector] public Vector3 knockback;
    
    private Rigidbody2D rigidbody;

    private float defaultDrag;
    private float duration;
    private float t_duration = 0;
    private float t_blink;
    private int counter;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
        
        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override string Update()
    {
        if ((t_duration += TimeManager.Instance.ScaledDeltaTime) >= duration)
            return "Idle";


        for (; t_duration >= t_blink; t_blink += BlinkInterval)
            playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = (counter++ & 1) == 0 ? Color.white : Color.grey;


        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        anim.Play("MainCharacter_Hurt", -1, 0f);


        defaultDrag = rigidbody.drag;
        duration = Player.CurrentPlayer.knockBackDuration;
        t_duration = 0;
        t_blink = BlinkInterval;
        counter = 0;

        rigidbody.drag = 0;


        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.gray;
    }

    public override void OnStateQuit(State nextState)
    {
        rigidbody.drag = defaultDrag;


        playerCharacter.SpriteHolder.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
