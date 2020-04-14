using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum TutorialState
{
    Null,
    TeachDash,
    TeachDoubleDash,

}

public class SimpleTutorialManager : Singleton<SimpleTutorialManager>
{

    public TutorialState CurrentState
    {
        set
        {
            switch (value)
            {
                case TutorialState.TeachDash:
                    //IntroduceNormalEnergy();
                    Time.timeScale = 0;

                    if (MouseIndicator.Singleton.CurrentInputType == InputType.Joystick) UI_DashKey_Joy.SetActive(true);
                    else UI_DashKey_Keyboard.SetActive(true);
                    break;

                case TutorialState.TeachDoubleDash:
                    IntroduceOverloadEnergy();
                    Time.timeScale = 0;
                    break;

                default:
                    Time.timeScale = 1;
                    break;

            }

            CurrentState = value;
        }

        get { return CurrentState; }
    }

    [SerializeField] private GameObject c1;
    [SerializeField] private GameObject c2;
    [SerializeField] private GameObject c3;
    [SerializeField] private GameObject c4;
    [SerializeField] private GameObject UI_DashKey_Joy;
    [SerializeField] private GameObject UI_DashKey_Keyboard;

    [SerializeField] private GameObject UI_AttackKey_Joy;
    [SerializeField] private GameObject UI_AttackKey_Keyboard;

    [SerializeField] private GameObject UI_PowerDashKey_Joy;
    [SerializeField] private GameObject UI_PowerDashKey_Keyboard;

    public GameObject UI_BlackMask;

    public GameObject UI_TeachDash_DirectionMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PickUpCollectible(int index) {
        switch (index) {
            case 1:
                c1.GetComponent<Image>().color = Color.white;
                break;
            case 2:
                c2.GetComponent<Image>().color = Color.white;
                break;
            case 3:
                c3.GetComponent<Image>().color = Color.white;
                break;
            case 4:
                c4.GetComponent<Image>().color = Color.white;
                break;
        }

    }


    public void SetTutorialState(int stateIndex)
    {
        CurrentState = (TutorialState)stateIndex;
    }

    public void DeleteEnergySystem()
    {
        Player.CurrentPlayer.energyLocked = true;
        Player.CurrentPlayer.overloadEnergyLocked = true;
        PlayerCharacter.Singleton.ConsumeEnergy(1);
    }

    public void ShowAttackNotification() {
        if (MouseIndicator.Singleton.CurrentInputType == InputType.Joystick) UI_AttackKey_Joy.SetActive(true);
        else UI_AttackKey_Keyboard.SetActive(true);
    }

    /// <summary> ------------------------------------------------------------------------------------------------
    /// Tutorial: Dash
    /// </summary>
    private TimelineManager timelineManager_DashTutorial;
    public Drone DashTutorial_Drone;
    [SerializeField] private Transform shootPoint_DashTutorial;
    public void IntroduceNormalEnergy(TimelineManager timelineManager)
    {
        timelineManager_DashTutorial = timelineManager;

        //Change button sprite according to the controller
        
        CameraManager.Instance.FocusTo(PlayerCharacter.Singleton.transform, 99999f);
        CameraManager.Instance.FlashIn(6f, 0.05f, 999999f, 0.01f);
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        TimeManager.Instance.startSlowMotion(-1f, 0f, 1.05f,0.15f);
        TimeManager.Instance.ApplyBlackScreen();


        Player.CurrentPlayer.energyLocked = false;
        PlayerCharacter.Singleton.AddNormalEnergy(1);

        PlayerCharacter.Singleton.isInTutorial = true;
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "InTutorial_Dash";

        DashTutorial_Drone.Fire(shootPoint_DashTutorial.position, false);

        StartCoroutine(ShowGameObjectAfterDelay(1.0f, MouseIndicator.Singleton.CurrentInputType == InputType.Joystick ? UI_DashKey_Joy :
            UI_DashKey_Keyboard));
    }


    public void AfterDashTutorial()
    {
        CameraManager.Instance.Idle();
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        timelineManager_DashTutorial.PlayNextTimeline();
        TimeManager.Instance.endSlowMotion(0f);

        if (MouseIndicator.Singleton.CurrentInputType == InputType.Joystick) UI_DashKey_Joy.SetActive(false);
        else UI_DashKey_Keyboard.SetActive(false);

        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }

    public void IntroduceOverloadEnergy()
    {
        Player.CurrentPlayer.overloadEnergyLocked = false;
        //PlayerCharacter.Singleton.AddOverLoadEnergy(1);
    }

    /// <summary> ------------------------------------------------------------------------------------------------
    /// Tutorial: Deflect
    /// </summary>
    private TimelineManager timelineManager_DeflectTutorial;
    public Bullet DeflectTutorial_Bullet;
    public GameObject DeflectTutorial_Jack;
    public void IntroduceBulletDeflect(TimelineManager timelineManager)
    {
        timelineManager_DeflectTutorial = timelineManager;

        DeflectTutorial_Jack.GetComponentsInChildren<Animator>()[1].runtimeAnimatorController = null;

        //Change button sprite according to the controller
        //UI_DashKey.GetComponent<SpriteRenderer>().sprite = ;

        timelineManager_DeflectTutorial.PlayTimelineInIndex(0);

        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "InTutorial_Deflect";

        StartCoroutine(ShowGameObjectAfterDelay(1.0f, null));
    }

    public void AfterDeflectTutorial()
    {
        CameraManager.Instance.Idle();
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        timelineManager_DeflectTutorial.PlayNextTimeline();
        TimeManager.Instance.endSlowMotion(0f);

        if (MouseIndicator.Singleton.CurrentInputType == InputType.Joystick) UI_AttackKey_Joy.SetActive(false);
        else UI_AttackKey_Keyboard.SetActive(false);

        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }
    
    /// <summary> ------------------------------------------------------------------------------------------------
    /// Tutorial: Power Dash
    /// </summary>
    private TimelineManager timelineManager_PowerDashTutorial;
    public void IntroducePowerDash(TimelineManager timelineManager)
    {
        timelineManager_PowerDashTutorial = timelineManager;

        timelineManager_PowerDashTutorial.PlayTimelineInIndex(0);

        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "InTutorial_PowerDash";

        StartCoroutine(ShowGUIButtonAfterDelay(3.5f, "Up"));
    }

    public void AfterPowerDashTutorial()
    {
        CameraManager.Instance.Idle();
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        //TimeManager.Instance.endSlowMotion(0f);

        GUITutorial.Singleton.Hide();


        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }


    private IEnumerator ShowGameObjectAfterDelay(float delayTime, GameObject targetObject)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        targetObject?.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);
        PlayerCharacter.Singleton.isInTutorial = false;
    }

    private IEnumerator ShowGUIButtonAfterDelay(float delayTime, string buttonName)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        GUITutorial.Singleton.Show(buttonName);

        yield return new WaitForSecondsRealtime(0.5f);
        PlayerCharacter.Singleton.isInTutorial = false;
    }

}
