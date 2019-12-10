using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;

    public Transform SpriteHolder;
    private new Rigidbody2D rigidbody;
    public GameObject groundDust;

    private float lastKillStreakDecaySecond;
    private Player player;


    public EventOnStatisticChange OnStatisticChange { get; private set; }

    public PlayerState State => fsm.CurrentState;


    public float this[StatisticType statisticType]
    {
        get
        {
            return statistics[statisticType];
        }
    }

    public FSMPlayer GetFSM()
    {
        return fsm;
    }

    public bool InKillStreak { get; private set; } = false;
    
    public bool InFever { get; private set; } = false;
    public bool MaxUltimateEnergy { get; private set; }
    //public PlayerCharacter(Player player)
    //{
    //    statistic = new StatisticSystem(player.attributes, player.inventory);


    //    OnStatisticChange = statistic.onStatisticChange;
    //}

 
    public void Knockback(Vector3 origin, float force, float duration = 1f)
    {
        Vector3 direction = transform.position - origin;
        Vector2 groundNormal = GroundNormal;

        direction = ((direction.x > 0 ? groundNormal.Right().normalized : groundNormal.Left().normalized) + groundNormal).normalized * force;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(direction);
        Player.CurrentPlayer.knockBackDuration = duration;
        fsm.CurrentStateIndex = 10;
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Take {0} damage.", rawDamage));


        if (rawDamage <= 0)
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -rawDamage, 0);

        if (result.currentValue <= 0)
            Dead();


        return result.previousValue - result.currentValue;
    }

    public float Heal(float rawHeal)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Heal {0}.", rawHeal));


        if (rawHeal <= 0)
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, rawHeal, 0, statistics[StatisticType.MaxHp]);


        return result.previousValue - result.currentValue;
    }

    public override void Dead()
    {
        fsm.CurrentStateIndex = 9;

        ResetStatistics();


        ObjectRecycler.Singleton.RecycleAll();
        CheckPointManager.Instance?.RestoreCheckPoint();
    }

    public bool ActivateFever()
    {
        if (statistics[StatisticType.UltimateEnergy] > 30f)
        {
            InFever = true;
            CameraManager.Instance.FlashIn(6,0.05f,0.05f,0.05f);
            AddOverLoadEnergy(1);
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
        
        return true;
    }

    public bool ConsumeUltimateEnergy(float value)
    {
        StatisticModificationResult result = statistics.Modify(StatisticType.UltimateEnergy, -value, 0, statistics[StatisticType.MaxUltimateEnergy]);

        if (result.currentValue <= 0 && InFever)
        {
            InFever = false;
            GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").DehighlightFeverBar();
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
        
        InKillStreak = true;
        SpriteHolder.GetComponent<GhostSprites>().Occupied = true;
        
        GUIManager.Singleton.GetGUIWindow<GUIHUD>("HUD").ShowText("Kill Streak!!!");
    }

    public void DeactivateKillStreak()
    {
        if (!InKillStreak)
            return;


        InKillStreak = false;
        SpriteHolder.GetComponent<GhostSprites>().Occupied = false;
        
    }


    private void ResetStatistics()
    {
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];
        statistics[StatisticType.Sp] = statistics[StatisticType.MaxSp];
        statistics[StatisticType.Osp] = 0;
        statistics[StatisticType.UltimateEnergy] = 0;
        statistics[StatisticType.KsCount] = 0;
        statistics[StatisticType.KsDecay] = 5;
        statistics[StatisticType.MaxKs] = 2;
    }


    private void Awake()
    {
        if (Singleton)
            Destroy(gameObject);
        else
            Singleton = this;
    }

    private void Start()
    {
        player = Player.CurrentPlayer == null ? Player.CreatePlayer() : Player.CurrentPlayer;


        statistics = new StatisticSystem(player.attributes, player.inventory);

        ResetStatistics();


        OnStatisticChange = statistics.onStatisticChange;


        rigidbody = GetComponent<Rigidbody2D>();


        fsm = fsm.Initialize(this);
        fsm.Boot();


        ((GUIHUD)GUIManager.Singleton.Open("HUD", this)).ShowDialogue(DataTableManager.singleton.GetDialogueData(0));
    }

    private void Update()
    {
        if (InFever)
            ConsumeUltimateEnergy(statistics[StatisticType.FeverDecay] * Time.deltaTime);
        
        KillStreakDecay();


        fsm.Update();
    }

    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
