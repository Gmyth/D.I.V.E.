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

    public bool jumpForceGate;
    public bool secondJumpReady;
    public bool lastWallJumpRight; 
    
    public float Health { get; private set; }
    
    public float normalEnergy { get; private set; }
    public float overloadEnergy { get; private set; }

    public float this[AttributeType type]
    {
        get
        {
            return AttributeSet.Sum(type, attributes, inventory);
        }
    }


    // Internal Usage
    private float lastSecondEnergyRecover;


    private Player(int healthCap = 3, float normalEnergyCap = 1 ,float overloadEnergyCap = 1, float energyRecoverRate = 0)
    {
        inventory = new Inventory();

        
        attributes = new AttributeSet();
        attributes.Set(AttributeType.MaxHp_c0, healthCap);
        attributes.Set(AttributeType.MaxSp_c0, normalEnergyCap);
        //TODO: add attributes for overload energy
        
        attributes.Set(AttributeType.SpRecovery_c0, energyRecoverRate);


        secondJumpReady = true;
        Health = healthCap;
        normalEnergy = normalEnergyCap;
        overloadEnergy = 0;
        lastSecondEnergyRecover = 0;
    }


    public bool CostEnergy(float amount)
    {
        if (amount < 0)
        {
            return false;
        }

        // temp UI related
        GameObject Energy1 = GameObject.Find("Energy1");
        GameObject Energy2 = GameObject.Find("Energy2");
        
        float maxNormalSp = this[AttributeType.MaxSp_c0];
        
        //TODO: use overload energy attribute for the MaxEnergy
        float maxOverloadSp = this[AttributeType.MaxSp_c0];
        if (normalEnergy + amount < 0)
        {
            if (overloadEnergy + amount > 0)
            {
                // use overload Energy
                overloadEnergy = Mathf.Max(Mathf.Min(amount + overloadEnergy, maxOverloadSp), 0);
                return true;
            }
            return false;
        }
        normalEnergy = Mathf.Max(Mathf.Min(amount + normalEnergy, maxNormalSp), 0);
        return true;

        

        
        
//        Energy1.GetComponent<Slider>().value = Mathf.Max(Mathf.Min(normalEnergy, maxSp / 2), 0) / (maxSp / 2);
//        Energy2.GetComponent<Slider>().value = Mathf.Max((normalEnergy-maxSp / 2), 0) / (maxSp / 2);
    }
    
    public bool AddNormalEnergy(float amount)
    {
        float maxSp = this[AttributeType.MaxSp_c0];
        
        normalEnergy = Mathf.Max(Mathf.Min(amount + normalEnergy, maxSp), 0);
        
        return true;
    }
    
    public bool AddOverLoadEnergy(float amount)
    {
        
        //TODO: use attribute for the MaxEnergy
        float maxSp = this[AttributeType.MaxSp_c0];
        
        overloadEnergy = Mathf.Max(Mathf.Min(amount + overloadEnergy, maxSp), 0);
        
        return true;
    }
    
    
    public void EnergyRecover(float time, float energyCustom = -1)
    {
        if (time - lastSecondEnergyRecover > 0.2f)
        {
            if (energyCustom < 0) {
                AddNormalEnergy(this[AttributeType.SpRecovery_c0]);
            }else{
                AddNormalEnergy(energyCustom);
            }
            
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

        if (Health <= 0)
        {
            RestoreHealth();
            CheckPointManager.Instance.RestoreCheckPoint();
        }

        return true;
    }

    private void RestoreHealth()
    {
        Health = 3;
        normalEnergy = 1;
        ApplyHealthChange(0);
       //ApplyEnergyChange(0);
    }

    private void UIUpdate()
    {
        
    }
}
