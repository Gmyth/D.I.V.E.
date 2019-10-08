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
        attributes.Set(AttributeType.MaxOsp_c0, overloadEnergyCap);
        
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
        
        float maxNormalSp = this[AttributeType.MaxSp_c0];
        
        //TODO: use overload energy attribute for the MaxEnergy
        float maxOverloadSp = this[AttributeType.MaxSp_c0];
        if (normalEnergy - amount < 0)
        {
            if (overloadEnergy - amount >= 0)
            {
                // use overload Energy
                overloadEnergy = Mathf.Max(Mathf.Min( overloadEnergy - amount, maxOverloadSp), 0);
                UIUpdate();
                return true;
            }
            return false;
        }
        
        normalEnergy = Mathf.Max(Mathf.Min( normalEnergy - amount, maxNormalSp), 0);
        UIUpdate();
        return true;
    }
    
    public bool AddNormalEnergy(float amount)
    {
        float maxSp = this[AttributeType.MaxSp_c0];
        
        normalEnergy = Mathf.Max(Mathf.Min(amount + normalEnergy, maxSp), 0);
        UIUpdate();
        return true;
    }
    
    public bool AddOverLoadEnergy(float amount)
    {
        
        //TODO: use attribute for the MaxEnergy
        float maxSp = this[AttributeType.MaxOsp_c0];
        
        overloadEnergy = Mathf.Max(Mathf.Min(amount + overloadEnergy, maxSp), 0);
        UIUpdate();
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
        UIUpdate();
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
            //DisableInput
            ObjectRecycler.Singleton.RecycleAll();
            PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 9;
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
        UIUpdate();
        //ApplyEnergyChange(0);
    }

    private void UIUpdate()
    {
        
        // temp UI related
        GameObject Energy1 = GameObject.Find("Energy1");
        GameObject Energy2 = GameObject.Find("Energy2");
        if (normalEnergy == 0 && overloadEnergy > 0)
        {
            // first energy bar red
            Energy1.GetComponent<Slider>().value = 1;
            Energy1.GetComponentInChildren<Image>().color = Color.red;
            
            Energy2.GetComponent<Slider>().value = 0;
            Energy2.GetComponentInChildren<Image>().color = Color.red;
        }else if (normalEnergy > 0)
        {
            Energy1.GetComponent<Slider>().value = 1;
            Energy1.GetComponentInChildren<Image>().color = Color.blue;
            
            if (overloadEnergy > 0)
            {
                Energy2.GetComponent<Slider>().value = 1;
                Energy2.GetComponentInChildren<Image>().color = Color.red;
            }
            else
            {
                Energy2.GetComponent<Slider>().value = 0;
            }
        }
        else
        {
            Energy1.GetComponent<Slider>().value = 0;
            Energy2.GetComponent<Slider>().value = 0;
        }


    }
}
