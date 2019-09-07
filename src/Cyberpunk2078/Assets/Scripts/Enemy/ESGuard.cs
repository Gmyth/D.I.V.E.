using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Guard", menuName = "Enemy State/Guard")]
public class ESGuard : DroneState
{
    [Header("Configuration")]
    [SerializeField] private float minTurnTime = 3f;
    [SerializeField] private float maxTurnTime = 6f;

    [Header("Connected States")]
    [SerializeField] private int index_ESIdle = -1;
    [SerializeField] private int index_ESAlert = -1;

    private float t0 = 0;


    public override int Update()
    {
        if (Time.time >= t0)
        {
            // TODO: Turn

            t0 += Random.Range(minTurnTime, maxTurnTime);
        }

        // TODO: Player detection

        return Index;
    }

    public override void OnStateEnter()
    {
        t0 = Time.time + Random.Range(minTurnTime, maxTurnTime);
    }
}
