using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;

    private new Rigidbody2D rigidbody;


    private Player player;

    public GameObject dashAtkBox;


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

    //public PlayerCharacter(Player player)
    //{
    //    statistic = new StatisticSystem(player.attributes, player.inventory);


    //    OnStatisticChange = statistic.onStatisticChange;
    //}

 
    public void Knockback(Vector3 origin, float force)
    {
        Vector3 direction = transform.position - origin;
        direction = (direction.x > 0 ? GroundNormal.Right().normalized : GroundNormal.Left().normalized) * force;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(direction);


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

        // Player.CurrentPlayer.ApplyHealthChange(-rawDamage);


        return result.previousValue - result.currentValue;
    }
    
    public override void Dead()
    {
        fsm.CurrentStateIndex = 9;

        ResetStatistics();


        ObjectRecycler.Singleton.RecycleAll();
        CheckPointManager.Instance?.RestoreCheckPoint();
    }


    public bool AddNormalEnergy(float amount)
    {
        float maxSp = statistics[StatisticType.MaxSp];

        statistics.Modify(StatisticType.Sp, amount, 0, maxSp);


        return true;
    }

    public bool AddOverLoadEnergy(float amount)
    {
        float maxSp = statistics[StatisticType.MaxOsp];

        statistics.Modify(StatisticType.Osp, amount, 0, maxSp);


        return true;
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


    public bool AddFever(float amount)
    {
        statistics.Modify(StatisticType.Fever, amount, 0, statistics[StatisticType.MaxFever]);

        return true;
    }

    public bool ConsumeFever()
    {
        if (statistics[StatisticType.Fever] == statistics[StatisticType.MaxFever] && statistics[StatisticType.Hp] < statistics[StatisticType.MaxHp])
        {
            statistics.Modify(StatisticType.Hp, 1, 0, statistics[StatisticType.MaxHp]);
            statistics[StatisticType.Fever] = 0;

            return true;
        }

        return false;
    }


    private void ResetStatistics()
    {
        statistics[StatisticType.Hp] = statistics[StatisticType.MaxHp];
        statistics[StatisticType.Sp] = statistics[StatisticType.MaxSp];
        statistics[StatisticType.Osp] = 0;
        statistics[StatisticType.Fever] = 0;
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


        dashAtkBox = GetComponentInChildren<HitBox>()?.gameObject;
        if (!dashAtkBox)
            dashAtkBox = GetComponentInChildren<Attack>()?.gameObject;

        dashAtkBox.SetActive(false);


        rigidbody = GetComponent<Rigidbody2D>();


        fsm = fsm.Initialize(this);
        fsm.Boot();


        GUIManager.Singleton.Open("HUD", this);
    }

    private void Update()
    {
        fsm.Update();
    }

    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
