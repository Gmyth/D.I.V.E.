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
        GameObject Energy1 = GameObject.Find("Energy1");
        GameObject Energy2 = GameObject.Find("Energy2");
        if (Energy + amount < 0)
            return false;

        float maxSp = this[AttributeType.MaxSp_c0];

        Energy = Mathf.Max(Mathf.Min(amount + Energy, maxSp), 0);
        Energy1.GetComponent<Slider>().value = Mathf.Max(Mathf.Min(Energy, maxSp / 2), 0) / (maxSp / 2);
        Energy2.GetComponent<Slider>().value = Mathf.Max((Energy-maxSp / 2), 0) / (maxSp / 2);


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
        SpriteRenderer health1 = GameObject.Find("Health1").GetComponent<SpriteRenderer>();
        SpriteRenderer health2 = GameObject.Find("Health2").GetComponent<SpriteRenderer>();
        SpriteRenderer health3 = GameObject.Find("Health3").GetComponent<SpriteRenderer>();

        if (Health < amount)
        {
            return false;
        }


        Health = Mathf.Max(Mathf.Min(amount + Health, this[AttributeType.MaxHp_c0]), 0);

        if (Health > 0)
            health1.enabled = true;
        else
            health1.enabled = false;

        if (Health > 1)
            health2.enabled = true;
        else
            health2.enabled = false;

        if (Health > 2)
            health3.enabled = true;
        else
            health3.enabled = false;

        Debug.Log(Health);
        if (Health <= 0)
        {
            Debug.Log("adwwadw");
            RestoreHealth();
            CheckPointManager.Instance.RestoreCheckPoint();
        }

        return true;
    }

    private void RestoreHealth()
    {
        Health = 3;
        Energy = 200;
        ApplyHealthChange(0);
        ApplyEnergyChange(0);
    }
}
