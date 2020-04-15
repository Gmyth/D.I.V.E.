﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;
    public float invulnerableDuration = 0.5f;
    private float lastHit;
    public Transform SpriteHolder;
    public GameObject groundDust;
    public GameObject FeverVFX;
    public GameObject OverloadVFX;
    public GameObject Spark;
    //public GUITutorial tutorial;
   
    public bool isInTutorial;


    private float lastKillStreakDecaySecond;
    private Player player;
    private new Rigidbody2D rigidbody;
        
    public int currentDialogueID = -1;
    public Action[] currentDialogueCallbacks = null;


    public EventOnStatisticChange OnStatisticChange { get; private set; }

    public PlayerState State => fsm.CurrentState;

    public float this[StatisticType statisticType]
    {
        get
        {
           // Debug.LogWarning(statistics);
            return statistics[statisticType];
        }
    }

    public FSMPlayer GetFSM()
    {
        return fsm;
    }

    public bool InKillStreak { get; private set; } = false;
    public bool ClimbBlock = false;
    public bool PowerDash = false;
    public bool PowerDashReady = false;
    public bool PowerDashUnlock = false;
    public float LastPowerDash;
    public float Gravity;
    public float DefaultGravity = 3;

    [SerializeField] private GameObject buttonTip;
    [SerializeField] private GameObject powerDashCoolDown;
    public bool InFever { get; private set; } = false;
    public bool MaxUltimateEnergy { get; private set; }
    //public PlayerCharacter(Player player)
    //{
    //    statistic = new StatisticSystem(player.attributes, player.inventory);


    //    OnStatisticChange = statistic.onStatisticChange;
    //}


    public void Knockback(Vector3 direction, float force, float duration = 1f)
    {
        if (duration > 0)
        {
            Player.CurrentPlayer.knockBackDuration = duration;
            fsm.CurrentStateName = "Knockback";
        }
        else
            fsm.CurrentStateName = "Bounced";


        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0;


        if (force > 0 && direction != Vector3.zero)
            rigidbody.AddForce(force * direction.normalized, ForceMode2D.Impulse);
    }

    public void KnockbackHorizontal(Vector3 origin, float force, float duration = 1f)
    {
        Vector3 direction = transform.position - origin;
        Vector2 groundNormal = GroundNormal;


        Knockback((direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized + groundNormal).normalized, force, duration);
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Take {0} damage.", rawDamage));
        if (lastHit + invulnerableDuration > Time.time)
        {
            // still invulnerable
            return 0;
        }

        if (rawDamage <= 0)
            return 0;

        AudioManager.Singleton.PlayOnce("Hurt");

        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -rawDamage, 0);

        if (result.currentValue <= 0)
        {
            AudioManager.Singleton.PlayOnce("Player_dead");
            Dead();
        }
        else if (result.currentValue <= 1f)
        {
            AudioManager.Singleton.PlayEvent("LowHealth");
        }

        lastHit = Time.time;
        StartCoroutine(Blink());
        
        
        return result.previousValue - result.currentValue;
    }

    private IEnumerator Blink()
    {
        int Counter = 0;
        while (lastHit + invulnerableDuration > Time.time)
        {
            SpriteHolder.GetComponent<SpriteRenderer>().color = Counter % 2 == 0 ? Color.grey : Color.white;
            Counter++;
            yield return new WaitForSeconds(0.1f);
        }
        
        SpriteHolder.GetComponent<SpriteRenderer>().color = Color.white;
        yield return null;
        yield return null;
    }

    public float Heal(float rawHeal)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Heal {0}.", rawHeal));


        if (rawHeal <= 0)
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, rawHeal, 0, statistics[StatisticType.MaxHp]);
        if (result.currentValue >= 1 && result.currentValue < statistics[StatisticType.MaxHp])
            AudioManager.Singleton.StopEvent("LowHealth");

        return result.previousValue - result.currentValue;
    }

    public override void Dead()
    {
        
        fsm.CurrentStateName = "NoInput";

        ResetStatistics();


        ObjectRecycler.Singleton.RecycleAll();
        CheckPointManager.Instance?.Restore();
    }

    public bool ActivateFever()
    {
        if (statistics[StatisticType.UltimateEnergy] > 30f && !InFever)
        {
            FeverVFX.SetActive(true);
            InFever = true;
            CameraManager.Instance.FlashIn(7,0.05f,0.05f,0.05f);
            AddOverLoadEnergy(1);
            TimeManager.Instance.StartFeverMotion();

            AudioManager.Singleton.PlayOnce("Fever");
            AudioManager.Singleton.PlayEvent("FeverMode");
        }
        return InFever;
        
    }
    

    public bool AddNormalEnergy(float amount)
    {
        if (!player.energyLocked)
        {
            float maxSp = statistics[StatisticType.MaxSp];

            statistics.Modify(StatisticType.Sp, amount, 0, maxSp);


            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AddOverLoadEnergy(float amount)
    {
        if (!player.overloadEnergyLocked)
        {
            float maxSp = statistics[StatisticType.MaxOsp];

            statistics.Modify(StatisticType.Osp, amount, 0, maxSp);

            OverloadVFX.SetActive(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public float ConsumeEnergy(float value)
    {
        if (value < 0)
            return -1;

        if (statistics[StatisticType.Sp] + statistics[StatisticType.Osp] < value)
            return -1;


        float usedEnergy = 0;

        StatisticModificationResult result = statistics.Modify(StatisticType.Sp, -value, 0, statistics[StatisticType.MaxSp]);

        usedEnergy += result.previousValue - result.currentValue;

        value -= usedEnergy;


        if (value >= 1)
        {
            result = statistics.Modify(StatisticType.Osp, -value, 0, statistics[StatisticType.MaxOsp]);
            OverloadVFX.SetActive(false);
            usedEnergy += result.previousValue - result.currentValue;
        }
        

        return usedEnergy;
    }

   


    public bool AddUltimateEnergy(float amount)
    {
        StatisticModificationResult result = statistics.Modify(StatisticType.UltimateEnergy, amount, 0, statistics[StatisticType.MaxUltimateEnergy]);
       
        //TODO add max value effect
        if (result.currentValue >= statistics[StatisticType.MaxUltimateEnergy])
            GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").HighlightFeverBar();
        else if(!InFever)
            GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").DehighlightFeverBar();

        float pitch = (statistics[StatisticType.UltimateEnergy] / statistics[StatisticType.MaxUltimateEnergy]);
        AudioManager.Singleton.PlayOnce("EnergyCharge", "Pitch", pitch);

        return true;
    }

    public bool ConsumeUltimateEnergy(float value)
    {
        StatisticModificationResult result = statistics.Modify(StatisticType.UltimateEnergy, -value, 0, statistics[StatisticType.MaxUltimateEnergy]);

        if (result.currentValue <= 0 && InFever)
        {
            InFever = false;
            FeverVFX.SetActive(false);
            GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").DehighlightFeverBar();
            TimeManager.Instance.EndFeverMotion();
            
            AudioManager.Singleton.StopEvent("FeverMode");
        }
        return true;
    }
    
    public void KillStreakDecay()
    {
        if (!InKillStreak){
            if(lastKillStreakDecaySecond + statistics[StatisticType.KsDecay] < Time.time)
            {
                //Kill Streak Decay
                statistics[StatisticType.KsCount] = Mathf.Max(0, statistics[StatisticType.KsCount] - 1);
                lastKillStreakDecaySecond = Time.time;
            }

        }
        else
        {
            if(lastKillStreakDecaySecond + statistics[StatisticType.KsDecay] * 2 < Time.time)
            {
                //Kill Streak Decay, Kill Streak Over
                statistics[StatisticType.KsCount] = 0;
                lastKillStreakDecaySecond = Time.time;
                DeactivateKillStreak();
            }
        }
    }

    public void PowerDashCoolDown()
    {
        if (!PowerDashUnlock)
        {
            powerDashCoolDown.GetComponent<Slider>().value = 0f;
            buttonTip.SetActive(false);
            return;
        }

        //Debug.Log((Time.unscaledTime - LastPowerDash) /statistics[StatisticType.PDCoolDown]);
        powerDashCoolDown.GetComponent<Slider>().value = Mathf.Max(0,Mathf.Min(1,(Time.unscaledTime - LastPowerDash) /statistics[StatisticType.PDCoolDown] ));

        if (!PowerDashReady)
        {
            if(LastPowerDash + statistics[StatisticType.PDCoolDown] < Time.unscaledTime)
            {
                PowerDashReady = true;
                buttonTip.SetActive(true);
            }
            else
            {
                buttonTip.SetActive(false);
            }
            
        }

    }
        

    public void UpdatePowerDashUI()
    {
        if (!PowerDashUnlock && powerDashCoolDown != null)
        {
            powerDashCoolDown.GetComponent<Slider>().value = 0f;
            buttonTip?.SetActive(false);
            return;
        }

        powerDashCoolDown.GetComponent<Slider>().value = Mathf.Max(0, Mathf.Min(1, (Time.unscaledTime - LastPowerDash) / statistics[StatisticType.PDCoolDown]));

        if (!PowerDashReady)
        {
            if (LastPowerDash + statistics[StatisticType.PDCoolDown] < Time.unscaledTime)
            {
                buttonTip.SetActive(true);
            }
            else
            {
                buttonTip.SetActive(false);
            }

        }

    }

    public bool AddKillCount(float amount)
    {
        StatisticModificationResult result = statistics.Modify(StatisticType.KsCount, amount, 0, statistics[StatisticType.MaxKs]);
    
        if (result.currentValue >= statistics[StatisticType.MaxKs])
             
            ActivateKillStreak();
        else
            DeactivateKillStreak();
         

        lastKillStreakDecaySecond = Time.time;


        return true;
    }
    
    public void ActivateKillStreak()
    {
        if (InKillStreak)
            return;

        AudioManager.Singleton.PlayOnce("KillStreak");

        InKillStreak = true;
        SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        TimeManager.Instance.startSlowMotion(0.5f);
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText("Kill Streak!!!");
    }

    public void DeactivateKillStreak()
    {
        if (!InKillStreak)
            return;


        InKillStreak = false;
        SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
    }


    public void StartDialogue(int dialogueID, params Action[] callbacks)
    {
        currentDialogueID = dialogueID;
        currentDialogueCallbacks = callbacks;
        fsm.CurrentStateName = "InDialogue";
    }


    public void ResetStatistics()
    {
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];
        statistics[StatisticType.Sp] = statistics[StatisticType.MaxSp];
        statistics[StatisticType.Osp] = 0;
        statistics[StatisticType.UltimateEnergy] = 0;
        statistics[StatisticType.KsCount] = 0;
        statistics[StatisticType.KsDecay] = 5;
        statistics[StatisticType.MaxKs] = 2;
    }


    public void init()
    {
        player = Player.CurrentPlayer == null ? Player.CreatePlayer() : Player.CurrentPlayer;


        statistics = new StatisticSystem(player.attributes, player.inventory);

        ResetStatistics();


        OnStatisticChange = statistics.onStatisticChange;


        rigidbody = GetComponent<Rigidbody2D>();


        fsm = fsm.Initialize(this);
        fsm.Boot();

        //GUIManager.Singleton.Open("MainMenu", this);
        //StartDialogue(10102001);

        // TODO: delete later
        if (buttonTip == null) buttonTip = GameObject.Find("HealthButton");
        if (powerDashCoolDown == null) powerDashCoolDown = GameObject.Find("HealthEnergy");
    }


    private void Awake()
    {
        if (Singleton)
            Destroy(gameObject);
        else
        {
            Singleton = this;


            init();
        }
    }

    private void OnEnable()
    {
        GUIManager.Singleton.Open("HUD", this);
    }

    private void Update()
    {
        if (InFever)
            ConsumeUltimateEnergy(statistics[StatisticType.FeverDecay] * Time.deltaTime);
        
       
        PowerDashCoolDown();
        KillStreakDecay();


        fsm.Update();
    }
    
    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
