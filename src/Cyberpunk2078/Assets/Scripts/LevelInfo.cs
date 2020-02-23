using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo: MonoBehaviour
{
    public GameObject StartPoint;
    public int LevelIndex;
    public GameObject DummyHolder;

    public GameObject[] breakable;

    public bool DashEnabled = false;
    
    public Enemy boss = null;
    private void Start()
    {
        PlayerCharacter.Singleton.PowerDashReady = DashEnabled;
        PlayerCharacter.Singleton.PowerDashUnlock = DashEnabled;
        PlayerCharacter.Singleton.UpdatePowerDashUI();
        
        
        if (boss)
            GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowEnemyWidget(boss);
    }
}
