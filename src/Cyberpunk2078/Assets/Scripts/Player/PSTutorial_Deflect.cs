using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "PS_Tutorial_Deflect", menuName = "Player State/Tutorial Deflect")]

public class PSTutorial_Deflect : PlayerState
{
    [SerializeField] private int indexPSNoInput;
    [SerializeField] private GameObject SplashFX;

    private float t = 0;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);
    }

    public override int Update()
    {
        if (t == 0)
        {
            Vector2 idealDirection = (SimpleTutorialManager.Instance.DeflectTutorial_Bullet.transform.position - PlayerCharacter.Singleton.transform.position).normalized;

            if (PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().DirectionNotification(idealDirection, 10f))
            {
                if (Input.GetButtonDown("Attack1") && !playerCharacter.isInTutorial)
                {
                    anim.Play("MainCharacter_Atk", -1, 0f);

                    var attack = Instantiate(SplashFX);
                    attack.transform.position = playerCharacter.transform.position;
                    attack.transform.right = PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().GetAttackDirection();
                    attack.transform.parent = playerCharacter.transform;
                    attack.GetComponentInChildren<HitBox>().hit.source = playerCharacter;

                    t = Time.unscaledTime + 0.5f;

                    SimpleTutorialManager.Instance.AfterDeflectTutorial();
                }
            }
        }
        else if (Time.unscaledTime > t)
            return indexPSNoInput;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        playerCharacter.isInTutorial = true;

        t = 0;
    }

    public override void OnStateQuit(State nextState)
    {
        anim.Play("MainCharacter_Idle", -1, 0f);
    }
}
