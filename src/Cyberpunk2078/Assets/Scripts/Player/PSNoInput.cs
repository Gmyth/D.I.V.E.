using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_No_Input", menuName = "Player State/No Input")]
public class PSNoInput : PlayerState
{
    //public TimelineManager currentTimeline;

    public override int Update()
    {
        if (isGrounded())
        {
            KillSpeed();
            Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
            if (isGrounded() && !playerCharacter.gameObject.GetComponent<DialoguePlayer>().CurrentTimelineManager.isPlayInstantly)
            {
                playerCharacter.gameObject.GetComponent<DialoguePlayer>().CurrentTimelineManager.OnTimelineStart();
                Debug.Log(LogUtility.MakeLogString("PSNoInput", "Timeline Start"));
            }
        }

        return Index;
    }


    public override void OnStateEnter(State previousState)
    {
        //KillSpeed();
        playerCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 3;
    }

    public override void OnStateQuit(State nextState)
    {
        playerCharacter.gameObject.GetComponent<Rigidbody2D>().simulated = true;
        playerCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 3;
        playerCharacter.gameObject.GetComponent<Rigidbody2D>().drag = 1;
    }

    public void KillSpeed()
    {
        Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
        rb2d.simulated = false;
    }
}
