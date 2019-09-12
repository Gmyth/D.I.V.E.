using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    [SerializeField] private FSMPlayer fsm;

    private StatisticSystem statistic;
    private new Rigidbody rigidbody;


    public PlayerCharacter(Player player)
    {
        statistic = new StatisticSystem(player.Inventory);
    }


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        fsm.Initialize(this);
        fsm.Boot();
    }

    private void Update()
    {
        fsm.Update();
    }

}
