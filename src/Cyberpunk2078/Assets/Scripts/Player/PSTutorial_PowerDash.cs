﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_Tutorial_PowerDash", menuName = "Player State/Tutorial PowerDash")]
public class PSTutorial_PowerDash : PlayerState
{
    private Rigidbody2D rb2d;
    private Animator animator;

    private SpriteRenderer sprite;

    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);

        rb2d = playerCharacter.gameObject.GetComponent<Rigidbody2D>();
        animator = playerCharacter.gameObject.GetComponentInChildren<Animator>();

        sprite = playerCharacter.gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public override string Update()
    {

        if (animator.isActiveAndEnabled)
        {
            animator.Play("MainCharacter_Jump", -1, 0f);
        }

        Vector2 idealDirection = PlayerCharacter.Singleton.transform.right.normalized;
        if (PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().DirectionNotification(idealDirection, 15f))
        {
            if (Input.GetKeyDown(KeyCode.R) && !playerCharacter.isInTutorial)
            {
                Player.CurrentPlayer.triggerReady = false;
                PlayerCharacter.Singleton.PowerDash = true;
                return "Dashing";
            }
        }

        return Name;
    }

    public override void OnStateEnter(State previousState)
    {
        rb2d.gravityScale = 0;

        animator.Play("MainCharacter_Jump", -1, 0f);

        playerCharacter.isInTutorial = true;

        sprite.enabled = false;
    }

    public override void OnStateQuit(State nextState)
    {
        //animator.gameObject.SetActive(true);
        sprite.enabled = true;

        rb2d.gravityScale = PlayerCharacter.Singleton.DefaultGravity;
        PowerDashTutorial.Instance.AfterPowerDashTutorial();
    }

}
