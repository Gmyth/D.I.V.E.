using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_No_Input", menuName = "Player State/No Input")]
public class PSNoInput : PlayerState
{
    [SerializeField] private float maxDuration = float.MaxValue;
    //public TimelineManager currentTimeline;
    private float t_duration = 0;


    public override string Update()
    {
        t_duration += TimeManager.Instance.ScaledDeltaTime;

        if (t_duration >= maxDuration)
            return "Idle";

        //if (isGrounded())
        //{
        //    KillSpeed();
        //    Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        //    if (isGrounded() && !playerCharacter.gameObject.GetComponent<DialoguePlayer>().CurrentTimelineManager.isPlayInstantly)
        //    {
        //        playerCharacter.gameObject.GetComponent<DialoguePlayer>().CurrentTimelineManager.OnTimelineStart();
        //        Debug.Log(LogUtility.MakeLogString("PSNoInput", "Timeline Start"));
        //    }
        //}

        return Name;
    }


    public override void OnStateEnter(State previousState)
    {
        //anim.Play("MainCharacter_Idle", -1, 0f);
        //KillSpeed();
        //playerCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 3;
    }

    public override void OnStateQuit(State nextState)
    {
        t_duration = 0;


        //playerCharacter.gameObject.GetComponent<Rigidbody2D>().simulated = true;
        //playerCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 3;
        //playerCharacter.gameObject.GetComponent<Rigidbody2D>().drag = 1;
        anim.Play("MainCharacter_Idle", -1, 0f);
    }

    public void KillSpeed()
    {
        Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
        //rb2d.simulated = false;
    }

}
