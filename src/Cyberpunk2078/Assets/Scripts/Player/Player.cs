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
    public bool ChainWallJumpReady;

    public bool climbReady = true;
    public bool energyLocked;
    public bool overloadEnergyLocked;

    public bool triggerReady;
    
    public float knockBackDuration;


    public float this[AttributeType type]
    {
        get
        {
            return AttributeSet.Sum(type, attributes, inventory);
        }
    }


    private Player(int healthCap = 3, float normalEnergyCap = 1 ,float overloadEnergyCap = 1,float feverEnergyCap = 100, float feverDecayRate = 1)
    {
        inventory = new Inventory();
        attributes = new AttributeSet();
        attributes.Set(AttributeType.MaxHp_c0, healthCap);
        attributes.Set(AttributeType.MaxSp_c0, normalEnergyCap);
        attributes.Set(AttributeType.MaxOsp_c0, overloadEnergyCap);
        attributes.Set(AttributeType.MaxFever_c0, feverEnergyCap);
        attributes.Set(AttributeType.FeverDecay_c0, feverDecayRate);


        energyLocked = false;
        overloadEnergyLocked = false;
        secondJumpReady = true;
    }
}
