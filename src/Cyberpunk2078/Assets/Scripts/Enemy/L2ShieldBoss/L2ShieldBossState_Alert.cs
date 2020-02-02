using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Alert", menuName = "Enemy State/Level 2/Shield Boss/Alert")]
public class L2ShieldBossState_Alert : ESAlert<L2ShieldBoss>
{
    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.AdjustFacingImmediately();
    }

    public override string Update()
    {
        enemy.AdjustFacing();


        if (Time.time >= t_finishWait)
        {
            return "DiveBomb";

            float r = Random.Range(0, 100);


            switch (enemy.statusModifiers[AttributeType.MaxFatigue_m0])
            {
                case 0:
                    return states_attacks[0];


                case 1:
                    if (r < 70)
                        return states_attacks[0];

                    return states_attacks[1];


                case 2:
                    if (r < 60)
                        return states_attacks[0];

                    if (r < 95)
                        return states_attacks[1];

                    
                    return states_attacks[2];
            }
        }


        return Name;
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.DisableHitBox(3);
    }
}
