using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;

    private StatisticSystem statistic;
    private new Rigidbody rigidbody;


    public PlayerCharacter(Player player)
    {
        statistic = new StatisticSystem(player.Inventory);
    }


    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }


        Singleton = this;


        rigidbody = GetComponent<Rigidbody>();

        fsm.Initialize(this);
        fsm.Boot();
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
