using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetTutorialState(int stateIndex)
    {
        CurrentState = (TutorialState)stateIndex;
    }

    public void DeleteEnergySystem()
    {
        Player.CurrentPlayer.energyLocked = true;
        Player.CurrentPlayer.overloadEnergyLocked = true;
        Player.CurrentPlayer.CostEnergy(1);
    }

    public void IntroduceNormalEnergy()
    {
        Player.CurrentPlayer.energyLocked = false;
        Player.CurrentPlayer.AddNormalEnergy(1);
    }

    public void IntroduceOverloadEnergy()
    {
        Player.CurrentPlayer.overloadEnergyLocked = false;
        Player.CurrentPlayer.AddOverLoadEnergy(1);
    }

}
