using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_Tutorial_Dash", menuName = "Player State/Tutorial Dash")]

public class PSTutorial_Dash : PlayerState
{
    [SerializeField] private int indexPSDashing;

    private Rigidbody2D rb2d;
    private Animator animator;

    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);

        rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        animator = playerCharacter.gameObject.GetComponentInChildren<Animator>();
    }

    public override int Update()
    {
        Vector2 idealDirection = (SimpleTutorialManager.Instance.DashTutorial_Drone.transform.position - PlayerCharacter.Singleton.transform.position).normalized;
        if (PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().DirectionNotification(idealDirection, 15f))
        {
            if ((Input.GetButtonDown("Dashing") || (Input.GetAxis("Trigger") > 0 && Player.CurrentPlayer.triggerReady)) && !playerCharacter.isInTutorial)
            {
                Player.CurrentPlayer.triggerReady = false;
                return indexPSDashing;
            }
        }

        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        //animator.speed = 0;
        animator.Play("MainCharacter_Airborne");

        //rb2d.simulated = false;
        //rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        //rb2d.velocity = Vector2.zero;

        //rb2d.gravityScale = 0;

        playerCharacter.isInTutorial = true;
    }

    public override void OnStateQuit(State nextState)
    {
        //animator.speed = 1;

        //rb2d.simulated = true;
        //rb2d.gravityScale = 3;
        //rb2d.drag = 1;
        TimeManager.Instance.endSlowMotion();
        SimpleTutorialManager.Instance.AfterDashTutorial();
    }

}
