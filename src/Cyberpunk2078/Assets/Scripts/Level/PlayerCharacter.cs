using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;
    
    
    private StatisticSystem statistic;
    private new Rigidbody rigidbody;

    public GameObject dashAtkBox;

    public PlayerState State => fsm.CurrentState;


    public PlayerCharacter(Player player)
    {
        statistic = new StatisticSystem(player.Inventory);
    }


    public override float ApplyDamage(int instanceId,float rawDamage, bool overWrite = false)
    {
        Debug.Log(LogUtility.MakeLogStringFormat("PlayerCharacter", "Take {0} damage.", rawDamage));
        Player.CurrentPlayer.ApplyHealthChange(-rawDamage);
        return rawDamage;
    }


    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }


        Singleton = this;

        dashAtkBox = GetComponentInChildren<Attack>().gameObject;
        dashAtkBox.SetActive(false);

        rigidbody = GetComponent<Rigidbody>();

        fsm.Initialize(this);
        fsm.Boot();
    }

    private void Update()
    {
        fsm.Update();
    }

    public override void Dead()
    {
        //game over
    }
    
    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
