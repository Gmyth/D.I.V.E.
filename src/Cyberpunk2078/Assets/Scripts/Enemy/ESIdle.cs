﻿using UnityEngine;


[CreateAssetMenuAttribute(fileName = "ES_Idle", menuName = "Enemy State/Idle")]
public class ESIdle : EnemyState
{
    [Header("Connected States")]
    [SerializeField] private int index_ESAlert;


    public override int Update()
    {
        // TODO: Player dectection

        return Index;
    }
}
