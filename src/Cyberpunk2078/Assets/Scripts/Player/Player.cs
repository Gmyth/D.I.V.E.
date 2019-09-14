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
    
    public float Health { get; private set; }
    
    public float HealthCap { get; private set; }

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
    
    public bool ApplyHealthChange(float amount)
    {
        if (Health < amount) return false;
        Health = Mathf.Max(Mathf.Min(amount + Health, HealthCap), 0);
        GameObject.Find("Health1").GetComponent<SpriteRenderer>().color = Health > 0? Color.white : Color.clear;
        GameObject.Find("Health2").GetComponent<SpriteRenderer>().color = Health > 1? Color.white : Color.clear;
        GameObject.Find("Health3").GetComponent<SpriteRenderer>().color = Health > 3? Color.white : Color.clear;
        return true;
    }



}
