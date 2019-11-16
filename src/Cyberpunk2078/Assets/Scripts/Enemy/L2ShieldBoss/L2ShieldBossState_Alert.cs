using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Alert", menuName = "Enemy State/Level 2/Shield Boss/Alert")]
public class L2ShieldBossState_Alert : ESAlert<L2ShieldBoss>
{
    public override int Update()
    {
        return stateIndex_attacks[0];
    }
}
