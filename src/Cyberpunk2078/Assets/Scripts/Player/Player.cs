using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public static Player CurrentPlayer { get; private set; } = null;

    public static Player CreatePlayer()
    {
        return CurrentPlayer = new Player();
    }

    public static Player LoadPlayer()
    {
        return new Player();
    }


    public Inventory Inventory { get; private set; }

    public bool SecondJumpReady;
    
    public int Health { get; private set; }
    
    public int HealthCap { get; private set; }

    public float EnergyCap { get; private set; }
    
    public float EnergyRecoverRate { get; private set; }
    
    public float Energy { get; private set; }
    
    
    // Internal Usage
    private float lastSecondEnergyRecover;

    private Player(int healthCap = 3, float energyCap = 200f , float energyRecoverRate = 6f)
    {
        Inventory = new Inventory();
        SecondJumpReady = true;
        HealthCap = healthCap;
        Health = healthCap;
        EnergyCap = energyCap;
        EnergyRecoverRate = energyRecoverRate;
        Energy = energyCap;
        lastSecondEnergyRecover = 0;
    }


    public bool ApplyEnergyChange(float amount)
    {
        if (Energy + amount < 0) return false;
        Energy = Mathf.Max(Mathf.Min(amount + Energy, EnergyCap), 0);
        GameObject.Find("Energy1").GetComponent<Slider>().value = Mathf.Max(Mathf.Min(Energy, EnergyCap/2), 0)/(EnergyCap/2);
        GameObject.Find("Energy2").GetComponent<Slider>().value = Mathf.Max((Energy-EnergyCap/2), 0)/(EnergyCap/2);
        return true;
    }
    
    public void EnergyRecover(float time)
    {
        if (time - lastSecondEnergyRecover > 0.2f)
        {
            ApplyEnergyChange(EnergyRecoverRate);
            lastSecondEnergyRecover = time;
        }

    }
    
    public bool ApplyHealthChange(int amount)
    {
        if (Health < amount) return false;
        Health = Mathf.Max(Mathf.Min(amount + Health, HealthCap), 0);
        return true;
    }



}
