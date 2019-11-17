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
    public bool NoApplyFriction = false;
    public bool secondJumpReady;
    public bool lastWallJumpRight;
    public bool energyLocked;
    public bool overloadEnergyLocked;

    public bool triggerReady;
    
    public float Health { get; private set; }
    
    public float normalEnergy { get; private set; }
    public float overloadEnergy { get; private set; }
    
    public float FeverEnergy { get; private set; }
    
    public bool Fever { get; private set; }

    public float this[AttributeType type]
    {
        get
        {
            return AttributeSet.Sum(type, attributes, inventory);
        }
    }


    // Internal Usage
    private float lastSecondEnergyRecover;
    
    private float lastSecondEnergyCost;


    private Player(int healthCap = 3, float normalEnergyCap = 1 ,float overloadEnergyCap = 1,float feverEnergyCap = 100, float feverDecayRate = 1)
    {

        inventory = new Inventory();
        attributes = new AttributeSet();
        attributes.Set(AttributeType.MaxHp_c0, healthCap);
        attributes.Set(AttributeType.MaxSp_c0, normalEnergyCap);
        attributes.Set(AttributeType.MaxOsp_c0, overloadEnergyCap);
        attributes.Set(AttributeType.MaxFsp_c0, feverEnergyCap);
        attributes.Set(AttributeType.FspDecay_c0, feverDecayRate);


        energyLocked = false;
        overloadEnergyLocked = false;
        secondJumpReady = true;
        Health = healthCap;
        normalEnergy = normalEnergyCap;
        FeverEnergy = 0;
        overloadEnergy = 0;
        lastSecondEnergyCost = 0;
        UIUpdate();
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
        if (energyLocked) return false;

        float maxSp = this[AttributeType.MaxSp_c0];
        
        normalEnergy = Mathf.Max(Mathf.Min(amount + normalEnergy, maxSp), 0);
        UIUpdate();
        return true;
    }
    
    public bool AddOverLoadEnergy(float amount)
    {
        if (overloadEnergyLocked) return false;

        float maxSp = this[AttributeType.MaxOsp_c0];
        
        overloadEnergy = Mathf.Max(Mathf.Min(amount + overloadEnergy, maxSp), 0);
        UIUpdate();
        return true;
    }

    public bool AddFeverEnergy(float amount)
    {
        float maxSp = this[AttributeType.MaxFsp_c0];

        FeverEnergy = Mathf.Max(Mathf.Min(amount + FeverEnergy, maxSp), 0);
        
        UIUpdate();

        if (FeverEnergy == 100)
        {
            Fever = true;
        }
        else if(FeverEnergy  == 0)
        {
            Fever = false;
        }
        return true;
    }

//    public bool CostFeverEnergy()
//    {
//        if (FeverEnergy == 100 && Health < this[AttributeType.MaxHp_c0])
//        {
//            ApplyHealthChange(1);
//            FeverEnergy = 0;
//            UIUpdate();
//            return true;
//        }
//        
//        if (FeverEnergy == 100)
//        {
//            Fever = true;
//        }
//        else if(FeverEnergy  == 0)
//        {
//            Fever = false;
//        }
//        
//        return false;
//    }

    public void CostFeverEnergy(float time, float energyCustom = -1)
    {
        // cost fever energy in Update
        // multi call prevention
        if (time - lastSecondEnergyCost > 0.2f)
        {
            if (energyCustom < 0) {
                AddFeverEnergy(-this[AttributeType.FspDecay_c0]);
            }else{
                AddFeverEnergy(-energyCustom);
            }
            
            lastSecondEnergyCost = time;
        }
        UIUpdate();
    }
    
    public bool ApplyHealthChange(float amount)
    {
        Health = Mathf.Clamp(amount + Health, 0, this[AttributeType.MaxHp_c0]);


        SpriteRenderer health1 = GameObject.Find("Health1").GetComponent<SpriteRenderer>();
        SpriteRenderer health2 = GameObject.Find("Health2").GetComponent<SpriteRenderer>();
        SpriteRenderer health3 = GameObject.Find("Health3").GetComponent<SpriteRenderer>();

        if (health1 && health2 && health3)
        {
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
        }


        if (Health <= 0)
        {
            // DisableInput
            ObjectRecycler.Singleton.RecycleAll();
            PlayerCharacter.Singleton.GetFSM().CurrentStateIndex = 9;
            RestoreHealth();
            CheckPointManager.Instance?.RestoreCheckPoint();
        }


        return Health > 0;
    }
    private void RestoreHealth()
    {
        Health = 3;
        normalEnergy = 1;
        FeverEnergy = 0;
        ApplyHealthChange(0);
        UIUpdate();
        //ApplyEnergyChange(0);
    }

    private void UIUpdate()
    {
        // temp UI related
        GameObject Energy1 = GameObject.Find("Energy1");
        GameObject Energy2 = GameObject.Find("Energy2");

        if (Energy1 && Energy2)
        {
            if (normalEnergy == 0 && overloadEnergy > 0)
            {
                // first energy bar red
                Energy1.GetComponent<Slider>().value = 1;
                Energy1.GetComponentInChildren<Image>().color = Color.red;

                Energy2.GetComponent<Slider>().value = 0;
                Energy2.GetComponentInChildren<Image>().color = Color.red;
            }
            else if (normalEnergy > 0)
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
        
        
        GameObject HealthEnergyUI = GameObject.Find("HealthEnergy");
        GameObject ButtonNotice = GameObject.Find("HealthButton");

        if (HealthEnergyUI && ButtonNotice)
        {
            HealthEnergyUI.GetComponent<Slider>().value = FeverEnergy / this[AttributeType.MaxFsp_c0];

            if (FeverEnergy == 100)
                ButtonNotice.transform.localScale = new Vector3(0.09541447f, 0.206793f, 0.4598505f);
            else
                ButtonNotice.transform.localScale = Vector3.zero;
        }
    }
}
