using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Dummy
{
    private StatisticSystem statistic;
    private new Rigidbody rigidbody;


    public PlayerCharacter(Player player)
    {
        statistic = new StatisticSystem(player.Inventory);
    }


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Input.GetAxis("Horizontal"));

        if (Input.GetButtonDown("Jump"))
            rigidbody.AddForce(Vector3.up);
    }
}
