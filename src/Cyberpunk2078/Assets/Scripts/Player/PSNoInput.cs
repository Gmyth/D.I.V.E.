using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_No_Input", menuName = "Player State/No Input")]
public class PSNoInput : PlayerState
{
    public TimelineManager currentTimeline;

    public override int Update()
    {
        if (isGrounded())
        {
            Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
            if (rb2d.velocity == Vector2.zero)
            {
                //currentTimeline.OnTimelineStart();
                Debug.Log(LogUtility.MakeLogString("PSNoInput", "Timeline Start"));
            }
        }

        return Index;
    }


    public override void OnStateEnter(State previousState)
    {
        //kill all speed
        Rigidbody2D rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
    }
}
