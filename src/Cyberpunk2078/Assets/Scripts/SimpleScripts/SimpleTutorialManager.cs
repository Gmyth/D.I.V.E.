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
                    Time.timeScale = 0;
                    dashKeyUI.SetActive(true);
                    break;

                case TutorialState.TeachDoubleDash:

                    break;
            }

            CurrentState = value;
        }

        get { return CurrentState; }
    }

    [SerializeField] private GameObject dashKeyUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {

        }
    }

    public void DeleteEnergySystem()
    {
        Player.CurrentPlayer.energyLocked = true;
        Player.CurrentPlayer.overloadEnergyLocked = true;
        Player.CurrentPlayer.CostEnergy(1);
    }

    public void TeachDash()
    {
        CurrentState = TutorialState.TeachDash;
        Time.timeScale = 0;
        dashKeyUI.SetActive(true);
    }

    public void TeachDoubleDash()
    {
        CurrentState = TutorialState.TeachDoubleDash;
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
