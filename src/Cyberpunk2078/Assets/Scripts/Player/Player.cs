﻿//using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
//using UnityEngine;
//using UnityEngine.UI;


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

    public bool JumpForceGate;
    public bool OnSlope;
    public float LastBounceSec;
    public bool NoApplyFriction = false;
    public bool secondJumpReady;
    public bool ChainWallJumpReady;

    public bool climbReady = true;
    public bool energyLocked;
    public bool overloadEnergyLocked;

    public bool RightTriggerReady;
    public bool LeftTriggerReady;
    public float knockBackDuration;

    public float FeverFactor = 1.3f;

    private List<KeyValuePair<int, uint>> temporaryCollectibles = new List<KeyValuePair<int, uint>>();
    private HashSet<uint> achievements = new HashSet<uint>();
    

    public float this[AttributeType type]
    {
        get
        {
            return AttributeSet.Sum(type, attributes, inventory);
        }
    }


    private Player(
        int healthCap = 3, 
        float normalEnergyCap = 1, 
        float overloadEnergyCap = 1,
        float feverEnergyCap = 100, 
        float feverDecayRate = 15, 
        float killStreakCap = 2,
        float killStreakDecay = 4,
        float powerDashCoolDown = 8
    )
    {
        inventory = new Inventory();
        attributes = new AttributeSet();
        attributes.Set(AttributeType.MaxHp_c0, healthCap);
        attributes.Set(AttributeType.MaxSp_c0, normalEnergyCap);
        attributes.Set(AttributeType.MaxOsp_c0, overloadEnergyCap);
        attributes.Set(AttributeType.MaxUltimateEnergy_c0, feverEnergyCap);
        attributes.Set(AttributeType.FeverDecay_c0, feverDecayRate);
        attributes.Set(AttributeType.MaxKs_c0, killStreakCap);
        attributes.Set(AttributeType.KsDecay_c0, killStreakDecay);
        attributes.Set(AttributeType.PDCoolDown_c0, powerDashCoolDown);
        energyLocked = false;
        overloadEnergyLocked = false;
        secondJumpReady = true;


        if (GameProcessManager.Singleton)
        {
            GameProcessManager.Singleton.OnStartLevel.AddListener(ClearTemporaryCollectibles);
            GameProcessManager.Singleton.OnQuitLevel.AddListener(RevokeTemporaryCollectibles);
        }
    }


    public void AddCollectibleItem(int itemID, uint achievementID)
    {
        inventory.AddItem(itemID);
        AddAchievement(achievementID);
        temporaryCollectibles.Add(new KeyValuePair<int, uint>(itemID, achievementID));
    }


    public void AddAchievement(uint id)
    {
        achievements.Add(id);
    }

    public bool HasAchievement(uint id)
    {
        return achievements.Contains(id);
    }


    private void ClearTemporaryCollectibles(int levelID)
    {
        temporaryCollectibles.Clear();
    }

    private void RevokeTemporaryCollectibles(int levelID)
    {
       // UnityEngine.Debug.LogWarning("!!!!");

        foreach (KeyValuePair<int, uint> temporaryCollectible in temporaryCollectibles)
        {
            inventory.RemoveItem(temporaryCollectible.Key);
            achievements.Remove(temporaryCollectible.Value);
        }


        ClearTemporaryCollectibles(levelID);
    }
}
