using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
