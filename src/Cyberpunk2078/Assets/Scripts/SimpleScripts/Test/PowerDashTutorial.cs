using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDashTutorial : Singleton<PowerDashTutorial>
{

    [SerializeField] private Transform tutorialEndPoint;

    [SerializeField] private GameObject tutorialSpriteGO;

    private GameObject playerSpriteGO;

    /// <summary> ------------------------------------------------------------------------------------------------
    /// Tutorial: Power Dash
    /// </summary>
    private TimelineManager timelineManager_PowerDashTutorial;
    public void IntroducePowerDash(TimelineManager timelineManager)
    {

        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "InTutorial_PowerDash";

        // playerSpriteGO = PlayerCharacter.Singleton.gameObject.GetComponentsInChildren<SpriteRenderer>()[0].gameObject;
        playerSpriteGO = PlayerCharacter.Singleton.transform.parent.gameObject;
        playerSpriteGO.transform.position = tutorialEndPoint.position;

        playerSpriteGO.SetActive(false);
        tutorialSpriteGO.SetActive(true);
        CameraManager.Instance.ChangeTarget(tutorialSpriteGO);


        timelineManager_PowerDashTutorial = timelineManager;
        timelineManager_PowerDashTutorial.PlayTimelineInIndex(0);


        //StartCoroutine(ShowPlayerSpriteAfterDelay(3.5f));
        //StartCoroutine(ShowGUIButtonAfterDelay(3.5f, "LongDash"));
    }

    public void AfterPowerDashTutorial()
    {
        tutorialSpriteGO?.SetActive(false);

        CameraManager.Instance.ResetTarget();
        CameraManager.Instance.Idle();
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        //TimeManager.Instance.endSlowMotion(0f);

        GUITutorial.Singleton.Hide();


        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }

    public void ShowPlayerSprite()
    {
        PlayerCharacter.Singleton.gameObject.GetComponentInChildren<SpriteRenderer>();
        playerSpriteGO?.SetActive(true);
        //tutorialSpriteGO?.SetActive(false);
        CameraManager.Instance.Reset();

        StartCoroutine(ShowGUIButtonAfterDelay(0.1f, "LongDash"));
    }

    private IEnumerator ShowPlayerSpriteAfterDelay(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        playerSpriteGO.SetActive(true);
        tutorialSpriteGO.SetActive(false);
        CameraManager.Instance.Reset();
    }

    private IEnumerator ShowGUIButtonAfterDelay(float delayTime, string buttonName)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        GUITutorial.Singleton.Show(buttonName);

        yield return new WaitForSecondsRealtime(0.5f);
        PlayerCharacter.Singleton.isInTutorial = false;
    }

}
