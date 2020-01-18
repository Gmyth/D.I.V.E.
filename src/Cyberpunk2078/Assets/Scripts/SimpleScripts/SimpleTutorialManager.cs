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
                    UI_DashKey.SetActive(true);
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
    public GameObject UI_DashKey;
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

    private TimelineManager timelineManager_DashTutorial;
    public Drone DashTutorial_Drone;
    [SerializeField] private Transform shootPoint_DashTutorial;
    public void IntroduceNormalEnergy(TimelineManager timelineManager)
    {
        timelineManager_DashTutorial = timelineManager;

        //Change button sprite according to the controller
        //UI_DashKey.GetComponent<SpriteRenderer>().sprite = ;
        CameraManager.Instance.FocusAt(PlayerCharacter.Singleton.transform, 99999f);
        CameraManager.Instance.FlashIn(6f, 0.05f, 999999f, 0.01f);
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        TimeManager.Instance.startSlowMotion(999999f, 0f, 0.001f);

        Player.CurrentPlayer.energyLocked = false;
        PlayerCharacter.Singleton.AddNormalEnergy(1);

        PlayerCharacter.Singleton.isInTutorial = true;
        PlayerCharacter.Singleton.GetFSM().CurrentStateName = "InTutorial_Dash";

        DashTutorial_Drone.Fire(shootPoint_DashTutorial.position, false);

        StartCoroutine(ShowGameObjectAfterDelay(1.0f, UI_DashKey));
    }


    public void AfterDashTutorial()
    {
        CameraManager.Instance.Idle();
        PlayerCharacter.Singleton.SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        timelineManager_DashTutorial.PlayNextTimeline();
        TimeManager.Instance.endSlowMotion(0f);

        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }

    public void IntroduceOverloadEnergy()
    {
        Player.CurrentPlayer.overloadEnergyLocked = false;
        //PlayerCharacter.Singleton.AddOverLoadEnergy(1);
    }

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

        PlayerCharacter.Singleton.transform.parent.GetComponentInChildren<MouseIndicator>().ResetColor();
    }


    private IEnumerator ShowGameObjectAfterDelay(float delayTime, GameObject targetObject)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        targetObject?.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);
        PlayerCharacter.Singleton.isInTutorial = false;
    }

}
