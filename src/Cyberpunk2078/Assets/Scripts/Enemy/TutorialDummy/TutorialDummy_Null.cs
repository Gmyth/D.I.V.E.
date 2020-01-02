using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "TutorialDummy_Null", menuName = "Enemy State/Tutorial Dummy/Null")]
public class TutorialDummy_Null : EnemyState
{

    public override int Update()
    {
        return Index;
    }

}
