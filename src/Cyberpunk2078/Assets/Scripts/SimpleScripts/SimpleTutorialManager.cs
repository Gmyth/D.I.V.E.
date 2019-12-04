using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                    IntroduceNormalEnergy();
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

    public void IntroduceNormalEnergy()
    {
        Player.CurrentPlayer.energyLocked = false;
        PlayerCharacter.Singleton.AddNormalEnergy(1);
    }

    public void IntroduceOverloadEnergy()
    {
        Player.CurrentPlayer.overloadEnergyLocked = false;
        //PlayerCharacter.Singleton.AddOverLoadEnergy(1);
    }

}
