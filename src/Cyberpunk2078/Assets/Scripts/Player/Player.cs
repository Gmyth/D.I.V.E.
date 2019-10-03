using System.Collections;
using System.Collections.Generic;
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


    public readonly AttributeSet attributes;
    public readonly Inventory inventory;

    public bool SecondJumpReady;
    
    public float Health { get; private set; }
    
    public float Energy { get; private set; }


    public float this[AttributeType type]
    {
        get
        {
            return AttributeSet.Sum(type, attributes, inventory);
        }
    }


    // Internal Usage
    private float lastSecondEnergyRecover;


    private Player(int healthCap = 3, float energyCap = 200f , float energyRecoverRate = 6f)
    {
        inventory = new Inventory();

        
        attributes = new AttributeSet();
        attributes.Set(AttributeType.MaxHp_c0, healthCap);
        attributes.Set(AttributeType.MaxSp_c0, energyCap);
        attributes.Set(AttributeType.SpRecovery_c0, energyRecoverRate);


        SecondJumpReady = true;
        Health = healthCap;
        Energy = energyCap;
        lastSecondEnergyRecover = 0;
    }


    public bool ApplyEnergyChange(float amount)
    {
        if (Energy + amount < 0)
            return false;

        float maxSp = this[AttributeType.MaxSp_c0];

        Energy = Mathf.Max(Mathf.Min(amount + Energy, maxSp), 0);
        GameObject.Find("Energy1").GetComponent<Slider>().value = Mathf.Max(Mathf.Min(Energy, maxSp / 2), 0) / (maxSp / 2);
        GameObject.Find("Energy2").GetComponent<Slider>().value = Mathf.Max((Energy-maxSp / 2), 0) / (maxSp / 2);


        return true;
    }
    
    public void EnergyRecover(float time)
    {
        if (time - lastSecondEnergyRecover > 0.2f)
        {
            ApplyEnergyChange(this[AttributeType.SpRecovery_c0]);
            lastSecondEnergyRecover = time;
        }
    }
    
    public bool ApplyHealthChange(float amount)
    {
        if (Health < amount)
            return false;


        Health = Mathf.Max(Mathf.Min(amount + Health, this[AttributeType.MaxHp_c0]), 0);

        if(GameObject.Find("Health1"))GameObject.Find("Health1").gameObject.SetActive(Health > 0);
        if(GameObject.Find("Health2")) GameObject.Find("Health2").gameObject.SetActive(Health > 1);
        if(GameObject.Find("Health3"))GameObject.Find("Health3").gameObject.SetActive(Health > 2);

        
        return true;
    }
}
